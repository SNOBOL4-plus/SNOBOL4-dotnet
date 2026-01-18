namespace Snobol4.Common;

public partial class Executive
{
    internal void Chop(List<Var> arguments) => UnaryNumericOperation(arguments, Chop0, 302, 0, 0);

    internal double Chop0(double dOperand) => Math.Round(dOperand, MidpointRounding.ToZero);
}