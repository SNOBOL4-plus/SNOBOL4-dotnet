namespace Snobol4.Common;

/// <summary>
/// Strategy interface for comparison operations on variables
/// </summary>
public interface IComparisonStrategy
{
    /// <summary>
    /// Compare this variable with another for ordering
    /// </summary>
    int CompareTo(Var self, Var other);

    /// <summary>
    /// Check if this variable is equal to another
    /// </summary>
    bool Equals(Var self, Var other);

    /// <summary>
    /// Check if this variable is identical to another (same reference/value)
    /// </summary>
    bool IsIdentical(Var self, Var other);
}