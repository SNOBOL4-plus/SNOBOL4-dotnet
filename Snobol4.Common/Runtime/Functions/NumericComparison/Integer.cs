namespace Snobol4.Common;

public partial class Executive
{
    public void Integer(List<Var> arguments)
    {
        var arg = arguments[0];
        // IntegerVar: always succeeds
        if (arg is IntegerVar) { PredicateSuccess(); return; }
        // RealVar: always fails (even 3.0 is not an integer)
        if (arg is RealVar)    { NonExceptionFailure(); return; }
        // StringVar: succeed only if the string parses as a plain integer (not real)
        if (arg is StringVar sv && Var.ToInteger(sv.Data, out _)) { PredicateSuccess(); return; }
        NonExceptionFailure();
    }
}