namespace Snobol4.Common;

public partial class Executive
{
    // ReSharper disable once UnusedMember.Global
    public void InitializeStatement(int lineNumber)
    {
        using var profiler1 = Profiler.Start("InitializeStatement", this);


        if (Parent.TraceStatements)
            Console.Error.WriteLine($"""

                               InitializeStatement {lineNumber}

                               """);

        AmpCurrentLineNumber = lineNumber;
        Failure = false;
        AlphaStack.Clear(); // Used for conditional variable association
        BetaStack.Clear();  // Used for conditional variable association
        SystemStack.Push(new StatementSeparator());
    }

    // ReSharper disable once UnusedMember.Global
    public void FinalizeStatement()
    {
        using var profiler1 = Profiler.Start($"FinalizeStatement", this);

        if (Parent.TraceStatements)
            Console.Error.WriteLine("""

                              FinalizeStatement

                              """);

        while (SystemStack.Peek() is not StatementSeparator)
            SystemStack.Pop();

        SystemStack.Pop();
        AmpLastLineNumber = AmpCurrentLineNumber;
    }
}