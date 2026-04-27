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
        // Aligned with SPITBOL's &STNO convention: count blank lines as
        // consuming an stno slot.  See MsilHelpers.cs InitStatementMsil.
        // S-2-bridge-7 (stno alignment), Mon Apr 28 2026.
        {
            var srcLines = Parent.Code.SourceLines;
            int blanks = (lineNumber >= 0 && lineNumber < srcLines.Count) ? srcLines[lineNumber].BlankLineCount : 0;
            MonitorIpc.EmitLabel((long)(lineNumber + 1 + blanks));
        }
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