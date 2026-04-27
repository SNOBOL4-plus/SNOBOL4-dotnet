// MonitorIpc.cs — snobol4dotnet sync-step monitor bridge.
//
// Wire protocol: see one4all/scripts/monitor/monitor_wire.h.
// Reference port: csnobol4/monitor_ipc_runtime.c.
//
// Design (S-2-bridge-1, GOAL-NET-BEAUTY-SELF):
//   - Statically linked into Snobol4.Common (no plugin).
//   - No SNOBOL4 LOAD() involvement.  Runtime fire-points call
//     EmitValue / EmitCall / EmitReturn directly.
//   - Lazy init on first emit: reads MONITOR_READY_PIPE / MONITOR_GO_PIPE.
//     If unset, all entry points become silent no-ops (single bool check
//     after init — zero overhead on normal beauty 17/17 runs).
//   - Auto-interns names into a growing in-memory dictionary.  At process
//     exit (AppDomain.ProcessExit), the table is dumped to MONITOR_NAMES_OUT.
//     We also flush the names file after every emit so partial state is
//     recoverable if shutdown skips ProcessExit.
//   - End record (MWK_END) emitted at exit before the names sidecar is
//     written, so the controller sees a clean wire close.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace Snobol4.Common;

public static class MonitorIpc
{
    // --- Event kinds (record.kind) -----------------------------------------
    private const uint MWK_VALUE  = 1u;
    private const uint MWK_CALL   = 2u;
    private const uint MWK_RETURN = 3u;
    private const uint MWK_END    = 4u;

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
    private static string?     _namesOutPath;
    private static readonly object _lock = new();

    // Auto-interning name table.  Linear-scan-on-miss via Dictionary.
    private static readonly List<string>             _names      = new();
    private static readonly Dictionary<string, uint> _nameToId   = new();

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
            string? namesPath = Environment.GetEnvironmentVariable("MONITOR_NAMES_OUT");
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

            _namesOutPath = string.IsNullOrEmpty(namesPath) ? null : namesPath;
            _initOk = true;
            AppDomain.CurrentDomain.ProcessExit += (_, _) => OnAtExit();
        }
    }

    // ------------------------------------------------------------------
    // Name interning — append on miss.  Returns name_id (>= 0) or
    // MW_NAME_ID_NONE on failure.
    // ------------------------------------------------------------------
    private static uint InternName(string s)
    {
        if (s is null) return MW_NAME_ID_NONE;
        if (_nameToId.TryGetValue(s, out uint id)) return id;
        id = (uint)_names.Count;
        _names.Add(s);
        _nameToId[s] = id;
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
                byte[] b = Encoding.UTF8.GetBytes(sv.Data ?? "");
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
            // Encode rtnType as a NAME-typed value so the wire matches csn's
            // shape: emit_descr_value(MWK_RETURN, name_id, &rtntype_descr).
            byte[] b = Encoding.UTF8.GetBytes(rtnType ?? "RETURN");
            EmitRecordRaw(MWK_RETURN, nameId, MWT_NAME, b, (uint)b.Length);
        }
    }

    // ------------------------------------------------------------------
    // ProcessExit: emit MWK_END and dump names sidecar.
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
            FlushNamesSidecar();
            try { _readyFs?.Dispose(); } catch { } _readyFs = null;
            try { _goFs?.Dispose();    } catch { } _goFs    = null;
        }
    }

    private static void FlushNamesSidecar()
    {
        if (string.IsNullOrEmpty(_namesOutPath)) return;
        try
        {
            using var w = new StreamWriter(_namesOutPath!, append: false, Encoding.UTF8);
            foreach (var s in _names)
            {
                w.WriteLine(s);
            }
        }
        catch { /* swallow — we tried */ }
    }
}
