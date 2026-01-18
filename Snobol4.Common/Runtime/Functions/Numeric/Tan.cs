namespace Snobol4.Common;

public partial class Executive
{
    internal void Tan(List<Var> arguments) => UnaryNumericOperation(arguments, Tan0, 313, 312, 0);

    internal double Tan0(double dOperand) => Math.Tan(dOperand);            
}