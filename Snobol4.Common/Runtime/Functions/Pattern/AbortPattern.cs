namespace Snobol4.Common;

internal class AbortPattern : TerminalPattern
{
    #region Methods

    internal override MatchResult Scan(int node, Scanner scan)
    {
        return MatchResult.Abort(scan);
    }

    internal override Pattern Clone() => new AbortPattern();

    #endregion
}