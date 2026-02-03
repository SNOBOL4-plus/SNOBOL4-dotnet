namespace Snobol4.Common;

//"multiplication left operand is not numeric" /* 26 */,
//"multiplication right operand is not numeric" /* 27 */,
//"multiplication caused integer overflow" /* 28 */,
//"multiplication caused real overflow" /* 263 */,

public partial class Executive
{
    internal void Multiply(List<Var> arguments)
    {
        BinaryNumericOperation(arguments, IntegerMultiply, RealMultiply, 26, 27, 28, 263);
    }

    internal long IntegerMultiply(long left, long right)
    {
        checked
        {
            return left * right;
        }
    }

    internal double RealMultiply(double left, double right)
    {
        return left * right;
    }
}