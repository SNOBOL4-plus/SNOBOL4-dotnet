using System.Globalization;

namespace Snobol4.Common;

/// <summary>
/// Comparison strategy for code variables
/// Code compares by creation time and data type
/// </summary>
public class CodeComparisonStrategy : IComparisonStrategy
{
    public int CompareTo(Var self, Var other)
    {
        var codeSelf = (CodeVar)self;

        // Code of the same type compares by creation time
        if (other is CodeVar)
        {
            return DateTime.Compare(codeSelf.CreationDateTime, other.CreationDateTime);
        }

        // Different types compare by type name
        return string.Compare(codeSelf.DataType(), other.DataType(), false, CultureInfo.InvariantCulture);
    }

    public bool Equals(Var self, Var other)
    {
        // Code is only equal if it's the same instance
        return IsIdentical(self, other);
    }

    public bool IsIdentical(Var self, Var other)
    {
        // Code is identical only if they have the same unique ID
        return other.UniqueId == self.UniqueId;
    }
}