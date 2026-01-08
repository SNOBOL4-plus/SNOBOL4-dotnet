namespace Snobol4.Common;

/// <summary>
/// Arithmetic strategy for expression variables
/// Expressions don't support traditional arithmetic operations
/// </summary>
public class ExpressionArithmeticStrategy : IArithmeticStrategy
{
    public Var Add(Var self, Var other, Executive executive)
    {
        // Expressions don't support addition
        executive.LogRuntimeException(1); // Left operand of + is not numeric
        return StringVar.Null();
    }

    public Var Subtract(Var self, Var other, Executive executive)
    {
        // Expressions don't support subtraction
        executive.LogRuntimeException(32); // Left operand of - is not numeric
        return StringVar.Null();
    }

    public Var Multiply(Var self, Var other, Executive executive)
    {
        // Expressions don't support multiplication
        executive.LogRuntimeException(26); // Left operand of * is not numeric
        return StringVar.Null();
    }

    public Var Divide(Var self, Var other, Executive executive)
    {
        // Expressions don't support division
        executive.LogRuntimeException(12); // Left operand of / is not numeric
        return StringVar.Null();
    }

    public Var Power(Var self, Var other, Executive executive)
    {
        // Expressions don't support exponentiation
        executive.LogRuntimeException(15); // Left operand of ^ is not numeric
        return StringVar.Null();
    }

    public Var Negate(Var self, Executive executive)
    {
        // Expressions don't support negation
        executive.LogRuntimeException(10); // Unary minus operand is not numeric
        return StringVar.Null();
    }
}