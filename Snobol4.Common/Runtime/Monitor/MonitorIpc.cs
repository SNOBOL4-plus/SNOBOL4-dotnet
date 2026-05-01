// MonitorIpc.cs — snobol4dotnet sync-step monitor bridge.
//
// Wire protocol: see one4all/scripts/monitor/monitor_wire.h.
// Reference port: csnobol4/monitor_ipc_runtime.c, scrip src/runtime/x86/snobol4.c.
//
// Design (S-2-bridge-1, GOAL-NET-BEAUTY-SELF):
//   - Statically linked into Snobol4.Common (no plugin).
//   - No SNOBOL4 LOAD() involvement.  Runtime fire-points call
//     EmitValue / EmitCall / EmitReturn / EmitLabel directly.
//   - Lazy init on first emit: reads MONITOR_READY_PIPE / MONITOR_GO_PIPE.
//     If unset, all entry points become silent no-ops (single bool check
//     after init — zero overhead on normal beauty 17/17 runs).
//   - SN-26-bridge-coverage-e: streaming intern on the wire.  When a
//     fresh name_id is assigned, an MWK_NAME_DEF record is emitted on
//     the wire BEFORE returning the new id.  No sidecar names file is
//     read or written.  MONITOR_NAMES_OUT (legacy env var) is ignored.
//   - SN-26-bridge-coverage-f: MWK_LABEL records emitted on every
//     statement entry, carrying the SNOBOL4 source statement number
//     (same value as &STNO).  Mirrors csn's STNOCL emit and spl's
//     kvstn emit; lets the controller align participants by structural
//     statement flow without needing a label-table reverse lookup.
//   - End record (MWK_END) emitted at exit on the wire so the controller
//     sees a clean termination.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace Snobol4.Common;

public static class MonitorIpc
{
    // --- Event kinds (record.kind) -----------------------------------------
    private const uint MWK_VALUE    = 1u;
    private const uint MWK_CALL     = 2u;
    private const uint MWK_RETURN   = 3u;
    private const uint MWK_END      = 4u;
    private const uint MWK_LABEL    = 5u;   // SN-26-bridge-coverage-f: stmt entry
    private const uint MWK_NAME_DEF = 6u;   // SN-26-bridge-coverage-e: streaming intern
    // S-2-bridge-7-byrd-pattern: Byrd-box pattern-match traversal events.
    // Per AST-node Match attempt: CALL (enter), EXIT (succeed), REDO (backtrack-restore), FAIL (no alt).
    private const uint MWK_PM_CALL  = 7u;
    private const uint MWK_PM_EXIT  = 8u;
    private const uint MWK_PM_REDO  = 9u;
    private const uint MWK_PM_FAIL  = 10u;

    // --- SNOBOL4 datatype codes (record.type) ------------------------------
    private const byte MWT_NULL       = 0;
    private const byte MWT_STRING     = 1;
    private const byte MWT_INTEGER    = 2;
    private const byte MWT_REAL       = 3;
    private const byte MWT_NAME       = 4;
    private const byte MWT_PATTERN    = 5;
    private const byte MWT_EXPRESSION = 6;
    private const byte MWT_ARRAY      = 7;
    private const byte MWT_TABLE      = 8;
    private const byte MWT_CODE       = 9;
    private const byte MWT_DATA       = 10;
    private const byte MWT_FILE       = 11;
    private const byte MWT_UNKNOWN    = 255;

    private const int  MW_HDR_BYTES   = 13;
    private const uint MW_NAME_ID_NONE = 0xffffffffu;

    // --- Module state ------------------------------------------------------
    private static FileStream? _readyFs;        // write end of READY pipe
    private static FileStream? _goFs;           // read  end of GO    pipe
    private static bool        _initAttempted;
    private static bool        _initOk;
    private static bool        _atexitDone;
    private static readonly object _lock = new();

    // Auto-interning name table.  Linear-scan-on-miss via Dictionary.
    private static readonly List<string>             _names      = new();
    private static readonly Dictionary<string, uint> _nameToId   = new();

    // --- Event-bombs (S-2-bridge-event-bombs) ------------------------------
    // _emitCount increments on every wire record emitted (any kind).
    // BREAK_AT  : when _emitCount == BREAK_AT (just before the Nth emit goes
    //             out), dump the managed stack to stderr and call
    //             Debugger.Break() if a debugger is attached.  Lets a session
    //             that has identified a DIVERGE row at event N pinpoint the
    //             dot-side call stack at the last-agreed event (N-1) and the
    //             first-diverge event (N).
    // TRACE_FROM/TO : while _emitCount is in [TRACE_FROM, TRACE_TO), set the
    //             public TraceEnabled flag.  Other dot-side instrumentation
    //             (ScannerState alt-stack trace, AST dumps, etc.) reads this
    //             single flag to scope their output to the gap between
    //             last-agreed and first-diverge.
    private static long _emitCount;
    private static long _breakAt        = -1;        // -1 = disabled
    private static long _traceFrom      = long.MaxValue;
    private static long _traceTo        = long.MinValue;

