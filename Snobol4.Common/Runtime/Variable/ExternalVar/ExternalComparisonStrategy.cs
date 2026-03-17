namespace Snobol4.Common;

public class ExternalComparisonStrategy : IComparisonStrategy
{
    public int CompareTo(Var self, Var other)
    {
        if (other is ExternalVar otherExt)
        {
            var a = ((ExternalVar)self).Pointer;
            var b = otherExt.Pointer;
            return a.ToInt64().CompareTo(b.ToInt64());
        }
        return string.Compare("EXTERNAL", other.DataType(), StringComparison.Ordinal);
    }

    public bool Equals(Var self, Var other) =>
        other is ExternalVar otherExt && ((ExternalVar)self).Pointer == otherExt.Pointer;

    public bool IsIdentical(Var self, Var other) => Equals(self, other);
}
