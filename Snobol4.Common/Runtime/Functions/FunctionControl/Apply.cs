namespace Snobol4.Common;

public partial class Executive
{
    public void Apply(List<Var> arguments)
    {
        //Debug.WriteLine("Apply()");
        if (!arguments[0].Convert(VarType.NAME, out var name, out var str, this) || (string)str == "")
        {
            LogRuntimeException(60);
            return;
        }

        arguments[0] = new StringVar(((NameVar)name).Pointer);
        arguments.RemoveAt(arguments.Count - 1);

        foreach (var argument in arguments)
            SystemStack.Push(argument);

        Function(arguments.Count - 1);
    }
}