    /// <summary>
    /// True while the running emit count is inside the
    /// [MONITOR_TRACE_FROM_EVENT, MONITOR_TRACE_TO_EVENT) interval.
    /// Read by other dot diagnostic instrumentation to scope its output.
    /// </summary>
    public static bool TraceEnabled => _emitCount >= _traceFrom && _emitCount < _traceTo;

    /// <summary>Current zero-based emit count (number of wire records sent).</summary>
    public static long EmitCount => _emitCount;

    /// <summary>
    /// Returns true if monitoring is active (MONITOR_READY_PIPE was set
    /// at first-emit time and FIFOs opened successfully).  Cheap after
    /// first call.
    /// </summary>
    public static bool Enabled
    {
        get
        {
            if (!_initAttempted) Init();
            return _initOk;
        }
    }

    // ------------------------------------------------------------------
    // Lazy init — read env vars on first emit.
    // ------------------------------------------------------------------
    private static void Init()
    {
        lock (_lock)
        {
            if (_initAttempted) return;
            _initAttempted = true;

            string? readyPath = Environment.GetEnvironmentVariable("MONITOR_READY_PIPE");
            string? goPath    = Environment.GetEnvironmentVariable("MONITOR_GO_PIPE");
            if (string.IsNullOrEmpty(readyPath) || string.IsNullOrEmpty(goPath))
                return;

            try
            {
                // Open READY for writing.  FIFO open(WRONLY) blocks until reader
                // is present — that is the controller's handshake.
                _readyFs = new FileStream(readyPath, FileMode.Open, FileAccess.Write,
                                          FileShare.ReadWrite, 1, FileOptions.WriteThrough);
                // Open GO for reading.  FIFO open(RDONLY) blocks until writer
                // (controller) is present.
                _goFs = new FileStream(goPath, FileMode.Open, FileAccess.Read,
                                       FileShare.ReadWrite, 1);
            }
            catch (Exception)
            {
                _readyFs?.Dispose(); _readyFs = null;
                _goFs?.Dispose();    _goFs    = null;
                return;
            }

            // SN-26-bridge-coverage-e: MONITOR_NAMES_OUT is intentionally
            // ignored.  Names are announced inline on the wire via
            // MWK_NAME_DEF records (see InternName below).  No sidecar.
            _initOk = true;

            // S-2-bridge-event-bombs: MONITOR_BREAK_AT_EVENT=N (single int)
            // and MONITOR_TRACE_FROM_EVENT=N / MONITOR_TRACE_TO_EVENT=M
            // (half-open interval).  All optional; silently ignored if unset
            // or unparseable.
            string? breakAt   = Environment.GetEnvironmentVariable("MONITOR_BREAK_AT_EVENT");
            string? traceFrom = Environment.GetEnvironmentVariable("MONITOR_TRACE_FROM_EVENT");
            string? traceTo   = Environment.GetEnvironmentVariable("MONITOR_TRACE_TO_EVENT");
            if (long.TryParse(breakAt,   out var n)) _breakAt   = n;
            if (long.TryParse(traceFrom, out var f)) _traceFrom = f;
            if (long.TryParse(traceTo,   out var t)) _traceTo   = t;

            AppDomain.CurrentDomain.ProcessExit += (_, _) => OnAtExit();
        }
    }

    // ------------------------------------------------------------------
    // Name interning — append on miss.  Returns name_id (>= 0) or
    // MW_NAME_ID_NONE on failure.
    //
    // SN-26-bridge-coverage-e (streaming intern): when a fresh id is
    // assigned and the wire is live, an MWK_NAME_DEF record is emitted
    // BEFORE returning the new id, binding (id -> name bytes) for the
    // controller's per-participant intern table.  No sidecar file.
    //
    // The NAME_DEF emit calls EmitRecordRaw, which itself acquires no
    // additional lock — every public entry point already holds _lock,
    // so this nests cleanly without deadlock.
    // ------------------------------------------------------------------
    private static uint InternName(string s)
    {
        if (s is null) return MW_NAME_ID_NONE;
        if (_nameToId.TryGetValue(s, out uint id)) return id;
        id = (uint)_names.Count;
        _names.Add(s);
        _nameToId[s] = id;
        // Announce the new binding on the wire BEFORE any record using
        // this id flows.  Silent no-op if the wire is closed.
        if (_readyFs is not null)
        {
            byte[] nameBytes = Encoding.UTF8.GetBytes(s);
            EmitRecordRaw(MWK_NAME_DEF, id, MWT_STRING, nameBytes, (uint)nameBytes.Length);
        }
        return id;
    }

