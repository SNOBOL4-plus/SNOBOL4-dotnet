namespace Snobol4.Common;

public partial class Executive
{
    //"reverse argument is not a string" /* 177 */,

    internal void Reverse(List<Var> arguments)
    {
        if (!arguments[0].Convert(VarType.STRING, out _, out var inputString, this))
        {
            LogRuntimeException(177);
            return;
        }

        var outputString = new string(((string)inputString).ToCharArray().Reverse().ToArray());
        SystemStack.Push(new StringVar(outputString));
    }
}