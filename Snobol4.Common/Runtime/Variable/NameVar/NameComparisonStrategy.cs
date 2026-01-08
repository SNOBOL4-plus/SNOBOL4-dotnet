using System.Globalization;

namespace Snobol4.Common;

/// <summary>
/// Comparison strategy for name variables
/// Names compare by dereferencing to the target variable
/// </summary>
public class NameComparisonStrategy : IComparisonStrategy
{
    public int CompareTo(Var self, Var other)
    {
        var nameSelf = (NameVar)self;

        // Names compare by their pointer string value
        if (other is NameVar nameOther)
        {
            return string.Compare(nameSelf.Pointer, nameOther.Pointer, false, CultureInfo.InvariantCulture);
        }

        // Different types compare by type name
        return string.Compare(nameSelf.DataType(), other.DataType(), false, CultureInfo.InvariantCulture);
    }

    public bool Equals(Var self, Var other)
    {
        if (other is not NameVar nameOther)
            return false;

        var nameSelf = (NameVar)self;

        // Names are equal if they point to the same thing
        return nameSelf.Pointer == nameOther.Pointer
               && Equals(nameSelf.Key, nameOther.Key)
               && nameSelf.Collection == nameOther.Collection;
    }

    public bool IsIdentical(Var self, Var other)
    {
        // From original: always returns true
        // This is intentional - names are considered identical based on what they reference
        return true;
    }
}