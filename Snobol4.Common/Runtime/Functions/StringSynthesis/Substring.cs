namespace Snobol4.Common;

public partial class Executive
{
    //"substr third argument is not integer" /* 192 */,
    //"substr second argument is not integer" /* 193 */,
    //"substr first argument is not a string" /* 194 */,
    internal void Substring(List<Var> arguments)
    {
        if (!arguments[0].Convert(VarType.STRING, out _, out var inputString, this))
        {
            LogRuntimeException(194);
            return;
        }

        if (!arguments[1].Convert(VarType.INTEGER, out _, out var start, this))
        {
            LogRuntimeException(193);
            return;
        }

        if (!arguments[2].Convert(VarType.INTEGER, out _, out var length, this))
        {
            LogRuntimeException(192);
            return;
        }

        if ((int)(long)start > ((string)inputString).Length || (int)(long)start < 1 || (int)(long)length < 0 || (int)(long)length + (int)(long)start - 1 > ((string)inputString).Length)
        {
            NonExceptionFailure();
            return;
        }

        if ((long)length == 0)
        {
            SystemStack.Push(new StringVar(((string)inputString)[((int)(long)start - 1)..]));
            return;
        }

        SystemStack.Push(new StringVar(((string)inputString).Substring((int)(long)start - 1, (int)(long)length)));
    }
}