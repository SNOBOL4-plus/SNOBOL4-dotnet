namespace Snobol4.Common;

/// <summary>
/// Arithmetic strategy for integer variables
/// Handles binary operations with overflow checking and type promotion
/// </summary>
public sealed class IntegerArithmeticStrategy : IArithmeticStrategy
{
    public Var Add(Var self, Var other, Executive executive)
    {
        var intSelf = (IntegerVar)self;
        // Use double dispatch to get type-specific behavior
        return other.AddInteger(intSelf, executive);
    }

    public Var Subtract(Var self, Var other, Executive executive)
    {
        var intSelf = (IntegerVar)self;
        return other.SubtractInteger(intSelf, executive);
    }

    public Var Multiply(Var self, Var other, Executive executive)
    {
        var intSelf = (IntegerVar)self;
        return other.MultiplyInteger(intSelf, executive);
    }

    public Var Divide(Var self, Var other, Executive executive)
    {
        var intSelf = (IntegerVar)self;
        return other.DivideInteger(intSelf, executive);
    }

    public Var Power(Var self, Var other, Executive executive)
    {
        var intSelf = (IntegerVar)self;

        if (other is not IntegerVar intOther)
        {
            // Convert to real for non-integer exponents
            return new RealVar(intSelf.Data).Power(other, executive);
        }

        if (intSelf.Data == 0 && intOther.Data <= 0)
        {
            executive.LogRuntimeException(18);
            return StringVar.Null();
        }

        try
        {
            checked
            {
                long result = 1;
                for (var i = 0; i < intOther.Data; ++i)
                {
                    result *= intSelf.Data;
                }
                return new IntegerVar(result);
            }
        }
        catch (OverflowException)
        {
            // Fall back to real arithmetic on overflow
            return new RealVar(Math.Pow(intSelf.Data, intOther.Data));
        }
    }

    public Var Negate(Var self, Executive executive)
    {
        var intSelf = (IntegerVar)self;

        try
        {
            checked
            {
                return new IntegerVar(-intSelf.Data);
            }
        }
        catch (OverflowException)
        {
            executive.LogRuntimeException(11);
            return StringVar.Null();
        }
    }
}