    // ------------------------------------------------------------------
    // Heuristic for valid lvalue identifier.  Mirrors looks_like_identifier
    // in csnobol4/monitor_ipc_runtime.c.  Anonymous lvalues (array element
    // a<i,j>, table slot d<'k'>) get the sentinel "<lval>" so the wire
    // stays well-formed.
    // ------------------------------------------------------------------
    private static bool LooksLikeIdentifier(string s)
    {
        if (string.IsNullOrEmpty(s) || s.Length > 256) return false;
        foreach (char c in s)
        {
            if (c < 0x20 || c >= 0x7f) return false;
        }
        return true;
    }

    private static uint LvalueNameId(string nameOrEmpty)
    {
        return LooksLikeIdentifier(nameOrEmpty)
            ? InternName(nameOrEmpty)
            : InternName("<lval>");
    }

    // ------------------------------------------------------------------
    // Wait for ack: read 1 byte from GO pipe.  Returns true on 'G' (or
    // any non-'S'), false on 'S' / EOF / error.
    // ------------------------------------------------------------------
    private static bool WaitAck()
    {
        if (_goFs is null) return false;
        try
        {
            int b = _goFs.ReadByte();
            if (b < 0) return false;
            return b != (byte)'S';
        }
        catch { return false; }
    }

    // ------------------------------------------------------------------
    // Header packing (LE byte order, 13 bytes).
    // ------------------------------------------------------------------
    private static void PackHeader(byte[] hdr, uint kind, uint nameId,
                                   byte type, uint valueLen)
    {
        hdr[0]  = (byte)( kind        & 0xff);
        hdr[1]  = (byte)((kind  >>  8)& 0xff);
        hdr[2]  = (byte)((kind  >> 16)& 0xff);
        hdr[3]  = (byte)((kind  >> 24)& 0xff);
        hdr[4]  = (byte)( nameId      & 0xff);
        hdr[5]  = (byte)((nameId>>  8)& 0xff);
        hdr[6]  = (byte)((nameId>> 16)& 0xff);
        hdr[7]  = (byte)((nameId>> 24)& 0xff);
        hdr[8]  = type;
        hdr[9]  = (byte)( valueLen      & 0xff);
        hdr[10] = (byte)((valueLen>>  8)& 0xff);
        hdr[11] = (byte)((valueLen>> 16)& 0xff);
        hdr[12] = (byte)((valueLen>> 24)& 0xff);
    }

    // ------------------------------------------------------------------
    // Emit raw record (header + optional value bytes), then block on ack.
    // ------------------------------------------------------------------
    private static void EmitRecordRaw(uint kind, uint nameId, byte type,
                                      byte[]? value, uint valueLen)
    {
        if (_readyFs is null) return;

        // S-2-bridge-event-bombs: pre-emit count.  _emitCount is the
        // 1-based ordinal of the record about to be sent.  NAME_DEF records
        // are excluded so the count matches the controller's wire-log
        // numbering (which lists only CALL/VALUE/RETURN/LABEL/END events).
        if (kind != MWK_NAME_DEF) _emitCount++;

        // Break-at-event: dump managed stack to stderr and (if attached)
        // signal the debugger.  The break fires BEFORE the record is sent
        // so a stopped session sees the stack that produced the event.
        if (_breakAt >= 0 && _emitCount == _breakAt)
        {
            try
            {
                Console.Error.WriteLine(
                    $"[MonitorIpc] BREAK_AT_EVENT fired at #{_emitCount} kind={kind} type={type} valueLen={valueLen}");
                Console.Error.WriteLine(Environment.StackTrace);
                Console.Error.Flush();
            }
            catch { /* swallow */ }
            if (System.Diagnostics.Debugger.IsAttached)
                System.Diagnostics.Debugger.Break();
        }

        var hdr = new byte[MW_HDR_BYTES];
        PackHeader(hdr, kind, nameId, type, valueLen);
        try
        {
            _readyFs.Write(hdr, 0, MW_HDR_BYTES);
            if (valueLen > 0 && value is not null)
                _readyFs.Write(value, 0, (int)valueLen);
            _readyFs.Flush();
        }
        catch { return; }

        // No ack on END — controller closes its side first.
        if (kind != MWK_END) WaitAck();
    }

