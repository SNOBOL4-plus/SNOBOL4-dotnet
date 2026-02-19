namespace Snobol4.Common;

public sealed class CodeComparisonStrategy : IComparisonStrategy
{

    public int CompareTo(Var self, Var other)
    {
        var codeSelf = (CodeVar)self;

        // Code of the same type compares by creation time
        if (other is CodeVar)
        {
            return codeSelf.SequenceId.CompareTo(other.SequenceId);
        }

        // Different types compare by type name
        return string.Compare(codeSelf.DataType(), other.DataType(), StringComparison.InvariantCulture);
    }


    public bool Equals(Var self, Var other)
    {
        // Code is only equal if it's the same instance
        return self.SequenceId == other.SequenceId;
    }


    public bool IsIdentical(Var self, Var other)
    {
        // Code is identical only if they have the same unique ID
        return self.SequenceId == other.SequenceId;
    }
}