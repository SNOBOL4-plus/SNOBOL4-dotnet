using System.Diagnostics;

namespace Snobol4.Common;

/// <summary>
/// Represents a pattern that succeeds only if the cursor is at a specific position from the end.
/// In SNOBOL4, this is created using the RPOS() function.
/// </summary>
/// <remarks>
/// <para>
/// RPOS (Reverse Position) is an anchor pattern that checks the cursor position relative
/// to the end of the subject string without consuming any input. It succeeds if and only
/// if the cursor is exactly n characters from the end.
/// </para>
/// <para>
/// Position interpretation:
/// - RPOS(0): End of string (after last character)
/// - RPOS(n): n characters before the end
/// - RPOS(length): Beginning of string (before first character)
/// </para>
/// <para>
/// RPOS is commonly used for:
/// - Anchoring patterns to the end of the string
/// - Verifying end-relative positions during matching
/// - Creating fully anchored patterns with POS(0) ... RPOS(0)
/// - Matching file extensions, suffixes, or endings
/// </para>
/// <para>
/// The actual cursor position checked is: (subject.Length - Position)
/// For example, in a string of length 10, RPOS(3) checks if cursor is at position 7.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// // Anchor pattern at end
/// subject = 'test'
/// subject arb rpos(0)           // Succeeds at end of string
///
/// // Fully anchored match
/// subject = 'exact'
/// subject pos(0) 'exact' rpos(0) // Matches entire string exactly
///
/// // Match file extension
/// subject = 'file.txt'
/// subject arb '.txt' rpos(0)    // Succeeds, '.txt' at end
///
/// // Position check from end
/// subject = 'test123'
/// subject span(&amp;lcase) rpos(3) span(&amp;digit)
/// // Succeeds if digits start exactly 3 chars from end
///
/// // Will fail if position doesn't match
/// subject = 'test'
/// subject 'test' rpos(5)        // Fails, string length is 4
/// </code>
/// </example>
[DebuggerDisplay("{DebugPattern()}")]
internal class RPosPattern : TerminalPattern
{
    #region Members

    /// <summary>
    /// The required number of characters from the end of the subject.
    /// </summary>
    internal int Position;

    #endregion

    #region Construction

    /// <summary>
    /// Creates an RPOS pattern that succeeds only at a specific position from the end.
    /// </summary>
    /// <param name="position">The required position from the end (0 = at end, n = n chars before end)</param>
    /// <remarks>
    /// Position 0 is at the end of the string. Position n is n characters before the end.
    /// Negative positions are invalid but may be passed by error recovery code.
    /// </remarks>
    internal RPosPattern(int position)
    {
        Position = position;
    }

    #endregion

    #region Internal Methods

    /// <summary>
    /// Creates a deep copy of this RPOS pattern.
    /// </summary>
    /// <returns>A new RPosPattern with the same position from end</returns>
    internal override Pattern Clone()
    {
        return new RPosPattern(Position);
    }

    /// <summary>
    /// Checks if the cursor is at the required position from the end.
    /// </summary>
    /// <param name="node">The AST node index for this pattern</param>
    /// <param name="scan">The scanner containing the cursor position and subject string</param>
    /// <returns>
    /// Success if cursor is exactly at (subject.Length - Position) (cursor unchanged),
    /// Failure if cursor is at any other position
    /// </returns>
    /// <remarks>
    /// <para>
    /// This is a pure position check - no input is consumed regardless of success or failure.
    /// The cursor position remains unchanged.
    /// </para>
    /// <para>
    /// The calculation is: required_position = subject.Length - Position
    /// For example, in "test" (length 4):
    /// - RPOS(0) checks position 4 (at end)
    /// - RPOS(1) checks position 3 (before last char)
    /// - RPOS(4) checks position 0 (at beginning)
    /// </para>
    /// </remarks>
    internal override MatchResult Scan(int node, Scanner scan)
    {
        return Position == scan.Subject.Length - scan.CursorPosition
            ? MatchResult.Success(scan)
            : MatchResult.Failure(scan);
    }

    #endregion

    #region Debugging

    /// <summary>
    /// Returns a debug string representation of this pattern for diagnostic purposes.
    /// </summary>
    /// <returns>A string in the format "rpos(&lt;n&gt;)" where &lt;n&gt; is the required position from the end.</returns>
    /// <remarks>
    /// This method is used by the debugger display attribute and diagnostic tools
    /// to provide a concise, human-readable representation of the pattern.
    /// </remarks>
    public override string DebugPattern() => "rpos";

    #endregion
}
