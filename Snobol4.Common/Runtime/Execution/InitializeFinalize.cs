namespace Snobol4.Common;

public partial class Executive
{
    // ReSharper disable once UnusedMember.Global
    public void InitializeStatement(int lineNumber)
    {
        using var profiler1 = Profiler.Start3("InitializeStatement", this);


        if (Parent.BuildOptions.TraceStatements)
            Console.Error.WriteLine($"""

                               InitializeStatement {lineNumber}

                               """);

        AmpCurrentLineNumber = lineNumber;
        Failure = false;
        AlphaStack.Clear(); // Used for conditional variable association
        BetaStack.Clear();  // Used for conditional variable association
        SystemStack.Push(new StatementSeparator());

        // Monitor bridge — LABEL event (SN-26-bridge-coverage-f).
        // 1-based statement number on the wire (matches scrip / csn / spl).
        MonitorIpc.EmitLabel((long)(lineNumber + 1));
    }

    // ReSharper disable once UnusedMember.Global
    public void FinalizeStatement()
    {
        using var profiler1 = Profiler.Start3($"FinalizeStatement", this);

        if (Parent.BuildOptions.TraceStatements)
            Console.Error.WriteLine("""

                              FinalizeStatement

                              """);

        while (SystemStack.Peek() is not StatementSeparator)
            SystemStack.Pop();

        SystemStack.Pop();
        AmpLastLineNumber = AmpCurrentLineNumber;
    }
}