using System.Globalization;

namespace Snobol4.Common;

/// <summary>
/// Comparison strategy for program-defined data variables
/// User-defined data types compare by creation time
/// </summary>
public class ProgramDefinedDataComparisonStrategy : IComparisonStrategy
{
    public int CompareTo(Var self, Var other)
    {
        var dataSelf = (ProgramDefinedDataVar)self;

        // Compare by data type name first
        if (other is ProgramDefinedDataVar dataOther)
        {
            var typeComparison = string.Compare(
                dataSelf.UserDefinedDataName,
                dataOther.UserDefinedDataName,
                false,
                CultureInfo.InvariantCulture);

            // If same type, compare by creation time
            return typeComparison != 0
                ? typeComparison
                : DateTime.Compare(dataSelf.CreationDateTime, other.CreationDateTime);
        }

        // Different base types compare by type name
        return string.Compare(dataSelf.DataType(), other.DataType(), false, CultureInfo.InvariantCulture);
    }

    public bool Equals(Var self, Var other)
    {
        // User-defined data is only equal if it's the same instance
        return IsIdentical(self, other);
    }

    public bool IsIdentical(Var self, Var other)
    {
        // User-defined data is identical only if they have the same unique ID
        return other.UniqueId == self.UniqueId;
    }
}