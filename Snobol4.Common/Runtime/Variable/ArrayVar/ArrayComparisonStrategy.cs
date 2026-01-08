using System.Globalization;

namespace Snobol4.Common;

/// <summary>
/// Comparison strategy for array variables
/// Arrays compare by creation time and data type
/// </summary>
public class ArrayComparisonStrategy : IComparisonStrategy
{
    public int CompareTo(Var self, Var other)
    {
        var arraySelf = (ArrayVar)self;

        // Arrays of the same type compare by creation time
        if (other is ArrayVar)
        {
            return DateTime.Compare(arraySelf.CreationDateTime, other.CreationDateTime);
        }

        // Different types compare by type name
        return string.Compare(arraySelf.DataType(), other.DataType(), false, CultureInfo.InvariantCulture);
    }

    public bool Equals(Var self, Var other)
    {
        // Arrays are only equal if they're the same instance
        return IsIdentical(self, other);
    }

    public bool IsIdentical(Var self, Var other)
    {
        // Arrays are identical only if they have the same unique ID
        return other.UniqueId == self.UniqueId;
    }
}