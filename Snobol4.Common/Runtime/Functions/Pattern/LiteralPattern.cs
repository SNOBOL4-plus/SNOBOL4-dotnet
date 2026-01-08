namespace Snobol4.Common;

internal class LiteralPattern : TerminalPattern
{
    #region Members

    internal string Literal;

    #endregion

    #region Construction

    /// <summary>
    /// Class for matching strings
    /// </summary>
    /// <param name="literal">String to match</param>
    /// <exception cref="ArgumentNullException">String to match cannot be null</exception>
    internal LiteralPattern(string literal)
    {
        Literal = literal;
    }

    #endregion

    #region Methods


    internal override Pattern Clone()
    {
        return new LiteralPattern(Literal);
    }

    internal override MatchResult Scan(int node, Scanner scan)
    {
        if (!scan.Subject[scan.CursorPosition..].StartsWith(Literal))
            return MatchResult.Failure(scan);

        scan.CursorPosition += Literal.Length;
        return MatchResult.Success(scan);
    }

    #endregion
}