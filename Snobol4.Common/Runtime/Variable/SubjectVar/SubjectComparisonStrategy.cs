using System.Globalization;

namespace Snobol4.Common;

/// <summary>
/// Comparison strategy for subject variables
/// Subject variables compare by their string content
/// </summary>
public class SubjectComparisonStrategy : IComparisonStrategy
{
    public int CompareTo(Var self, Var other)
    {
        var subjectSelf = (SubjectVar)self;

        if (other is SubjectVar subjectOther)
        {
            return string.Compare(subjectSelf.Subject, subjectOther.Subject, false, CultureInfo.InvariantCulture);
        }

        if (other is StringVar stringOther)
        {
            return string.Compare(subjectSelf.Subject, stringOther.Data, false, CultureInfo.InvariantCulture);
        }

        // Different types compare by type name
        return string.Compare(subjectSelf.DataType(), other.DataType(), false, CultureInfo.InvariantCulture);
    }

    public bool Equals(Var self, Var other)
    {
        var subjectSelf = (SubjectVar)self;

        return other switch
        {
            SubjectVar subjectOther => subjectSelf.Subject == subjectOther.Subject,
            StringVar stringOther => subjectSelf.Subject == stringOther.Data,
            _ => false
        };
    }

    public bool IsIdentical(Var self, Var other)
    {
        // Subject variables are identical only if they have the same unique ID
        return other.UniqueId == self.UniqueId;
    }
}