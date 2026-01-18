namespace Snobol4.Common;

public partial class Executive
{
    internal void Sin(List<Var> arguments) => UnaryNumericOperation(arguments, Sin0, 308, 0, 0);

    internal double Sin0(double dOperand) => Math.Sin(dOperand);
}