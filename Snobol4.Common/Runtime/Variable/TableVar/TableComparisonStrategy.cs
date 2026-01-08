using System.Globalization;

namespace Snobol4.Common;

/// <summary>
/// Comparison strategy for table variables
/// Tables compare by creation time and data type
/// </summary>
public class TableComparisonStrategy : IComparisonStrategy
{
    public int CompareTo(Var self, Var other)
    {
        var tableSelf = (TableVar)self;

        // Tables of the same type compare by creation time
        if (other is TableVar)
        {
            return DateTime.Compare(tableSelf.CreationDateTime, other.CreationDateTime);
        }

        // Different types compare by type name
        return string.Compare(tableSelf.DataType(), other.DataType(), false, CultureInfo.InvariantCulture);
    }

    public bool Equals(Var self, Var other)
    {
        // Tables are only equal if they're the same instance
        return IsIdentical(self, other);
    }

    public bool IsIdentical(Var self, Var other)
    {
        // Tables are identical only if they have the same unique ID
        return other.UniqueId == self.UniqueId;
    }
}