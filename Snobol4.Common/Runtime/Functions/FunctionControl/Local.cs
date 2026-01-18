namespace Snobol4.Common;

//"local second argument is not integer" /* 134 */,
//"local first arg is not a program function name" /* 135 */,

public partial class Executive
{
    public void Local(List<Var> arguments)
    {
        //Debug.WriteLine("Local()");
        if (!arguments[0].Convert(VarType.STRING, out _, out var str, this) || (string)str == "")
        {   
            LogRuntimeException(60);
            return;
        }

        if (!FunctionTable.TryGetValue((string)str, out var entry))
        {
            LogRuntimeException(135);
            return;
        }

        if (!arguments[1].Convert(VarType.INTEGER, out _, out var i, this))
        {
            LogRuntimeException(134);
            return;
        }

        if ((long)i > entry.Locals.Count - entry.ArgumentCount - 1 || (long)i <= 0)
        {
            NonExceptionFailure();
            return;
        }

        SystemStack.Push(new StringVar(entry.Locals[(int)(long)i + entry.ArgumentCount - 1]));
    }
}