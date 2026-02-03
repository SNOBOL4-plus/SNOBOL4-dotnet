namespace Snobol4.Common;

/// <summary>
/// Base class for all SNOBOL4 pattern types.
/// Patterns are the fundamental building blocks of SNOBOL4's powerful string matching capabilities.
/// </summary>
/// <remarks>
/// <para>
/// In SNOBOL4, patterns are first-class objects that can be assigned to variables, passed as arguments,
/// and composed to create complex matching expressions. This base class provides the foundation for
/// both terminal patterns (that perform actual matching) and composite patterns (that combine other patterns).
/// </para>
/// <para>
/// The pattern hierarchy consists of:
/// - <b>Terminal Patterns</b>: Perform actual matching (LiteralPattern, AnyPattern, LenPattern, etc.)
/// - <b>Composite Patterns</b>: Combine other patterns (ConcatenatePattern, AlternatePattern)
/// </para>
/// <para>
/// Patterns are immutable after construction and use an Abstract Syntax Tree (AST) for efficient
/// matching with backtracking support. The AST is lazily built on first use and cached for
/// subsequent matches.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// // Simple literal pattern
/// pattern = new LiteralPattern("hello");
///
/// // Composite pattern (concatenation)
/// pattern = new ConcatenatePattern(
///     new LiteralPattern("hello"),
///     new LiteralPattern("world")
/// );
///
/// // Pattern with alternation
/// pattern = new AlternatePattern(
///     new LiteralPattern("cat"),
///     new LiteralPattern("dog")
/// );
/// </code>
/// </example>
public abstract class Pattern
{
    /// <summary>
    /// The left child pattern in a composite pattern structure.
    /// Null for terminal patterns that have no children.
    /// </summary>
    /// <remarks>
    /// In composite patterns like ConcatenatePattern and AlternatePattern,
    /// this represents the first pattern to be evaluated.
    /// </remarks>
    internal Pattern? LeftPattern = null;

    /// <summary>
    /// The right child pattern in a composite pattern structure.
    /// Null for terminal patterns that have no children.
    /// </summary>
    /// <remarks>
    /// In composite patterns like ConcatenatePattern and AlternatePattern,
    /// this represents the second pattern to be evaluated.
    /// </remarks>
    internal Pattern? RightPattern = null;

    /// <summary>
    /// Cached Abstract Syntax Tree nodes for this pattern.
    /// Built lazily on first match attempt and reused for subsequent matches.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The AST is a flattened representation of the pattern tree that includes
    /// precomputed information about subsequent patterns and alternate paths for backtracking.
    /// This structure enables efficient pattern matching with minimal overhead.
    /// </para>
    /// <para>
    /// An empty list indicates the AST has not yet been built.
    /// </para>
    /// </remarks>
    internal List<AbstractSyntaxTreeNode> Ast = [];

    /// <summary>
    /// The starting node in the cached AST for this pattern.
    /// Null until the AST is built during the first match attempt.
    /// </summary>
    /// <remarks>
    /// The start node is always a terminal pattern node - the leftmost leaf
    /// in the pattern tree where matching begins.
    /// </remarks>
    internal AbstractSyntaxTreeNode? StartNode;

    /// <summary>
    /// Determines whether this pattern is a terminal (leaf) node in the pattern tree.
    /// </summary>
    /// <returns>
    /// <c>true</c> if this is a terminal pattern that performs actual matching;
    /// <c>false</c> if this is a composite pattern that combines other patterns.
    /// </returns>
    /// <remarks>
    /// <para>
    /// Terminal patterns include:
    /// - LiteralPattern, AnyPattern, NotAnyPattern
    /// - SpanPattern, BreakPattern, LenPattern
    /// - ArbPattern, ArbNoPattern, BalPattern
    /// - FailPattern, AbortPattern, SucceedPattern, NullPattern, RemPattern
    /// - PosPattern, RPosPattern, TabPattern, RTabPattern
    /// </para>
    /// <para>
    /// Composite patterns include:
    /// - ConcatenatePattern (sequential matching)
    /// - AlternatePattern (choice with backtracking)
    /// </para>
    /// <para>
    /// This method is used during AST construction to identify leaf nodes
    /// where actual matching logic resides.
    /// </para>
    /// </remarks>
    internal virtual bool IsTerminal()
    {
        return true;
    }

    /// <summary>
    /// Creates a deep copy of this pattern, including all child patterns.
    /// </summary>
    /// <returns>
    /// A new pattern instance with the same structure and behavior as the original.
    /// The clone is completely independent and can be modified without affecting the original.
    /// </returns>
    /// <remarks>
    /// <para>
    /// Cloning is essential for pattern reuse because:
    /// 1. Patterns cache their AST after the first match
    /// 2. The same pattern structure may need different AST instances for concurrent matching
    /// 3. Pattern modification operations need to preserve the original
    /// </para>
    /// <para>
    /// For composite patterns, Clone() recursively clones all child patterns.
    /// For terminal patterns, Clone() creates a new instance with the same parameters.
    /// </para>
    /// <para>
    /// The AST cache is NOT cloned - each cloned pattern builds its own AST on first use.
    /// This is intentional as the AST is specific to each pattern instance.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Clone a pattern for reuse
    /// var original = new LiteralPattern("test");
    /// var copy = original.Clone();
    /// 
    /// // Clone a composite pattern
    /// var pattern = new ConcatenatePattern(
    ///     new LiteralPattern("hello"),
    ///     new LiteralPattern("world")
    /// );
    /// var cloned = pattern.Clone(); // Recursively clones both children
    /// </code>
    /// </example>
    internal abstract Pattern Clone();

    #region Debugging

    /// <summary>
    /// Returns a debug string representation of this pattern
    /// </summary>
    /// <returns>A string showing this pattern</returns>
    public abstract string DebugPattern();

    #endregion
}