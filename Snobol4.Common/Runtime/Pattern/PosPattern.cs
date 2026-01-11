namespace Snobol4.Common;

/// <summary>
/// Represents a pattern that succeeds only if the cursor is at a specific position.
/// In SNOBOL4, this is created using the POS() function.
/// </summary>
/// <remarks>
/// <para>
/// POS is an anchor pattern that checks the cursor position without consuming any input.
/// It succeeds if and only if the cursor is exactly at the specified position (0-based).
/// </para>
/// <para>
/// Position interpretation:
/// - POS(0): Beginning of string (before first character)
/// - POS(n): After n characters
/// - POS(length): End of string (after last character)
/// </para>
/// <para>
/// POS is commonly used for:
/// - Anchoring patterns to specific positions
/// - Verifying cursor position during complex matches
/// - Implementing position-dependent pattern logic
/// - Creating fully anchored patterns with POS(0) ... RPOS(0)
/// </para>
/// <para>
/// Unlike TAB which advances the cursor to a position, POS only checks the position
/// and fails if the cursor is not at the expected location.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// // Anchor pattern at beginning
/// subject = 'test'
/// subject pos(0) 'test'         // Succeeds, cursor starts at 0
///
/// // Fully anchored match (beginning and end)
/// subject = 'exact'
/// subject pos(0) 'exact' rpos(0) // Matches entire string exactly
///
/// // Position check in middle of pattern
/// subject = 'abcdef'
/// subject 'abc' pos(3) 'def'    // Succeeds, cursor at 3 after 'abc'
///
/// // Position verification
/// subject = 'test123'
/// subject span(&amp;lcase) pos(4) span(&amp;digit)
/// // Succeeds if letters end exactly at position 4
///
/// // Will fail if position doesn't match
/// subject = 'test'
/// subject pos(5) 'test'         // Fails, cursor not at position 5
/// </code>
/// </example>
internal class PosPattern : TerminalPattern
{
    #region Internal Members

    /// <summary>
    /// The required cursor position (0-based index from start of subject).
    /// </summary>
    internal int Position;

    #endregion

    #region Construction

    /// <summary>
    /// Creates a POS pattern that succeeds only at a specific position.
    /// </summary>
    /// <param name="position">The required 0-based position in the subject string</param>
    /// <remarks>
    /// Position 0 is the beginning of the string, position n is after n characters.
    /// Negative positions are invalid but may be passed by error recovery code.
    /// </remarks>
    internal PosPattern(int position)
    {
        Position = position;
    }

    #endregion

    #region Internal Methods

    /// <summary>
    /// Creates a deep copy of this POS pattern.
    /// </summary>
    /// <returns>A new PosPattern with the same position</returns>
    internal override Pattern Clone()
    {
        return new PosPattern(Position);
    }

    /// <summary>
    /// Checks if the cursor is at the required position.
    /// </summary>
    /// <param name="node">The AST node index for this pattern</param>
    /// <param name="scan">The scanner containing the cursor position</param>
    /// <returns>
    /// Success if cursor is exactly at the required position (cursor unchanged),
    /// Failure if cursor is at any other position
    /// </returns>
    /// <remarks>
    /// <para>
    /// This is a pure position check - no input is consumed regardless of success or failure.
    /// The cursor position remains unchanged.
    /// </para>
    /// <para>
    /// POS is useful for creating anchored matches and verifying positions during
    /// complex pattern matching operations.
    /// </para>
    /// </remarks>
    internal override MatchResult Scan(int node, Scanner scan)
    {
        return Position == scan.CursorPosition
            ? MatchResult.Success(scan)
            : MatchResult.Failure(scan);
    }

    #endregion

    #region Debugging

    /// <summary>
    /// Returns a debug string representation of this alternation
    /// </summary>
    /// <returns>A string showing this pattern</returns>
    public override string DebugString() => $"pos({Position})";

    #endregion
}