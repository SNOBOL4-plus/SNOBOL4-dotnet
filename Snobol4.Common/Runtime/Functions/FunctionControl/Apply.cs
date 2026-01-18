namespace Snobol4.Common;

public partial class Executive
{
    public void Apply(List<Var> arguments)
    {
        //Debug.WriteLine("Apply()");
        if (!arguments[0].Convert(VarType.STRING, out _, out var str, this) || (string)str == "")
        {
            LogRuntimeException(60);
            return;
        }

        arguments.RemoveAt(arguments.Count - 1);

        foreach (var argument in arguments)
            SystemStack.Push(argument);

        Function(arguments.Count - 1);
    }
}