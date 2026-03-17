namespace Snobol4.Common;

/// <summary>Arithmetic on opaque external pointers — not supported.</summary>
public class ExternalArithmeticStrategy : IArithmeticStrategy
{
    public Var Add(Var self, Var other, Executive executive)      => throw new NotSupportedException("Cannot add EXTERNAL");
    public Var Subtract(Var self, Var other, Executive executive) => throw new NotSupportedException("Cannot subtract EXTERNAL");
    public Var Multiply(Var self, Var other, Executive executive) => throw new NotSupportedException("Cannot multiply EXTERNAL");
    public Var Divide(Var self, Var other, Executive executive)   => throw new NotSupportedException("Cannot divide EXTERNAL");
    public Var Power(Var self, Var other, Executive executive)    => throw new NotSupportedException("Cannot exponentiate EXTERNAL");
    public Var Negate(Var self, Executive executive)              => throw new NotSupportedException("Cannot negate EXTERNAL");
}