    // ------------------------------------------------------------------
    // Map a Var to (wire_type, value_bytes).  Mirrors emit_descr_value
    // in csnobol4/monitor_ipc_runtime.c.
    // ------------------------------------------------------------------
    private static (byte type, byte[]? bytes, uint len) ClassifyValue(Var? v)
    {
        switch (v)
        {
            case null:
                return (MWT_NULL, null, 0);
            case StringVar sv:
            {
                // SNOBOL4 strings are 8-bit byte buffers smuggled in C# strings.
                // Use Latin-1 (ISO-8859-1) to preserve bytes 0x80–0xFF verbatim.
                // UTF-8 would double-encode non-ASCII bytes (e.g. 0x80 → 0xC2 0x80).
                byte[] b = Encoding.Latin1.GetBytes(sv.Data ?? "");
                return (MWT_STRING, b, (uint)b.Length);
            }
            case IntegerVar iv:
            {
                long n = iv.Data;
                var b = new byte[8];
                for (int k = 0; k < 8; k++) b[k] = (byte)((n >> (k * 8)) & 0xff);
                return (MWT_INTEGER, b, 8);
            }
            case RealVar rv:
            {
                byte[] b = BitConverter.GetBytes(rv.Data);
                if (!BitConverter.IsLittleEndian) Array.Reverse(b);
                return (MWT_REAL, b, 8);
            }
            case NameVar nv:
            {
                string nm = nv.GetTargetName() ?? "";
                byte[] b = Encoding.UTF8.GetBytes(nm);
                return (MWT_NAME, b, (uint)b.Length);
            }
            case PatternVar:    return (MWT_PATTERN,    null, 0);
            case ExpressionVar: return (MWT_EXPRESSION, null, 0);
            case ArrayVar:      return (MWT_ARRAY,      null, 0);
            case TableVar:      return (MWT_TABLE,      null, 0);
            case CodeVar:       return (MWT_CODE,       null, 0);
            case ProgramDefinedDataVar: return (MWT_DATA, null, 0);
            default:            return (MWT_UNKNOWN,    null, 0);
        }
    }

    // ===================================================================
    // Public API — fire-points call these.  When monitoring is disabled
    // (env vars unset), all three are a single boolean check.
    // ===================================================================

    /// <summary>VALUE event — variable assignment / .-capture commit.</summary>
    public static void EmitValue(string lvalueName, Var? value)
    {
        if (!Enabled) return;
        lock (_lock)
        {
            if (!_initOk) return;
            uint nameId = LvalueNameId(lvalueName ?? "");
            if (nameId == MW_NAME_ID_NONE) return;
            var (type, bytes, len) = ClassifyValue(value);
            EmitRecordRaw(MWK_VALUE, nameId, type, bytes, len);
        }
    }

    /// <summary>CALL event — user-defined function entry.</summary>
    public static void EmitCall(string fnName)
    {
        if (!Enabled) return;
        lock (_lock)
        {
            if (!_initOk) return;
            uint nameId = InternName(fnName ?? "");
            if (nameId == MW_NAME_ID_NONE) return;
            EmitRecordRaw(MWK_CALL, nameId, MWT_NULL, null, 0);
        }
    }

    /// <summary>RETURN event — user-defined function exit.
    /// rtnType is one of "RETURN" / "NRETURN" / "FRETURN" — *how* the function
    /// exited.  The function's actual return value, if any, was delivered via
    /// a preceding EmitValue(fnName, result) on the function-name slot.</summary>
    public static void EmitReturn(string fnName, string rtnType)
    {
        if (!Enabled) return;
        lock (_lock)
        {
            if (!_initOk) return;
            uint nameId = InternName(fnName ?? "");
            if (nameId == MW_NAME_ID_NONE) return;
            // Encode rtnType as a STRING-typed value to match spl's wire encoding.
            // spl's zysrt emits the rtntype scblk (e.g. "RETURN") via spl_block_to_wire
            // which returns MWT_STRING for string blocks.  Using MWT_NAME here caused
            // a type-byte mismatch (STRING vs NAME) that stopped the monitor at every
            // function return.  See GOAL-NET-BEAUTY-SELF S-2-bridge-7-fullscan.
            byte[] b = Encoding.UTF8.GetBytes(rtnType ?? "RETURN");
            EmitRecordRaw(MWK_RETURN, nameId, MWT_STRING, b, (uint)b.Length);
        }
    }

