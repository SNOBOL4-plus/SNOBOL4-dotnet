namespace Snobol4.Common;

internal class SucceedPattern : TerminalPattern
{
    #region Methods

    internal override Pattern Clone()
    {
        return new SucceedPattern();
    }

    internal override MatchResult Scan(int node, Scanner scan)
    {
        return MatchResult.Success(scan);
    }

    #endregion
}