using System.Diagnostics;

namespace Snobol4.Common;

/// <summary>
/// Represents the concatenation of two patterns that must match in sequence.
/// In SNOBOL4, concatenation is expressed by whitespace between patterns.
/// </summary>
/// <remarks>
/// <para>
/// Concatenation is the fundamental operation for building complex patterns in SNOBOL4.
/// The left pattern must match first, then the right pattern must match immediately after
/// at the position where the left pattern ended. Both patterns must succeed for the
/// concatenation to succeed.
/// </para>
/// <para>
/// This is a composite (non-terminal) pattern that has left and right child patterns.
/// During matching, the pattern matcher traverses the concatenation structure in the
/// Abstract Syntax Tree, ensuring sequential matching.
/// </para>
/// <para>
/// Concatenation is associative but not commutative:
/// - (A B) C is equivalent to A (B C)
/// - A B is NOT equivalent to B A (order matters)
/// </para>
/// <para>
/// Concatenation participates in backtracking. If the right pattern fails after the left
/// pattern succeeds, the matcher can backtrack to try alternative matches for the left
/// pattern (if it has alternatives).
/// </para>
/// </remarks>
/// <example>
/// <code>
/// // Simple concatenation
/// subject = 'programmer'
/// pattern = new ConcatenatePattern(
///     new LiteralPattern("pro"),
///     new LiteralPattern("gram")
/// );
/// subject pattern      // Success - matches 'program'
///
/// // Multiple concatenations (chaining)
/// pattern = new ConcatenatePattern(
///     new LiteralPattern("pro"),
///     new ConcatenatePattern(
///         new LiteralPattern("gram"),
///         new LiteralPattern("mer")
///     )
/// );
/// subject pattern      // Success - matches 'programmer'
///
/// // In SNOBOL4 syntax (whitespace = concatenation)
/// subject = 'hello world'
/// subject 'hello' ' ' 'world'    // Three literals concatenated
///
/// // Concatenation with pattern variables
/// p1 = 'hello'
/// p2 = ' '
/// p3 = 'world'
/// pattern = p1 p2 p3             // Concatenation in SNOBOL4
///
/// // Mixed with other patterns
/// subject = 'The quick brown fox'
/// pattern = new ConcatenatePattern(
///     new LiteralPattern("The "),
///     new ConcatenatePattern(
///         ArbPattern.Structure(),
///         new LiteralPattern(" fox")
///     )
/// );
/// subject pattern . captured     // captured = "The quick brown fox"
///
/// // Concatenation with backtracking
/// subject = 'test123'
/// pattern = new ConcatenatePattern(
///     new AlternatePattern(
///         new LiteralPattern("test"),
///         new LiteralPattern("t")
///     ),
///     new LiteralPattern("est123")
/// );
/// // First tries 'test' + 'est123' (fails)
/// // Backtracks to 't' + 'est123' (succeeds)
/// </code>
/// </example>
[DebuggerDisplay("{DebugString()}")]
internal class ConcatenatePattern : NonTerminalPattern
{
    #region Construction

    /// <summary>
    /// Creates a new concatenation pattern that matches left followed by right.
    /// </summary>
    /// <param name="left">The pattern that must match first. Cannot be null.</param>
    /// <param name="right">The pattern that must match after the left pattern. Cannot be null.</param>
    /// <exception cref="ArgumentNullException">Thrown when either left or right is null</exception>
    /// <remarks>
    /// <para>
    /// The order of patterns is significant - left always matches before right.
    /// Both patterns are stored as references; the patterns themselves are not cloned
    /// during construction (cloning occurs only in the Clone() method).
    /// </para>
    /// <para>
    /// After construction, the Left and Right properties of the base Pattern class
    /// will contain these patterns, making them available for AST construction.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Create a concatenation of two literals
    /// var pattern = new ConcatenatePattern(
    ///     new LiteralPattern("hello"),
    ///     new LiteralPattern("world")
    /// );
    ///
    /// // Create nested concatenation
    /// var complex = new ConcatenatePattern(
    ///     new LiteralPattern("a"),
    ///     new ConcatenatePattern(
    ///         new LiteralPattern("b"),
    ///         new LiteralPattern("c")
    ///     )
    /// );
    /// </code>
    /// </example>
    internal ConcatenatePattern(Pattern left, Pattern right)
    {
        Left = left ?? throw new ArgumentNullException(nameof(left));
        Right = right ?? throw new ArgumentNullException(nameof(right));
    }

    #endregion

    #region Methods

    /// <summary>
    /// Creates a deep copy of this concatenation pattern.
    /// </summary>
    /// <returns>
    /// A new ConcatenatePattern with cloned left and right patterns.
    /// The new pattern is completely independent of the original.
    /// </returns>
    /// <exception cref="ApplicationException">
    /// Thrown if left or right pattern is null, which should never happen
    /// if the pattern was constructed properly.
    /// </exception>
    /// <remarks>
    /// <para>
    /// Clone() performs a deep copy by recursively cloning both child patterns.
    /// This ensures that modifications to the cloned pattern tree do not affect
    /// the original pattern.
    /// </para>
    /// <para>
    /// The AST cache is NOT copied - each cloned pattern builds its own AST
    /// on first use. This is by design as the AST is specific to each pattern instance.
    /// </para>
    /// <para>
    /// The null check and exception are defensive programming - under normal circumstances,
    /// Left and Right should never be null after construction.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Clone a concatenation pattern
    /// var original = new ConcatenatePattern(
    ///     new LiteralPattern("test"),
    ///     new LiteralPattern("123")
    /// );
    /// var cloned = original.Clone();
    /// 
    /// // The cloned pattern is independent
    /// // Modifying cloned does not affect original
    /// </code>
    /// </example>
    internal override Pattern Clone()
    {
        if (Left == null || Right == null)
            throw new ApplicationException("Pattern.Clone");

        return new ConcatenatePattern(Left, Right);
    }

    #endregion

    #region Debugging

    /// <summary>
    /// Returns a debug string representation of this concatenation.
    /// </summary>
    /// <returns>A string showing left &amp; right pattern using ampersand notation.</returns>
    /// <remarks>
    /// <para>
    /// This method is used by the DebuggerDisplay attribute to show a concise
    /// representation in the Visual Studio debugger. The ampersand (&amp;) is used
    /// as a visual indicator of concatenation.
    /// </para>
    /// <para>
    /// This format is more compact than GetDescription() and is specifically
    /// designed for debugger views where space is limited.
    /// </para>
    /// </remarks>
    public override string DebugString()
    {
        return $"{Left?.DebugString()} & {Right?.DebugString()}";
    }

    #endregion
}