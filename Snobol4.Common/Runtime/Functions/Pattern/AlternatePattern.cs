using System.Diagnostics;

namespace Snobol4.Common;

/// <summary>
/// Class to encapsulate alternate patterns
/// </summary>
[DebuggerDisplay("{DebugString()}")]
internal class AlternatePattern : NonTerminalPattern
{
    #region Construction

    internal AlternatePattern(Pattern left, Pattern right)
    {
        if (left == null || right == null)
            throw new ApplicationException("AlternatePattern");

        Left = left;
        Right = right;
    }

    #endregion region

    #region Methods

    internal override Pattern Clone()
    {
        if (Left == null || Right == null)
            throw new ApplicationException("Patten.Clone");

        return new AlternatePattern(Left, Right);
    }

    #endregion Methods

    #region Debugging

    internal string DebugString()
    {
        return $"{Left} | {Right}";
    }

    #endregion'
}