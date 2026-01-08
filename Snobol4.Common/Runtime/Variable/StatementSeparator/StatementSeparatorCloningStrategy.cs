namespace Snobol4.Common;

/// <summary>
/// Cloning strategy for statement separator
/// Creates a new statement separator instance
/// </summary>
public class StatementSeparatorCloningStrategy : ICloningStrategy
{
    public Var Clone(Var self)
    {
        // Create a new statement separator
        return new StatementSeparator();
    }
}