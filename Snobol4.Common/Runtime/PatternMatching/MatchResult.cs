namespace Snobol4.Common;

public class MatchResult
{
                internal int PreCursor { get; }
    
                internal int PostCursor { get; }
    
                internal Status Outcome { get; }

                public enum Status 
    { 
                SUCCESS, 
        
                FAILURE, 
        
                ABORT,

                // GOTO: the pattern has grafted new nodes into the scanner's AST.
                // The scanner should jump to GotoNode to continue matching.
                GOTO
    }

    // Valid when Outcome == GOTO: the node index in the scanner's (now-extended) AST
    // at which matching should continue.
    internal int GotoNode { get; private set; } = -1;

    public bool IsSuccess => Outcome == Status.SUCCESS;
    
                public bool IsFailure => Outcome == Status.FAILURE;
    
                public bool IsAbort => Outcome == Status.ABORT;

    public bool IsGoto => Outcome == Status.GOTO;

                                public int MatchLength => PostCursor - PreCursor;

    #region Factory Methods

                        internal static MatchResult Failure(Scanner scan)
        => new(scan.CursorPosition, scan.CursorPosition, Status.FAILURE);

                        internal static MatchResult Failure(ScannerState state)
        => new(state.CursorPosition, state.CursorPosition, Status.FAILURE);

                        internal static MatchResult Abort(Scanner scan)
        => new(scan.CursorPosition, scan.CursorPosition, Status.ABORT);

                        internal static MatchResult Abort(ScannerState state)
        => new(state.CursorPosition, state.CursorPosition, Status.ABORT);

    internal static MatchResult Goto(Scanner scan, int gotoNode)
        => new(scan.CursorPosition, scan.CursorPosition, Status.GOTO) { GotoNode = gotoNode };

                        internal static MatchResult Success(Scanner scan)
        => new(scan.PreviousCursorPosition, scan.CursorPosition, Status.SUCCESS);

                        internal static MatchResult Success(ScannerState state)
        => new(state.PreviousCursorPosition, state.CursorPosition, Status.SUCCESS);

    #endregion

                            private MatchResult(int preCursor, int postCursor, Status status)
    {
        PreCursor = preCursor;
        PostCursor = postCursor;
        Outcome = status;
    }
}