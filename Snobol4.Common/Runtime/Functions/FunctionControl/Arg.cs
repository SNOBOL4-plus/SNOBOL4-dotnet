namespace Snobol4.Common;

public partial class Executive
{

    //"arg second argument is not integer" /* 62 */,
    //"arg first argument is not program function name" /* 63 */,

    #region Arg

    public void Arg(List<Var> arguments)
    {
        //Debug.WriteLine("Arg()");
        if (!arguments[0].Convert(VarType.STRING, out _, out var str,this) || (string)str == "")
        {
            LogRuntimeException(60);
            return;
        }

        if (!FunctionTable.TryGetValue((string)str, out var entry))
        {
            LogRuntimeException(63);
            return;
        }

        if (!arguments[1].Convert(VarType.INTEGER, out _, out var i,this))
        {
            LogRuntimeException(62);
            return;
        }

        if ((long)i > entry.ArgumentCount || (long)i <= 0)
        {
            NonExceptionFailure();
            return;
        }

        SystemStack.Push(new StringVar(entry.Locals[(int)(long)i - 1]));
    }

    #endregion
}