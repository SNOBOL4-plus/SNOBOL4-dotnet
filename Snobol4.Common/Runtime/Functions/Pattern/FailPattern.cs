namespace Snobol4.Common;

internal class FailPattern : TerminalPattern
{
    #region Methods

    internal override FailPattern Clone()
    {
        return new FailPattern();
    }

    internal override MatchResult Scan(int node, Scanner scan)
    {
        return MatchResult.Failure(scan);
    }

    #endregion
}