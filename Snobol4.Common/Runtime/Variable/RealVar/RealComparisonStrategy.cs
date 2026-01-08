using System.Globalization;

namespace Snobol4.Common;

/// <summary>
/// Comparison strategy for real (floating-point) variables
/// </summary>
public class RealComparisonStrategy : IComparisonStrategy
{
    public int CompareTo(Var self, Var other)
    {
        var realSelf = (RealVar)self;

        return other switch
        {
            IntegerVar intOther => realSelf.Data.CompareTo((double)intOther.Data),
            RealVar realOther => realSelf.Data.CompareTo(realOther.Data),
            _ => string.Compare(realSelf.DataType(), other.DataType(), false, CultureInfo.InvariantCulture)
        };
    }

    public bool Equals(Var self, Var other)
    {
        var realSelf = (RealVar)self;

        return other switch
        {
            IntegerVar intOther => Math.Abs(realSelf.Data - intOther.Data) < double.Epsilon,
            RealVar realOther => Math.Abs(realSelf.Data - realOther.Data) < double.Epsilon,
            _ => false
        };
    }

    public bool IsIdentical(Var self, Var other)
    {
        if (other is not RealVar realOther)
            return false;

        var realSelf = (RealVar)self;

        // For real numbers, use exact comparison (as in original code)
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        return realSelf.Data == realOther.Data;
    }
}