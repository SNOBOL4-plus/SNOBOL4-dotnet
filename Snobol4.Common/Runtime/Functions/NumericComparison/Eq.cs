namespace Snobol4.Common;

public partial class Executive
{
    internal void Eq(List<Var> arguments) => BinaryComparison(arguments, IntegerEq, RealEq, 101, 102);

    internal bool IntegerEq(long left, long right) => left == right;

    internal bool RealEq(double left, double right) => left == right;
}