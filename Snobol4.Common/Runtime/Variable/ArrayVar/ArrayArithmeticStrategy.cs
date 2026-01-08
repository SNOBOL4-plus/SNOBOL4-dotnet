namespace Snobol4.Common;

/// <summary>
/// Arithmetic strategy for array variables
/// Arrays don't support traditional arithmetic operations
/// </summary>
public class ArrayArithmeticStrategy : IArithmeticStrategy
{
    public Var Add(Var self, Var other, Executive executive)
    {
        // Arrays don't support addition
        executive.LogRuntimeException(1); // Left operand of + is not numeric
        return StringVar.Null();
    }

    public Var Subtract(Var self, Var other, Executive executive)
    {
        // Arrays don't support subtraction
        executive.LogRuntimeException(32); // Left operand of - is not numeric
        return StringVar.Null();
    }

    public Var Multiply(Var self, Var other, Executive executive)
    {
        // Arrays don't support multiplication
        executive.LogRuntimeException(26); // Left operand of * is not numeric
        return StringVar.Null();
    }

    public Var Divide(Var self, Var other, Executive executive)
    {
        // Arrays don't support division
        executive.LogRuntimeException(12); // Left operand of / is not numeric
        return StringVar.Null();
    }

    public Var Power(Var self, Var other, Executive executive)
    {
        // Arrays don't support exponentiation
        executive.LogRuntimeException(15); // Left operand of ^ is not numeric
        return StringVar.Null();
    }

    public Var Negate(Var self, Executive executive)
    {
        // Arrays don't support negation
        executive.LogRuntimeException(10); // Unary minus operand is not numeric
        return StringVar.Null();
    }
}