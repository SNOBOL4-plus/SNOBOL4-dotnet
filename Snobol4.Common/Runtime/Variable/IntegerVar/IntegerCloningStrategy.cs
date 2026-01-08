namespace Snobol4.Common;

/// <summary>
/// Cloning strategy for integer variables
/// </summary>
public class IntegerCloningStrategy : ICloningStrategy
{
    public Var Clone(Var self)
    {
        var intSelf = (IntegerVar)self;
        return new IntegerVar(intSelf.Data);
    }
}