    namespace Snobol4.Common;

/// <summary>
/// Represents a pattern that matches exactly N characters.
/// In SNOBOL4, this is created using the LEN() function.
/// </summary>
/// <remarks>
/// <para>
/// LEN matches a fixed number of characters regardless of their content. It simply
/// advances the cursor by the specified length. This is useful for matching fixed-width
/// fields or skipping a known number of characters.
/// </para>
/// <para>
/// LEN(0) is valid and matches zero characters (similar to NULL), always succeeding
/// without advancing the cursor.
/// </para>
/// <para>
/// The pattern fails if:
/// - The requested length would exceed the end of the subject string
/// </para>
/// </remarks>
/// <example>
/// <code>
/// // Match exactly 3 characters
/// subject = 'ABCDA'
/// pattern = len(3) . matched      // matched = "ABC"
///
/// // Fixed-width field parsing
/// subject = 'John    Smith   42'
/// pattern = len(8) . firstname    // firstname = "John    "
///           len(8) . lastname     // lastname = "Smith   "
///           len(2) . age          // age = "42"
///
/// // LEN(0) matches zero characters
/// subject = 'ABCDA'
/// pattern = 'ABCD' len(0) 'A'     // Succeeds, LEN(0) is like NULL
///
/// // Skip N characters
/// subject = 'prefixDATA'
/// pattern = len(6) rem . data     // data = "DATA"
/// </code>
/// </example>
internal class LenPattern : TerminalPattern
{
    #region Members

    private readonly int _length;

    #endregion

    #region Constructors

    /// <summary>
    /// Creates a LEN pattern that matches exactly N characters
    /// </summary>
    /// <param name="length">Number of characters to match (must be non-negative)</param>
    internal LenPattern(int length)
    {
        _length = length;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Creates a deep copy of this LEN pattern
    /// </summary>
    /// <returns>A new LenPattern with the same length</returns>
    internal override Pattern Clone()
    {
        return new LenPattern(_length);
    }

    /// <summary>
    /// Matches exactly the specified number of characters
    /// </summary>
    /// <param name="node">The AST node index for this pattern</param>
    /// <param name="scan">The scanner containing the subject string and cursor state</param>
    /// <returns>
    /// Success if N characters are available (cursor advances by N),
    /// Failure if N characters would exceed the subject length
    /// </returns>
    internal override MatchResult Scan(int node, Scanner scan)
    {
        scan.CursorPosition += _length;
        return scan.CursorPosition <= scan.Subject.Length 
            ? MatchResult.Success(scan) 
            : MatchResult.Failure(scan);
    }

    #endregion
}