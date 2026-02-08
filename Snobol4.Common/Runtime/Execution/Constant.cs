namespace Snobol4.Common;

public partial class Executive
{
    // ReSharper disable once UnusedMember.Global
    public void Constant(string value)
    {
        using var profiler = Profiler.Start3("Constant", this);
        SystemStack.Push(new StringVar(value));
    }

    // ReSharper disable once UnusedMember.Global
    public void Constant(long value)
    {
        using var profiler = Profiler.Start3("Constant", this);
        SystemStack.Push(new IntegerVar(value));
    }

    // ReSharper disable once UnusedMember.Global
    public void Constant(double value)
    {
        using var profiler = Profiler.Start3("Constant", this);
        SystemStack.Push(new RealVar(value));
    }

    // ReSharper disable once UnusedMember.Global
    public void Constant(DeferredCode value)
    {
        using var profiler = Profiler.Start3("Constant", this);
        SystemStack.Push(new ExpressionVar(value));
    }
}