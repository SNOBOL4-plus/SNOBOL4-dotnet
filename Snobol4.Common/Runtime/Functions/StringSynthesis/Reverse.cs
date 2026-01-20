namespace Snobol4.Common;

//"reverse argument is not a string" /* 177 */,

public partial class Executive
{
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