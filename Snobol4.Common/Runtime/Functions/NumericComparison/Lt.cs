namespace Snobol4.Common;

public partial class Executive
{
    internal void Lt(List<Var> arguments) => BinaryComparison(arguments, IntegerLt, RealLt, 147, 148);

    internal bool IntegerLt(long left, long right) => left < right;

    internal bool RealLt(double left, double right) => left < right;
}