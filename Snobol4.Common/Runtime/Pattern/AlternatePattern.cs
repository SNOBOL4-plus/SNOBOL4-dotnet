using System.Diagnostics;

namespace Snobol4.Common;

/// <summary>
/// Represents a pattern that tries alternative patterns, succeeding if either matches.
/// In SNOBOL4, alternation is expressed using the | operator.
/// </summary>
/// <remarks>
/// <para>
/// Alternation provides a way to specify multiple pattern choices. The left pattern
/// is tried first, and if it fails, the right pattern is tried. This enables
/// fallback matching and pattern variations.
/// </para>
/// <para>
/// Alternation is a composite (non-terminal) pattern with left and right children.
/// It implements backtracking: if the left pattern initially succeeds but the
/// overall match later fails, the system backtracks and tries the right pattern.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// // Match either singular or plural
/// subject = 'dogs'
/// subject ('dog' | 'dogs')        // Matches 'dogs'
///
/// // Multiple alternatives
/// vowel = ('a' | 'e' | 'i' | 'o' | 'u')
/// subject = 'hello'
/// subject 'h' vowel               // Matches 'he'
///
/// // With backtracking
/// subject = 'CATALOG FOR SEADOGS'
/// pattern = ('CAT' arb 'DOG') | ('DOG' arb 'CAT')
/// subject pattern                 // Matches first alternative
///
/// // Commonly used with ABORT to prevent backtracking
/// pattern = any('ab') | '1' abort
/// </code>
/// </example>
[DebuggerDisplay("{DebugPattern()}")]
internal class AlternatePattern : NonTerminalPattern
{
    #region Construction

    /// <summary>
    /// Creates a new alternation pattern
    /// </summary>
    /// <param name="leftPattern">The first pattern to try</param>
    /// <param name="rightPattern">The alternative pattern to try if leftPattern fails</param>
    /// <exception cref="ApplicationException">Thrown if either pattern is null</exception>
    internal AlternatePattern(Pattern leftPattern, Pattern rightPattern)
    {
        LeftPattern = leftPattern;
        RightPattern = rightPattern;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Creates a deep copy of this alternation pattern
    /// </summary>
    /// <returns>A new AlternatePattern with cloned left and right patterns</returns>
    /// <exception cref="ApplicationException">Thrown if left or right pattern is null (should never happen)</exception>
    internal override Pattern Clone()
    {
        return new AlternatePattern(LeftPattern, RightPattern);
    }

    #endregion

    #region Debugging

    /// <summary>
    /// Returns a debug string representation of this pattern for diagnostic purposes.
    /// </summary>
    /// <returns>
    /// A string in the format "&lt;left&gt; | &lt;right&gt;" showing both alternative patterns.
    /// </returns>
    /// <remarks>
    /// This method is used by the debugger display attribute and diagnostic tools
    /// to provide a concise, human-readable representation of the pattern.
    /// The pipe (|) symbol represents the alternation operator, matching SNOBOL4 syntax.
    /// Both child patterns' DebugPattern() methods are recursively called to show the complete structure.
    /// </remarks>
    public override string DebugPattern() => "alternate";

    #endregion
}