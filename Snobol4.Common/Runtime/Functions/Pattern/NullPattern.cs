namespace Snobol4.Common;

internal class NullPattern : LiteralPattern
{
    #region Construction

    internal NullPattern() : base("")
    {
    }

    #endregion

    #region Methods

    /// <summary>
    /// Null Pattern. A pattern that always succeeds/
    /// Also known "StarNull"
    /// </summary>
    /// <param name="node">Index of this pattern in the AbstractSynTaxTree</param>
    /// <param name="scan">root pattern for AbstractSyntaxTree</param>
    /// <returns>Match results, which is always success</returns>
    internal override MatchResult Scan(int node, Scanner scan)
    {
        return MatchResult.Success(scan);
    }

    internal override Pattern Clone()
    {
        return new NullPattern();
    }

    #endregion
}