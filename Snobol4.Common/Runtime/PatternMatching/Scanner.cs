namespace Snobol4.Common;

public class Scanner
{
    internal int PreviousCursorPosition
    {
        get => _state?.PreviousCursorPosition ?? 0;
        set
        {
            if (_state is not null)
                _state.PreviousCursorPosition = value;
        }
    }

    internal int CursorPosition
    {
        get => _state?.CursorPosition ?? 0;
        set
        {
            if (_state is not null)
                _state.CursorPosition = value;
        }
    }

    internal string Subject => _state?.Subject ?? "";
    internal Executive Exec { get; }

    private ScannerState? _state;
    private AbstractSyntaxTree? _ast;

    internal Scanner(Executive exec)
    {
        Exec = exec;
    }

    internal MatchResult PatternMatch(string subject, Pattern pattern, int startPosition, bool anchor)
    {
        _ast = AbstractSyntaxTree.Build(pattern);
        _state = new ScannerState(subject, startPosition);

        var length = anchor ? 0 : subject.Length;

        for (var cursorPosition = startPosition; cursorPosition <= length; ++cursorPosition)
        {
            _state.PreviousCursorPosition = _state.CursorPosition = cursorPosition;
            var mr = Match(_ast.StartNode);
            if (mr.IsSuccess || mr.IsAbort)
                return mr;
        }

        return MatchResult.Failure(_state);
    }

    internal void SaveAlternate(int node)
    {
        _state?.SaveAlternate(node);
    }

    internal void SealAlternates()
    {
        _state?.SealAlternates();
    }

    internal void MarkAlternates()
    {
        _state?.MarkAlternates();
    }

    // Graft the nodes of subPattern into this scanner's live AST, wiring the last
    // node's Subsequent to successorNodeIndex (the node that follows *X in the outer
    // pattern).  Returns the index of the grafted sub-tree's start node so the Match
    // loop can jump there via MatchResult.Goto.
    internal int Graft(Pattern subPattern, int successorNodeIndex)
        => _ast!.Graft(subPattern, successorNodeIndex);

    internal AbstractSyntaxTreeNode GetNode(int index) => _ast![index];

    // S-2-bridge-7-byrd-pattern: tag a pattern node for the wire trace.
    // UnevaluatedPattern reports its function name (e.g. "*snoString").
    // LiteralPattern reports its literal in single-quotes (e.g. 'h', '"').
    // Other terminal patterns report their type name (BreakPattern, SpanPattern, ...).
    private static string NodeTag(AbstractSyntaxTreeNode node)
    {
        if (node.Self is UnevaluatedPattern up) return up.MethodName;
        if (node.Self is LiteralPattern lp)     return "'" + lp.Literal + "'";
        return node.Self.GetType().Name;
    }

    private MatchResult Match(AbstractSyntaxTreeNode node)
    {
        _state!.ClearAlternates();

        while (true)
        {
            if (node.HasAlternate())
            {
                _state.SaveAlternate(node.Alternate);
            }

            // S-2-bridge-7-byrd-pattern: PM_CALL — entering a node match.
            MonitorIpc.EmitPmCall(NodeTag(node), _state.CursorPosition);

            Exec.Failure = false;
            var mr = ((TerminalPattern)node.Self).Scan(node.SelfIndex, this);

            switch (mr.Outcome)
            {
                case MatchResult.Status.SUCCESS:
                    // PM_EXIT — node Scan succeeded; advancing cursor reflected.
                    MonitorIpc.EmitPmExit(NodeTag(node), _state.CursorPosition);
                    if (!node.HasSubsequent())
                        return MatchResult.Success(_state);
                    node = node.GetSubsequent()!;
                    break;

                case MatchResult.Status.FAILURE:
                    if (!_state.HasAlternates())
                    {
                        // PM_FAIL — propagate FAILURE outward (no live alt).
                        MonitorIpc.EmitPmFail(NodeTag(node), _state.CursorPosition);
                        return mr;
                    }
                    var (alternateIndex, _) = _state.RestoreAlternate();
                    // Seal hit on backtrack: per Gimpel 1973 FENCE = NULL | ABORT,
                    // backtrack INTO the sealed region is blocked.  But OUTER
                    // alternates saved BEFORE the FENCE was entered (and which
                    // sit BELOW the seal on the alt stack — preserved by
                    // SealAlternates) must still be allowed to fire — they
                    // represent OTHER ways the OUTER pattern could match
                    // without re-entering the FENCE.  Pop the seal and
                    // continue to the next non-seal entry.  If only the floor
                    // (-1) remains, propagate ABORT to suppress unanchored
                    // cursor-retry in PatternMatch (per session #66 correction).
                    while (alternateIndex == -2)
                    {
                        if (!_state.HasAlternates())
                        {
                            // PM_FAIL — seal-only stack, ABORT propagates outward.
                            MonitorIpc.EmitPmFail(NodeTag(node), _state.CursorPosition);
                            return MatchResult.Abort(_state);
                        }
                        (alternateIndex, _) = _state.RestoreAlternate();
                    }
                    node = _ast![alternateIndex];
                    // PM_REDO — backtracked into restored node at restored cursor.
                    MonitorIpc.EmitPmRedo(NodeTag(node), _state.CursorPosition);
                    break;

                case MatchResult.Status.ABORT:
                    // PM_FAIL — explicit ABORT from a Scan() (e.g. AbortPattern, seal).
                    MonitorIpc.EmitPmFail(NodeTag(node), _state.CursorPosition);
                    return mr;

                case MatchResult.Status.GOTO:
                    node = _ast![mr.GotoNode];
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    internal void Dump(Pattern rootPattern)
    {
        _ast?.Dump();
    }
}