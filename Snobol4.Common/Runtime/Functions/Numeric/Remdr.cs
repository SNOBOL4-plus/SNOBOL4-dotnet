namespace Snobol4.Common;

public partial class Executive
{
    internal void Remainder(List<Var> arguments) => BinaryNumericOperation(arguments, IntegerRemainder, RealRemainder, 166, 165, 167, 312);

    internal long IntegerRemainder(long left, long right) => left % right;

    internal double RealRemainder(double left, double right) => left % right;   
}