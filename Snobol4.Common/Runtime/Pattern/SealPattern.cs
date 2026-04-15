using System.Diagnostics;

namespace Snobol4.Common;

/// <summary>
/// SealPattern — always succeeds, but first clears P's saved alternates and pushes
/// sentinel -2. On subsequent backtrack through this point, Match returns FAILURE
/// (not ABORT) outward — matching SIL FNCD which propagates FAIL, not ABORT.
/// Used by FENCE(p): structure is ConcatenatePattern(p, SealPattern()).
/// </summary>
[DebuggerDisplay("{DebugPattern()}")]
internal class SealPattern : TerminalPattern
{
    internal override Pattern Clone() => new SealPattern();

    internal override MatchResult Scan(int node, Scanner scan)
    {
        using var profile1 = Profiler.Start4("Seal", scan.Exec);
        scan.SealAlternates();
        return MatchResult.Success(scan);
    }

    public override string DebugPattern() => "seal";
}
