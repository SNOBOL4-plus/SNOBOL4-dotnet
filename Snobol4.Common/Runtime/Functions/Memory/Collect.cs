namespace Snobol4.Common;

public partial class Executive
{
    internal void GarbageCollect(List<Var> arguments)
    {
        GC.Collect();
        SystemStack.Push(StringVar.Null());
    }
}