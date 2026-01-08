namespace Snobol4.Common;

public partial class Executive
{
    internal void Prototype(List<Var> arguments)
    {
        switch (arguments[0])
        {
            case ArrayVar array:
                SystemStack.Push(new StringVar(array.Prototype));
                return;
            case TableVar:
                SystemStack.Push(StringVar.Null());
                return;
            default:
                LogRuntimeException(164);
                break;
        }
    }
}