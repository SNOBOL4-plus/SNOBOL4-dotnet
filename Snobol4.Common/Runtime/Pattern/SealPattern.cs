using System.Diagnostics;

namespace Snobol4.Common;

/// <summary>
/// SealPattern — always succeeds, but first pops the alt stack down to (and
/// through) the most recent -3 mark from a paired MarkPattern, then pushes
/// sentinel -2.  On subsequent backtrack through this point, Match returns
/// ABORT outward — the entire match terminates without cursor-position retry,
/// matching Gimpel 1973: FENCE = NULL | ABORT.
/// Used by FENCE(p): structure is ConcatenatePattern(Mark, ConcatenatePattern(p, Seal)).
/// Outer alternates that were saved BEFORE the paired Mark remain live.
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
