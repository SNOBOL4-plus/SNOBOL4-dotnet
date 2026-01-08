namespace Snobol4.Common;

internal class RemPattern : TerminalPattern
{
    #region Internal Methods

    internal override Pattern Clone()
    {
        return new RemPattern();
    }

    internal override MatchResult Scan(int node, Scanner scan)
    {
        scan.CursorPosition = scan.Subject.Length;
        return MatchResult.Success(scan);
    }

    #endregion
}