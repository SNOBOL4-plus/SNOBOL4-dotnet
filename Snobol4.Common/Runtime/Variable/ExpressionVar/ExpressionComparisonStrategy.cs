using System.Globalization;

namespace Snobol4.Common;

/// <summary>
/// Comparison strategy for expression variables
/// Expressions compare by creation time and data type
/// </summary>
public class ExpressionComparisonStrategy : IComparisonStrategy
{
    public int CompareTo(Var self, Var other)
    {
        var expressionSelf = (ExpressionVar)self;

        // Expressions of the same type compare by creation time
        if (other is ExpressionVar)
        {
            return DateTime.Compare(expressionSelf.CreationDateTime, other.CreationDateTime);
        }

        // Different types compare by type name
        return string.Compare(expressionSelf.DataType(), other.DataType(), false, CultureInfo.InvariantCulture);
    }

    public bool Equals(Var self, Var other)
    {
        // Expressions are only equal if they're the same instance
        return IsIdentical(self, other);
    }

    public bool IsIdentical(Var self, Var other)
    {
        // Expressions are identical only if they have the same unique ID
        return other.UniqueId == self.UniqueId;
    }
}