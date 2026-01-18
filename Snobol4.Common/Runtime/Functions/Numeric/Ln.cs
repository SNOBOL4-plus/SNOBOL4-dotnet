namespace Snobol4.Common;

public partial class Executive
{
    internal void Ln(List<Var> arguments) => UnaryNumericOperation(arguments, Ln0, 306, 307, 315);

    internal double Ln0(double dOperand) => Math.Log(dOperand);
}