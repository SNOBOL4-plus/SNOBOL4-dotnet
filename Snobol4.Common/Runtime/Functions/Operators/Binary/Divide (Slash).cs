namespace Snobol4.Common;

public partial class Executive
{
    internal void Divide(List<Var> arguments)
    {
        BinaryNumericOperation(arguments, IntegerDivide, RealDivide,
            12, 13, 14, 262);
    }

    internal long IntegerDivide(long left, long right)
    {
        return left / right;
    }

    internal double RealDivide(double left, double right)
    {
        return left / right;
    }
}