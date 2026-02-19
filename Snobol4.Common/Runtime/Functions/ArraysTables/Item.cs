namespace Snobol4.Common;

public partial class Executive
{
    internal void Item(List<Var> arguments)
    {
        SystemStack.Push(arguments[0]);
        arguments.RemoveAt(0);
        arguments.RemoveAt(arguments.Count - 1);

        foreach (var argument in arguments)
            SystemStack.Push(argument);

        IndexCollection();
    }
}