using System.Diagnostics;

namespace Snobol4.Common;

/// <summary>
/// MarkPattern — always succeeds, but first pushes a -3 mark onto the alternate
/// stack at FENCE entry.  Paired with SealPattern: when P matches and Seal fires,
/// the Seal pops down to (and through) this mark, preserving outer alternates
/// that were on the stack before FENCE was entered.
/// FENCE(p) structure: ConcatenatePattern(Mark, ConcatenatePattern(p, Seal)).
/// If P fails before Seal fires, the mark is stale; RestoreAlternate and
/// HasAlternates both treat stale -3 entries as transparent.
/// Mirrors CSNOBOL4 FNCD's "inner base" / SPITBOL xkalt's saved P-stack pointer.
/// </summary>
[DebuggerDisplay("{DebugPattern()}")]
internal class MarkPattern : TerminalPattern
{
    internal override Pattern Clone() => new MarkPattern();

    internal override MatchResult Scan(int node, Scanner scan)
    {
        using var profile1 = Profiler.Start4("Mark", scan.Exec);
        scan.MarkAlternates();
        return MatchResult.Success(scan);
    }

    public override string DebugPattern() => "mark";
}
