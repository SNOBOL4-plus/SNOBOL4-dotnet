using System.Globalization;

namespace Snobol4.Common;

/// <summary>
/// Comparison strategy for array variables.
/// Arrays compare by creation time when same type, otherwise by type name.
/// Identity comparison uses unique ID.
/// </summary>
public class ArrayComparisonStrategy : IComparisonStrategy
{
    public int CompareTo(Var self, Var other)
    {
        if (other is null)
            return 1; // Non-null is always greater than null

        var arraySelf = (ArrayVar)self;

        // Arrays of the same type compare by creation time
        if (other is ArrayVar)
        {
            return DateTime.Compare(arraySelf.CreationDateTime, other.CreationDateTime);
        }

        // Different types compare by type name (lexicographically)
        return string.Compare(arraySelf.DataType(), other.DataType(), false, CultureInfo.InvariantCulture);
    }

    public bool Equals(Var self, Var other)
    {
        // Arrays are only equal if they're the same instance
        return IsIdentical(self, other);
    }

    public bool IsIdentical(Var self, Var other)
    {
        if (other is null)
            return false;

        // Arrays are identical only if they have the same unique ID
        return other.UniqueId == self.UniqueId;
    }
}