namespace Snobol4.Common;

/// <summary>
/// Arithmetic strategy for real (floating-point) variables
/// </summary>
public class RealArithmeticStrategy : IArithmeticStrategy
{
    public Var Add(Var self, Var other, Executive executive)
    {
        var realSelf = (RealVar)self;

        // Use double dispatch to get type-specific behavior
        return other.AddReal(realSelf, executive);
    }

    public Var Subtract(Var self, Var other, Executive executive)
    {
        var realSelf = (RealVar)self;
        return other.SubtractReal(realSelf, executive);
    }

    public Var Multiply(Var self, Var other, Executive executive)
    {
        var realSelf = (RealVar)self;
        return other.MultiplyReal(realSelf, executive);
    }

    public Var Divide(Var self, Var other, Executive executive)
    {
        var realSelf = (RealVar)self;
        return other.DivideReal(realSelf, executive);
    }

    public Var Power(Var self, Var other, Executive executive)
    {
        var realSelf = (RealVar)self;

        // Convert other to real if needed
        double exponent;
        if (other is IntegerVar intOther)
        {
            exponent = intOther.Data;
        }
        else if (other is RealVar realOther)
        {
            exponent = realOther.Data;
        }
        else
        {
            executive.LogRuntimeException(17);
            return StringVar.Null();
        }

        // Check for special cases
        if (realSelf.Data == 0 && exponent == 0)
        {
            executive.LogRuntimeException(18);
            return StringVar.Null();
        }

        var result = Math.Pow(realSelf.Data, exponent);

        if (double.IsNaN(result))
        {
            executive.LogRuntimeException(311);
            return StringVar.Null();
        }

        if (double.IsInfinity(result))
        {
            executive.LogRuntimeException(realSelf.Data == 0 ? 18 : 266);
            return StringVar.Null();
        }

        return new RealVar(result);
    }

    public Var Negate(Var self, Executive executive)
    {
        var realSelf = (RealVar)self;
        return new RealVar(-realSelf.Data);
    }
}