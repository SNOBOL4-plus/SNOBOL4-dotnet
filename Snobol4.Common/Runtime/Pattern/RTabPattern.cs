using System.Diagnostics;

namespace Snobol4.Common;

/// <summary>
/// Represents a pattern that advances the cursor to a position relative to the end of the subject.
/// In SNOBOL4, this is created using the RTAB() function.
/// </summary>
/// <remarks>
/// <para>
/// RTAB is similar to TAB but counts from the end of the subject string instead of the beginning.
/// RTAB(N) positions the cursor N characters from the end of the subject.
/// </para>
/// <para>
/// RTAB can only move forward (toward the end). If the cursor is already past the target
/// position, the pattern fails.
/// </para>
/// <para>
/// The pattern fails if:
/// - The cursor is already beyond the target position
/// - The position parameter is negative or exceeds the subject length
/// </para>
/// </remarks>
/// <example>
/// <code>
/// // Position at 5 characters from end
/// subject = 'Hello World'
/// pattern = rtab(5) . before      // before = "Hello ", cursor at 'W'
///
/// // Extract last N characters
/// subject = 'filename.txt'
/// pattern = rtab(4) rem . ext     // ext = ".txt"
///
/// // Combine with RPOS for end anchoring
/// subject = 'test.doc'
/// pattern = rtab(4) '.doc' rpos(0) // Matches '.doc' at end
///
/// // Skip to near end
/// subject = 'long string here'
/// pattern = rtab(4)               // Positions 4 from end
///           rem . last            // last = "here"
/// </code>
/// </example>
[DebuggerDisplay("{DebugString()}")]
internal class RTabPattern : TerminalPattern
{
    #region Members

    internal int Position;

    #endregion

    #region Construction

    /// <summary>
    /// Creates an RTAB pattern that positions relative to the end
    /// </summary>
    /// <param name="position">Number of characters from the end of the subject</param>
    internal RTabPattern(int position)
    {
        Position = position;
    }

    #endregion

    #region Internal Methods

    /// <summary>
    /// Creates a deep copy of this RTAB pattern
    /// </summary>
    /// <returns>A new RTabPattern with the same position</returns>
    internal override Pattern Clone()
    {
        return new RTabPattern(Position);
    }

    /// <summary>
    /// Advances the cursor to N characters from the end of the subject
    /// </summary>
    /// <param name="node">The AST node index for this pattern</param>
    /// <param name="scan">The scanner containing the subject string and cursor state</param>
    /// <returns>
    /// Success if cursor can advance to the target position,
    /// Failure if cursor is already past the target or position is invalid
    /// </returns>
    internal override MatchResult Scan(int node, Scanner scan)
    {
        // Calculate target position from end
        var targetPosition = scan.Subject.Length - Position;

        // Fail if cursor is already past target or position is invalid
        if (scan.CursorPosition > targetPosition || targetPosition < 0)
            return MatchResult.Failure(scan);

        scan.CursorPosition = targetPosition;
        return MatchResult.Success(scan);
    }

    #endregion

    #region Debugging

    /// <summary>
    /// Returns a debug string representation of this pattern for diagnostic purposes.
    /// </summary>
    /// <returns>A string in the format "rtab(&lt;n&gt;)" where &lt;n&gt; is the target position from the end.</returns>
    /// <remarks>
    /// This method is used by the debugger display attribute and diagnostic tools
    /// to provide a concise, human-readable representation of the pattern.
    /// </remarks>
    public override string DebugString() => $"rtab({Position})";

    #endregion
}