namespace Snobol4.Common;

//"rpad third argument is not a string" /* 178 */,
//"rpad second argument is not integer" /* 179 */,
//"rpad first argument is not a string" /* 180 */,

public partial class Executive
{
    internal void PadRight(List<Var> arguments)
    {
        if (!arguments[0].Convert(VarType.STRING, out _, out var inputString, this))
        {
            LogRuntimeException(180);
            return;
        }

        if (!arguments[1].Convert(VarType.INTEGER, out _, out var totalWidth, this))
        {
            LogRuntimeException(179);
            return;
        }

        if (!arguments[2].Convert(VarType.STRING, out _, out var padString, this))
        {
            LogRuntimeException(178);
            return;
        }

        if ((int)(long)totalWidth < 0)
        {
            NonExceptionFailure();
            return;
        }

        var paddingChar = ((string)padString).Length == 0 ? ' ' : ((string)padString)[0];

        var outputString = ((string)inputString).PadRight((int)(long)totalWidth, paddingChar);

        SystemStack.Push(new StringVar(outputString));
    }
}