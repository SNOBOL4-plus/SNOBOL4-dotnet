namespace Snobol4.Common;

public partial class Executive
{
    public void Integer(List<Var> arguments)
    {
        if (arguments[0].Convert(VarType.INTEGER, out _, out _, this))
        {
            PredicateSuccess();
            return;
        }

        NonExceptionFailure();
    }
}