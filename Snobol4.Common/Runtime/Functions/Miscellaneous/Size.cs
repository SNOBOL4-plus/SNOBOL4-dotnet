namespace Snobol4.Common;

public partial class Executive
{
    public void Size(List<Var> arguments)
    {
        if (!arguments[0].Convert(VarType.STRING, out _, out var str, this))
        {
            LogRuntimeException(5);
            return;
        }

        SystemStack.Push(new IntegerVar(((string)str).Length));
    }
}