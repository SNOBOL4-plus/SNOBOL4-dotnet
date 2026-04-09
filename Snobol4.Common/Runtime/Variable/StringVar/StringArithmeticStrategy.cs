namespace Snobol4.Common;

public class StringArithmeticStrategy : IArithmeticStrategy
{
    // SNOBOL4 spec: strings that look like numbers are coerced automatically.
    // "3" + 1 → integer 3 + 1 = 4.  Non-numeric strings throw the error.
    private static Var ToNumericVar(Var self, Executive executive, int errorCode)
    {
        var sv = (StringVar)self;
        if (Var.ToInteger(sv.Data, out var lv)) return IntegerVar.Create(lv);
        if (Var.ToReal(sv.Data, out var dv))   return new RealVar(dv);
        executive.LogRuntimeException(errorCode); // throws
        return StringVar.Null();
    }

    public Var Add(Var self, Var other, Executive executive)
        => ToNumericVar(self, executive, 1).Add(other, executive);

    public Var Subtract(Var self, Var other, Executive executive)
        => ToNumericVar(self, executive, 32).Subtract(other, executive);

    public Var Multiply(Var self, Var other, Executive executive)
        => ToNumericVar(self, executive, 26).Multiply(other, executive);

    public Var Divide(Var self, Var other, Executive executive)
        => ToNumericVar(self, executive, 12).Divide(other, executive);

    public Var Power(Var self, Var other, Executive executive)
        => ToNumericVar(self, executive, 15).Power(other, executive);

    public Var Negate(Var self, Executive executive)
        => ToNumericVar(self, executive, 10).Negate(executive);
}