namespace Snobol4.Common;

/// <summary>
/// Cloning strategy for string variables
/// </summary>
public class StringCloningStrategy : ICloningStrategy
{
    public Var Clone(Var self)
    {
        var stringSelf = (StringVar)self;
        return new StringVar(stringSelf);
    }
}