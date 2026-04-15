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

    // Graft the nodes of subPattern into this scanner's live AST, wiring the last
    // node's Subsequent to successorNodeIndex (the node that follows *X in the outer
    // pattern).  Returns the index of the grafted sub-tree's start node so the Match
    // loop can jump there via MatchResult.Goto.
    internal int Graft(Pattern subPattern, int successorNodeIndex)
        => _ast!.Graft(subPattern, successorNodeIndex);

    internal AbstractSyntaxTreeNode GetNode(int index) => _ast![index];

    private MatchResult Match(AbstractSyntaxTreeNode node)
    {
        _state!.ClearAlternates();

        while (true)
        {
            if (node.HasAlternate())
            {
                _state.SaveAlternate(node.Alternate);
            }

            Exec.Failure = false;
            var mr = ((TerminalPattern)node.Self).Scan(node.SelfIndex, this);

            switch (mr.Outcome)
            {
                case MatchResult.Status.SUCCESS:
                    if (!node.HasSubsequent())
                        return MatchResult.Success(_state);
                    node = node.GetSubsequent()!;
                    break;

                case MatchResult.Status.FAILURE:
                    if (!_state.HasAlternates())
                        return mr;
                    var (alternateIndex, _) = _state.RestoreAlternate();
                    if (alternateIndex == -2)
                        return MatchResult.Failure(_state);   // FNCD: seal hit, FAIL outward
                    node = _ast![alternateIndex];
                    break;

                case MatchResult.Status.ABORT:
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