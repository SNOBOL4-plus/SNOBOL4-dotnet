namespace Snobol4.Common;

public partial class Executive
{
    internal void Cos(List<Var> arguments) => UnaryNumericOperation(arguments, Cos0, 303, 0, 0);

    internal double Cos0(double dOperand) => Math.Cos(dOperand);
}