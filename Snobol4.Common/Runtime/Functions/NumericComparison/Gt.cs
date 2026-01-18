namespace Snobol4.Common;

public partial class Executive
{
    internal void Gt(List<Var> arguments) => BinaryComparison(arguments, IntegerGt, RealGt, 111, 112);

    internal bool IntegerGt(long left, long right) => left > right;

    internal bool RealGt(double left, double right) => left > right;
}