namespace Snobol4.Common;

public partial class Executive
{
    internal void Ne(List<Var> arguments) => BinaryComparison(arguments, IntegerNe, RealNe, 149, 150);

    internal bool IntegerNe(long left, long right) => left != right;

    internal bool RealNe(double left, double right) => left != right;  
}