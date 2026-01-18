namespace Snobol4.Common;

public partial class Executive
{
    internal void Exp(List<Var> arguments) => UnaryNumericOperation(arguments, Exp0, 304, 305, 0);

    internal double Exp0(double dOperand) => Math.Exp(dOperand);
}