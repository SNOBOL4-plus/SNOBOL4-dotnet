namespace Snobol4.Common;

public partial class Executive
{
    internal void Power(List<Var> arguments)
    {
        BinaryNumericOperation(arguments, IntegerPower, RealPower,
            15, 16, 17, 266);
    }

    /// <summary>
    /// Calculate x^y where x and y are integers. There is no built-in .NET
    /// version for integers, so this has to be simulated.
    /// </summary>
    /// <param name="left">Left operand</param>
    /// <param name="right">Right operand</param>
    /// <returns></returns>
    internal long IntegerPower(long left, long right)
    {
        long result = 1;
        if (left == 0 && right <= 0)
        {
            LogRuntimeException(18);
            return 0;
        }

        checked
        {
            for (var i = 0; i < right; ++i)
                result *= left;
        }

        return result;
    }

    internal double RealPower(double left, double right)
    {
        return Math.Pow(left, right);
    }
}