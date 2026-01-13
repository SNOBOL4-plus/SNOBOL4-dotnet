using System.Diagnostics;

namespace Snobol4.Common;

/// <summary>
/// Represents the FENCE pattern in SNOBOL4, which prevents backtracking past a certain point.
/// Implemented as: pattern | ABORT
/// </summary>
/// <remarks>
/// <para>
/// FENCE is a pattern construction function in SNOBOL4 that creates a "fence" preventing
/// backtracking. Once a FENCE pattern succeeds, the pattern matcher cannot backtrack
/// past that point to try alternative matches. The FENCE function accepts a pattern
/// argument and returns a new pattern that either succeeds with the argument pattern
/// or aborts matching entirely.
/// </para>
/// <para>
/// The FENCE pattern is structurally equivalent to: <c>pattern | ABORT</c>
/// - If the pattern succeeds, FENCE succeeds
/// - If the pattern fails, ABORT is invoked, terminating all backtracking
/// </para>
/// <para>
/// Special case: <c>FENCE()</c> with no arguments or NULL pattern is equivalent to: <c>NULL | ABORT</c>
/// This creates a simple fence that always succeeds initially but prevents backtracking.
/// </para>
/// <para>
/// This implementation follows the algorithm described in:
/// Gimpel, J.F. "Algorithms in SNOBOL4." John Wiley &amp; Sons, 1976. p. 109.
/// </para>
/// <para>
/// FENCE is useful for:
/// - Optimizing pattern performance by eliminating futile backtracking
/// - Implementing committed choice in pattern matching
/// - Creating deterministic pattern behavior
/// - Preventing exponential backtracking in complex patterns
/// </para>
/// </remarks>
/// <example>
/// <code>
/// // Simple fence - commit to first match found
/// subject = 'aaabbb'
/// subject fence span('a') . result span('b')
/// // result = "aaa", no backtracking to try fewer 'a's
///
/// // Fence with pattern argument
/// subject = 'test123data'
/// subject fence(span(&amp;lcase)) . word
/// // If span(&amp;lcase) succeeds with "test", commits to it
/// // If it fails, ABORT prevents trying alternatives
///
/// // Optimize with fence to prevent backtracking
/// subject = 'programming'
/// // Without fence: tries 'prog', 'pro', 'pr', 'p' on failure
/// subject fence('prog') 'ram'    // Commits to 'prog' or aborts
///
/// // Fence vs no fence comparison
/// subject = 'ab'
/// pattern1 = ('a' | 'ab') 'b'     // Tries 'a'+'b', succeeds
/// pattern2 = fence('a' | 'ab') 'b' // Tries 'a'+'b', succeeds
/// pattern3 = ('a' | 'ab') 'c'     // Tries 'a'+'c', fails, tries 'ab'+'c', fails
/// pattern4 = fence('a' | 'ab') 'c' // Tries 'a'+'c', fails, ABORT (no 'ab' tried)
///
/// // Using FENCE() with no arguments (NULL | ABORT)
/// subject = 'test'
/// subject 'te' fence() 'st'       // Commits after matching 'te'
///
/// // Performance optimization pattern
/// expensive = complex_pattern1 | complex_pattern2 | complex_pattern3
/// fast_check = simple_test
/// pattern = fence(fast_check) expensive
/// // If fast_check fails, abort immediately without trying expensive patterns
///
/// // Deterministic parsing
/// keyword = fence('if' | 'then' | 'else' | 'while')
/// // Once a keyword matches, no backtracking to try other keywords
/// </code>
/// </example>
// ReSharper disable once UnusedMember.Global
[DebuggerDisplay("{DebugPattern()}")]
internal class FencePattern : Pattern
{
    #region Methods

    /// <summary>
    /// Creates the composite pattern structure for FENCE with a pattern argument.
    /// Implements FENCE(pattern) as: pattern | ABORT
    /// </summary>
    /// <param name="pattern">The pattern to fence. If it fails, ABORT prevents backtracking.</param>
    /// <returns>
    /// An AlternatePattern where the left alternative is the given pattern,
    /// and the right alternative is ABORT
    /// </returns>
    /// <remarks>
    /// <para>
    /// The structure ensures that:
    /// - First, the given pattern is tried
    /// - If it succeeds, the alternation succeeds
    /// - If it fails, the ABORT pattern is tried
    /// - ABORT always fails and prevents all backtracking
    /// </para>
    /// <para>
    /// This means once the pattern fails, no alternatives before the fence can be tried.
    /// The entire pattern match is terminated (aborted).
    /// </para>
    /// <para>
    /// Common use: <c>FENCE(NULL)</c> creates a simple fence that always succeeds
    /// but prevents backtracking if a subsequent pattern fails.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Create a fence around a literal pattern
    /// var pattern = FencePattern.Structure(new LiteralPattern("test"));
    /// // Equivalent to: 'test' | ABORT
    ///
    /// // Create a simple fence (NULL | ABORT)
    /// var simpleFence = FencePattern.Structure(new NullPattern());
    /// // Always succeeds initially, prevents backtracking on failure
    /// </code>
    /// </example>
    // ReSharper disable once UnusedMember.Global
    internal static Pattern Structure(Pattern pattern)
    {
        return new AlternatePattern(pattern, new AbortPattern());
    }

    /// <summary>
    /// Creates a deep copy of this FENCE pattern.
    /// </summary>
    /// <returns>
    /// A new AlternatePattern with a cloned left pattern and a new AbortPattern
    /// </returns>
    /// <exception cref="ApplicationException">
    /// Thrown if the left pattern is null, which should never happen if the
    /// pattern was constructed properly via Structure()
    /// </exception>
    /// <remarks>
    /// <para>
    /// Clone() creates a new FENCE structure by:
    /// 1. Cloning the left (fenced) pattern
    /// 2. Creating a new AbortPattern instance
    /// 3. Returning a new AlternatePattern combining them
    /// </para>
    /// <para>
    /// The null check is defensive programming - under normal circumstances,
    /// Left should never be null because FENCE is always constructed as an
    /// AlternatePattern with Left = pattern and Right = ABORT.
    /// </para>
    /// <para>
    /// Note: This class serves as a pattern factory/helper. In practice, FENCE
    /// patterns are represented directly as AlternatePattern instances created
    /// by the Structure() method.
    /// </para>
    /// </remarks>
    internal override Pattern Clone()
    {
        return Left == null
            ? throw new ApplicationException("Pattern.Clone")
            : new AlternatePattern(Left.Clone(), new AbortPattern());
    }

    #endregion


    #region Debugging

    /// <summary>
    /// Returns a debug string representation of this pattern for diagnostic purposes.
    /// </summary>
    /// <returns>The string "fence" indicating this is a FENCE pattern.</returns>
    /// <remarks>
    /// This method is used by the debugger display attribute and diagnostic tools
    /// to provide a concise, human-readable representation of the pattern.
    /// </remarks>
    public override string DebugPattern() => "fence";

    #endregion
}