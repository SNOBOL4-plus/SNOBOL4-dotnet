namespace Snobol4.Common;

/// <summary>
/// Base class for non-terminal (composite) patterns that combine other patterns.
/// Non-terminal patterns are internal nodes in the pattern tree.
/// </summary>
/// <remarks>
/// <para>
/// Non-terminal patterns do not perform actual string matching themselves. Instead,
/// they compose other patterns to create complex matching structures. The pattern
/// matcher traverses the tree of non-terminal patterns to reach terminal patterns
/// where actual matching occurs.
/// </para>
/// <para>
/// Non-terminal patterns in SNOBOL4 include:
/// - <b>ConcatenatePattern</b>: Matches left pattern followed by right pattern
/// - <b>AlternatePattern</b>: Tries left pattern, then right pattern on failure
/// </para>
/// <para>
/// During Abstract Syntax Tree (AST) construction, non-terminal patterns are traversed
/// to build a flat list of terminal patterns with precomputed links for efficient
/// sequential matching and backtracking.
/// </para>
/// <para>
/// Non-terminal patterns always have LeftPattern and RightPattern child patterns (inherited from
/// the base Pattern class). These children may themselves be terminal or non-terminal,
/// forming a tree structure.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// // Concatenation (non-terminal)
/// var concat = new ConcatenatePattern(
///     new LiteralPattern("hello"),  // terminal
///     new LiteralPattern("world")   // terminal
/// );
///
/// // Alternation (non-terminal)
/// var alternate = new AlternatePattern(
///     new LiteralPattern("cat"),    // terminal
///     new LiteralPattern("dog")     // terminal
/// );
///
/// // Nested non-terminals
/// var complex = new ConcatenatePattern(
///     new AlternatePattern(
///         new LiteralPattern("a"),
///         new LiteralPattern("b")
///     ),
///     new LiteralPattern("c")
/// );
/// // Matches "ac" or "bc"
/// </code>
/// </example>
internal abstract class NonTerminalPattern : Pattern
{
    /// <summary>
    /// Indicates that this is a non-terminal pattern (internal node in pattern tree).
    /// </summary>
    /// <returns>Always returns false for non-terminal patterns</returns>
    /// <remarks>
    /// <para>
    /// This method is used during AST construction to distinguish between:
    /// - Terminal patterns (leaf nodes) where matching logic resides
    /// - Non-terminal patterns (internal nodes) that structure the pattern tree
    /// </para>
    /// <para>
    /// When building the AST, non-terminal patterns are traversed to reach their
    /// child patterns, while terminal patterns are added to the flat AST node list.
    /// </para>
    /// </remarks>
    internal override bool IsTerminal()
    {
        return false;
    }
}