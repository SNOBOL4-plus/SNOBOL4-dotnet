using System.Globalization;

namespace Snobol4.Common;

/// <summary>
/// Comparison strategy for pattern variables
/// Patterns compare by creation time and data type
/// </summary>
public class PatternComparisonStrategy : IComparisonStrategy
{
    public int CompareTo(Var self, Var other)
    {
        var patternSelf = (PatternVar)self;

        // Patterns of the same type compare by creation time
        if (other is PatternVar)
        {
            return DateTime.Compare(patternSelf.CreationDateTime, other.CreationDateTime);
        }

        // Different types compare by type name
        return string.Compare(patternSelf.DataType(), other.DataType(), false, CultureInfo.InvariantCulture);
    }

    public bool Equals(Var self, Var other)
    {
        // Patterns are only equal if they're the same instance
        return IsIdentical(self, other);
    }

    public bool IsIdentical(Var self, Var other)
    {
        // Patterns are identical only if they have the same unique ID
        // (Pattern structure equality would be complex and is not needed)
        return other.UniqueId == self.UniqueId;
    }
}