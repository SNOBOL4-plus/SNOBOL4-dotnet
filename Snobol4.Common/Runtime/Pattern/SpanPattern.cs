using System.Buffers;
using System.Diagnostics;

namespace Snobol4.Common;

/// <summary>
/// Represents a pattern that matches one or more consecutive characters from a specified set.
/// In SNOBOL4, this is created using the SPAN() function.
/// </summary>
/// <remarks>
/// <para>
/// SPAN matches a sequence of one or more characters, all of which must be in the
/// specified character set. It continues matching as long as consecutive characters
/// are in the set, and fails if the first character is not in the set.
/// </para>
/// <para>
/// SPAN is greedy - it matches as many consecutive characters as possible from the set.
/// Unlike BREAK which stops before finding a character, SPAN continues while characters
/// are IN the set.
/// </para>
/// <para>
/// The pattern fails if:
/// - The first character at the cursor is not in the character set
/// - The cursor is at the end of the subject string
/// </para>
/// <para>
/// SPAN is commonly used for:
/// - Matching sequences of digits, letters, or other character classes
/// - Tokenization (matching identifiers, numbers, etc.)
/// - Consuming whitespace
/// - Extracting runs of similar characters
/// </para>
/// <para>
/// Key difference from ANY: ANY matches exactly one character, SPAN matches one or more.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// // Match a sequence of letters
/// letters = 'abcdefghijklmnopqrstuvwxyz'
/// word = span(letters)
/// subject = 'one,,, two'
/// subject word . w1               // w1 = "one"
///
/// // Match digits
/// digits = '0123456789'
/// number = span(digits)
/// subject = '12345abc'
/// subject number . n              // n = "12345"
///
/// // Match whitespace
/// spaces = ' \t'
/// gap = span(spaces)
/// subject = '   hello'
/// subject gap                     // Matches "   "
///
/// // Common pattern: signed integer
/// digits = '0123456789'
/// integer = (any('+-') | '') span(digits)
/// subject = '-43'
/// subject integer . num           // num = "-43"
///
/// // Fails if first character not in set
/// subject = '123abc'
/// subject span('abc') . letters   // Fails, '1' not in 'abc'
///
/// // Greedy matching
/// subject = 'aaabbb'
/// subject span('ab') . all        // all = "aaabbb", matches all chars
/// </code>
/// </example>
[DebuggerDisplay("{DebugString()}")]
internal class SpanPattern : TerminalPattern
{
    #region Members

    /// <summary>
    /// The set of characters to span (match consecutively).
    /// </summary>
    private readonly string _charList;

    /// <summary>
    /// Optimized character search values using hardware acceleration when available.
    /// SearchValues provides vectorized character matching for significantly improved performance.
    /// Nullable for small character set optimization.
    /// </summary>
    private readonly SearchValues<char>? _searchValues;

    /// <summary>
    /// Threshold for using SearchValues. For very small character sets (1-3 chars),
    /// direct comparison is faster than SearchValues overhead.
    /// </summary>
    private const int SearchValuesThreshold = 4;

    #endregion

    #region Constructors

