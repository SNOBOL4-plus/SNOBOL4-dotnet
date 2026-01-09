namespace Snobol4.Common;

/// <summary>
/// Arithmetic strategy for array variables.
/// Arrays do not support traditional arithmetic operations.
/// All operations log appropriate errors and return null.
/// </summary>
public class ArrayArithmeticStrategy : IArithmeticStrategy
{
    public Var Add(Var self, Var other, Executive executive)
    {
        executive.LogRuntimeException(1); // Left operand of + is not numeric
        return StringVar.Null();
    }

    public Var Subtract(Var self, Var other, Executive executive)
    {
        executive.LogRuntimeException(32); // Left operand of - is not numeric
        return StringVar.Null();
    }

    public Var Multiply(Var self, Var other, Executive executive)
    {
        executive.LogRuntimeException(26); // Left operand of * is not numeric
        return StringVar.Null();
    }

    public Var Divide(Var self, Var other, Executive executive)
    {
        executive.LogRuntimeException(12); // Left operand of / is not numeric
        return StringVar.Null();
    }

    public Var Power(Var self, Var other, Executive executive)
    {
        executive.LogRuntimeException(15); // Left operand of ^ is not numeric
        return StringVar.Null();
    }

    public Var Negate(Var self, Executive executive)
    {
        executive.LogRuntimeException(10); // Unary minus operand is not numeric
        return StringVar.Null();
    }
}