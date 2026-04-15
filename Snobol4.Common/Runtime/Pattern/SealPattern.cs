using System.Diagnostics;

namespace Snobol4.Common;

/// <summary>
/// SealPattern — always succeeds, but first discards all saved alternates and pushes
/// an abort sentinel. Any subsequent backtrack through this point returns ABORT.
/// Used by FENCE(p): structure is ConcatenatePattern(p, SealPattern()) so that once
/// p matches, its interior alternates are sealed and cannot be re-entered.
/// </summary>
[DebuggerDisplay("{DebugPattern()}")]
internal class SealPattern : TerminalPattern
{
    #region Methods

    internal override Pattern Clone() => new SealPattern();

    internal override MatchResult Scan(int node, Scanner scan)
    {
        using var profile1 = Profiler.Start4("Seal", scan.Exec);
        scan.SealAlternates();
        return MatchResult.Success(scan);
    }

    #endregion

    #region Debugging

    public override string DebugPattern() => "seal";

    #endregion
}
