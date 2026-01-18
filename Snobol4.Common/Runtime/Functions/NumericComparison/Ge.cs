namespace Snobol4.Common;

public partial class Executive
{
    internal void Ge(List<Var> arguments) => BinaryComparison(arguments, IntegerGe, RealGe, 109, 110);

    internal bool IntegerGe(long left, long right) => left >= right;

    internal bool RealGe(double left, double right) => left >= right;
}