    /// <summary>
    /// Creates a SPAN pattern that matches consecutive characters from a set.
    /// </summary>
    /// <param name="charList">String containing characters to span. Cannot be null or empty.</param>
    /// <remarks>
    /// The character list cannot be empty because SPAN must match at least one character,
    /// and there would be no valid characters to match.
    /// Creates a SearchValues instance for larger sets, or uses direct comparison for small sets.
    /// </remarks>
    internal SpanPattern(string charList)
    {
        _charList = charList;
        // Create SearchValues only for larger character sets
        _searchValues = charList.Length >= SearchValuesThreshold
            ? SearchValues.Create(charList)
            : null;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Creates a deep copy of this SPAN pattern.
    /// </summary>
    /// <returns>A new SpanPattern with the same character set</returns>
    internal override Pattern Clone()
    {
        return new SpanPattern(_charList);
    }

    /// <summary>
    /// Matches one or more consecutive characters from the character set.
    /// </summary>
    /// <param name="node">The AST node index for this pattern</param>
    /// <param name="scan">The scanner containing the subject string and cursor position</param>
    /// <returns>
    /// Success if at least one character matched and cursor advanced,
    /// Failure if no characters matched or at end of subject
    /// </returns>
    /// <remarks>
    /// <para>
    /// SPAN is greedy - it matches as many consecutive characters from the set as possible.
    /// The matching stops when:
    /// - A character not in the set is encountered
    /// - The end of the subject string is reached
    /// </para>
    /// <para>
    /// The pattern fails if the very first character is not in the set (or if at end of subject).
    /// This is the key difference between SPAN (one or more) and ARBNO (zero or more).
    /// </para>
    /// <para>
    /// After successful match, the cursor is positioned after the last matched character
    /// (at the first character NOT in the set, or at end of subject).
    /// </para>
    /// <para>
    /// Optimizations:
    /// - Fast path for single-character sets (common case like span('x'))
    /// - Direct comparison for small sets (2-3 chars) to avoid SearchValues overhead
    /// - Hardware-accelerated SearchValues for larger character sets
    /// - Uses IndexOfAnyExcept for efficient boundary detection
    /// </para>
    /// </remarks>
    internal override MatchResult Scan(int node, Scanner scan)
    {
        // Early exit if at end of subject
        if (scan.CursorPosition >= scan.Subject.Length)
            return MatchResult.Failure(scan);

        var startPosition = scan.CursorPosition;
        var subject = scan.Subject.AsSpan(scan.CursorPosition);

        // Fast path for single-character sets (common case: span('x'), span(','), span(' '))
        // This optimization eliminates SearchValues overhead and array operations
        if (_charList.Length == 1)
        {
            var targetChar = _charList[0];
            var matched = 0;

            // Simple loop is faster than IndexOf for single character matching
            while (matched < subject.Length && subject[matched] == targetChar)
                matched++;

            // Must match at least one character
            if (matched == 0)
                return MatchResult.Failure(scan);

            scan.CursorPosition += matched;
            return MatchResult.Success(scan);
        }

        int endIndex;

        // Optimize for small character sets (2-3 chars) with direct comparison
        if (_searchValues == null)
        {
            endIndex = 0;

            // Manual loop with direct character comparison
            // Faster than SearchValues for very small sets
            while (endIndex < subject.Length)
            {
                var ch = subject[endIndex];
                bool isInSet = _charList.Length switch
                {
                    2 => ch == _charList[0] || ch == _charList[1],
                    3 => ch == _charList[0] || ch == _charList[1] || ch == _charList[2],
                    _ => false // Should not happen (length < SearchValuesThreshold)
                };

                if (!isInSet)
                    break;

                endIndex++;
            }

            // Must match at least one character
            if (endIndex == 0)
                return MatchResult.Failure(scan);

            // Update cursor position
            scan.CursorPosition = endIndex == subject.Length
                ? scan.Subject.Length  // Matched to end
                : startPosition + endIndex;  // Matched until non-matching char
        }
        else
        {
            // Use SearchValues for optimized character set matching (larger sets)
            // IndexOfAnyExcept finds the first character NOT in the set
            endIndex = subject.IndexOfAnyExcept(_searchValues);

            // If endIndex is 0, first character is not in set - fail immediately
            if (endIndex == 0)
                return MatchResult.Failure(scan);

            // Update cursor position
            if (endIndex < 0)
            {
                // All remaining characters are in the set - match to end
                scan.CursorPosition = scan.Subject.Length;
            }
            else
            {
                // Match up to the first character not in the set
                scan.CursorPosition = startPosition + endIndex;
            }
        }

        return MatchResult.Success(scan);
    }

    #endregion

    #region Debugging

    /// <summary>
    /// Returns a debug string representation of this pattern for diagnostic purposes.
    /// </summary>
    /// <returns>A string in the format "span(&lt;characters&gt;)" where &lt;characters&gt; is the character set to span.</returns>
    /// <remarks>
    /// This method is used by the debugger display attribute and diagnostic tools
    /// to provide a concise, human-readable representation of the pattern.
    /// </remarks>
    public override string DebugString() => $"span({_charList})";

    #endregion
}