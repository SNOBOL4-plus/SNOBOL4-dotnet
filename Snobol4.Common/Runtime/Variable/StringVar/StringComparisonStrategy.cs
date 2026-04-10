using System.Globalization;

namespace Snobol4.Common;

public class StringComparisonStrategy : IComparisonStrategy
{

    public int CompareTo(Var self, Var other)
    {
        var stringSelf = (StringVar)self;

        if (other is StringVar stringOther)
        {
            // SPITBOL uses byte-order (ordinal) comparison for strings.
            // Normalize to -1/0/1 for consistent comparison results.
            var cmp = string.CompareOrdinal(stringSelf.Data, stringOther.Data);
            return cmp < 0 ? -1 : cmp > 0 ? 1 : 0;
        }

        // Strings sort after other types by type name comparison
        return string.Compare(stringSelf.DataType(), other.DataType(), StringComparison.Ordinal);
    }


    public bool Equals(Var self, Var other)
    {
        if (other is not StringVar stringOther)
            return false;

        var stringSelf = (StringVar)self;
        return stringSelf.Data == stringOther.Data;
    }


    public bool IsIdentical(Var self, Var other)
    {
        if (other is not StringVar stringOther)
            return false;

        var stringSelf = (StringVar)self;
        return stringSelf.Data == stringOther.Data;
    }
}