    /// <summary>LABEL event — statement entry (SN-26-bridge-coverage-f).
    /// Fired on every statement entry (one per source statement, regardless
    /// of GOTO / fall-through).  Wire payload mirrors the oracles:
    ///   name_id = MW_NAME_ID_NONE
    ///   type    = MWT_INTEGER
    ///   value   = 8-byte LE STNO of the statement being entered
    /// Caller should pass the resolved source statement number — for dot,
    /// that is <c>SourceLineNumbers[AmpCurrentLineNumber - 1]</c>, which is
    /// the same value <c>&amp;STNO</c> evaluates to.
    /// </summary>
    public static void EmitLabel(long stno)
    {
        if (!Enabled) return;
        lock (_lock)
        {
            if (!_initOk) return;
            var b = new byte[8];
            for (int k = 0; k < 8; k++) b[k] = (byte)((stno >> (k * 8)) & 0xff);
            EmitRecordRaw(MWK_LABEL, MW_NAME_ID_NONE, MWT_INTEGER, b, 8);
        }
    }

    // ------------------------------------------------------------------
    // S-2-bridge-7-byrd-pattern — Byrd-box pattern-match wire events.
    //
    // Off by default; opt in by setting MONITOR_PM_TRACE=1 in the
    // environment.  When on, every AST-node Match attempt fires four
    // ports as it executes:
    //
    //   PM_CALL  enter Match() for node                (forward, new attempt)
    //   PM_EXIT  node Scan returned SUCCESS            (forward, advance to subsequent)
    //   PM_REDO  RestoreAlternate popped this node     (backward, retry from saved cursor)
    //   PM_FAIL  node FAILED, no alternate restored    (backward, propagate FAILURE/ABORT)
    //
    // Wire encoding: name_id = node-tag (e.g. *snoString, BREAK, LITERAL),
    // type = MWT_INTEGER, value = 8-byte LE cursor position.
    //
    // Comparison key: (kind, name, cursor).  Adjacent ports bracket
    // exactly one Scan() call — a C# trace between adjacent sync events
    // lands inside one node's match logic, surfacing the structural bug
    // directly (alternate-link wiring, scan outcome).
    // ------------------------------------------------------------------
    private static bool _pmTraceInitDone;
    private static bool _pmTraceOn;

    public static bool PmTraceEnabled
    {
        get
        {
            if (!_pmTraceInitDone)
            {
                _pmTraceInitDone = true;
                _pmTraceOn = Environment.GetEnvironmentVariable("MONITOR_PM_TRACE") == "1";
            }
            return _pmTraceOn;
        }
    }

    private static void EmitPmRecord(uint kind, string nodeTag, long cursor)
    {
        if (!Enabled) return;
        lock (_lock)
        {
            if (!_initOk) return;
            uint nameId = InternName(nodeTag ?? "");
            if (nameId == MW_NAME_ID_NONE) return;
            var b = new byte[8];
            for (int k = 0; k < 8; k++) b[k] = (byte)((cursor >> (k * 8)) & 0xff);
            EmitRecordRaw(kind, nameId, MWT_INTEGER, b, 8);
        }
    }

    public static void EmitPmCall(string nodeTag, long cursor)
    {
        if (!PmTraceEnabled) return;
        EmitPmRecord(MWK_PM_CALL, nodeTag, cursor);
    }

    public static void EmitPmExit(string nodeTag, long cursor)
    {
        if (!PmTraceEnabled) return;
        EmitPmRecord(MWK_PM_EXIT, nodeTag, cursor);
    }

    public static void EmitPmRedo(string nodeTag, long cursor)
    {
        if (!PmTraceEnabled) return;
        EmitPmRecord(MWK_PM_REDO, nodeTag, cursor);
    }

    public static void EmitPmFail(string nodeTag, long cursor)
    {
        if (!PmTraceEnabled) return;
        EmitPmRecord(MWK_PM_FAIL, nodeTag, cursor);
    }

    // ------------------------------------------------------------------
    // ProcessExit: emit MWK_END.
    //
    // SN-26-bridge-coverage-e: no sidecar dump.  Names already announced
    // inline via MWK_NAME_DEF records as they were interned.
    // ------------------------------------------------------------------
    private static void OnAtExit()
    {
        lock (_lock)
        {
            if (_atexitDone) return;
            _atexitDone = true;

            if (_initOk && _readyFs is not null)
            {
                EmitRecordRaw(MWK_END, MW_NAME_ID_NONE, MWT_NULL, null, 0);
            }
            try { _readyFs?.Dispose(); } catch { } _readyFs = null;
            try { _goFs?.Dispose();    } catch { } _goFs    = null;
        }
    }
}
