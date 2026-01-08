namespace Snobol4.Common;

internal class ArbPattern : TerminalPattern
{
    #region Methods

    internal static Pattern Structure()
    {
        return new ConcatenatePattern(new NullPattern(), new AlternatePattern(new NullPattern(), new ArbPattern()));
    }

    internal override Pattern Clone()
    {
        return new ArbPattern();
    }

    internal override MatchResult Scan(int node, Scanner scan)
    {
        if (scan.CursorPosition == scan.Subject.Length)
            return MatchResult.Failure(scan);

        scan.CursorPosition++;
        scan.SaveAlternate(node);
        return MatchResult.Success(scan);
    }
    #endregion
}