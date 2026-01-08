namespace Snobol4.Common;

public partial class Executive
{
    //"lpad third argument is not a string" /* 144 */,
    //"lpad second argument is not integer" /* 145 */,
    //"lpad first argument is not a string" /* 146 */,

    internal void PadLeft(List<Var> arguments)
    {
        if (!arguments[0].Convert(VarType.STRING, out _, out var inputString, this))
        {
            LogRuntimeException(146);
            return;
        }

        if (!arguments[1].Convert(VarType.INTEGER, out _, out var totalWidth, this))
        {
            LogRuntimeException(145);
            return;
        }

        if (!arguments[2].Convert(VarType.STRING, out _, out var padString, this))
        {
            LogRuntimeException(144);
            return;
        }

        if ((int)(long)totalWidth < 0)
        {
            NonExceptionFailure();
            return;
        }

        var paddingChar = ((string)padString).Length == 0 ? ' ' : ((string)padString)[0];

        var outputString = ((string)inputString).PadLeft((int)(long)totalWidth, paddingChar);

        SystemStack.Push(new StringVar(outputString));
    }
}