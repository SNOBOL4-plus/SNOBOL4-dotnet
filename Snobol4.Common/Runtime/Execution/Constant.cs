namespace Snobol4.Common;

public partial class Executive
{
    #region Constants

    // ReSharper disable once UnusedMember.Global
    public void Constant(string value)
    {
        if (Builder.TraceStatements)
            Console.Error.WriteLine($@"Constant {value}");
        SystemStack.Push(new StringVar(value));
    }

    // ReSharper disable once UnusedMember.Global
    public void Constant(long value)
    {
        if (Builder.TraceStatements)
            Console.Error.WriteLine($@"Constant {value}");
        SystemStack.Push(new IntegerVar(value));
    }

    // ReSharper disable once UnusedMember.Global
    public void Constant(double value)
    {
        if (Builder.TraceStatements)
            Console.Error.WriteLine($@"Constant {value}");
        // Do not delete. Used by DLL
        SystemStack.Push(new RealVar(value));
    }

    // ReSharper disable once UnusedMember.Global
    public void Constant(DeferredCode value)
    {
        if (Builder.TraceStatements)
            Console.Error.WriteLine($@"Constant {value}");
        SystemStack.Push(new ExpressionVar(value));
    }

    #endregion
}