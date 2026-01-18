namespace Snobol4.Common;

public partial class Executive
{
    internal void Le(List<Var> arguments) => BinaryComparison(arguments, IntegerLe, RealLe, 121, 122);

    internal bool IntegerLe(long left, long right) => left <= right;

    internal bool RealLe(double left, double right) => left <= right;
}