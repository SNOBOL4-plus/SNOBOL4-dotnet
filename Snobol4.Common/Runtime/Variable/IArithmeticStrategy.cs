namespace Snobol4.Common;

/// <summary>
/// Strategy interface for arithmetic operations on variables
/// </summary>
public interface IArithmeticStrategy
{
    /// <summary>
    /// Add another variable to this one
    /// </summary>
    Var Add(Var self, Var other, Executive executive);

    /// <summary>
    /// Subtract another variable from this one
    /// </summary>
    Var Subtract(Var self, Var other, Executive executive);

    /// <summary>
    /// Multiply this variable by another
    /// </summary>
    Var Multiply(Var self, Var other, Executive executive);

    /// <summary>
    /// Divide this variable by another
    /// </summary>
    Var Divide(Var self, Var other, Executive executive);

    /// <summary>
    /// Raise this variable to the power of another
    /// </summary>
    Var Power(Var self, Var other, Executive executive);

    /// <summary>
    /// Negate this variable (unary minus)
    /// </summary>
    Var Negate(Var self, Executive executive);
}