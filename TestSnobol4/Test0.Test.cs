// ********************************
// **** MACHINE GENERATED CODE ****
// **** DO NOT EDIT            ****
// **** 5/23/2025 3:29:16 PM   ****
// ********************************

using System;
using System.Collections;
using System.Collections.Generic;
using Snobol4.Common;

namespace Test0;

public class Test
{
    delegate int StatementCode(Executive x);

    public int Run(Executive x)
    {
        Executive.BreakPoint();

        x.Parent.CaseFolding = true;
        x.Parent.DisplayListingHeader = false;
        x.Parent.ErrorsToStdout = false;
        x.Parent.FilesToCompile.Add(@"C:\Users\jcooper\Documents\Visual Studio 2022\Snobol4\TestSnobol4\Test.sno");
        x.Parent.ListFileName = @"";
        x.Parent.Listing = false;
        x.Parent.ShowExecutionStatistics = false;
        ConsoleExt.SetStdError(x.Parent.Listing, x.Parent.ListFileName, x.Parent.FilesToCompile[^1]);

        x.SourceCode.Add("Test.sno(3)\n    S = \"L  a = a ' ' N; N = lt(N,10) N + 1 :S(L)F(DONE)\"");
        x.SourceCode.Add("Test.sno(4)\n    code(S)");
        x.SourceCode.Add("Test.sno(6)\n    :(L)");
        x.SourceCode.Add("Test.sno(8)\nDONE output = a");
        x.SourceCode.Add("Test.sno(9)\nend");

        x.SourceFiles.Add(@"C:\Users\jcooper\Documents\Visual Studio 2022\Snobol4\TestSnobol4\Test.sno");
        x.SourceFiles.Add(@"C:\Users\jcooper\Documents\Visual Studio 2022\Snobol4\TestSnobol4\Test.sno");
        x.SourceFiles.Add(@"C:\Users\jcooper\Documents\Visual Studio 2022\Snobol4\TestSnobol4\Test.sno");
        x.SourceFiles.Add(@"C:\Users\jcooper\Documents\Visual Studio 2022\Snobol4\TestSnobol4\Test.sno");
        x.SourceFiles.Add(@"C:\Users\jcooper\Documents\Visual Studio 2022\Snobol4\TestSnobol4\Test.sno");

        x.SourceLineNumbers.Add(3);
        x.SourceLineNumbers.Add(4);
        x.SourceLineNumbers.Add(6);
        x.SourceLineNumbers.Add(8);
        x.SourceLineNumbers.Add(9);

        x.SourceStatementNumbers.Add(1);
        x.SourceStatementNumbers.Add(2);
        x.SourceStatementNumbers.Add(3);
        x.SourceStatementNumbers.Add(4);
        x.SourceStatementNumbers.Add(5);

        x.Statements.Add(Statement0000000);
        x.Statements.Add(Statement0000001);
        x.Statements.Add(Statement0000002);
        x.Statements.Add(Statement0000003);
        x.Statements.Add(Statement0000004);

        x.Labels.Add("DONE", 3);
        x.Labels.Add("end",-1);
        x.Labels.Add("return", -2);
        x.Labels.Add("sreturn", -3);
        x.Labels.Add("freturn", -4);

        x.ExecuteLoop(0);

        return 0;
    }

    private int Statement0000000(Executive x)
    {
        // Test.sno(3):     S = \"L  a = a ' ' N; N = lt(N,10) N + 1 :S(L)F(DONE)\"
        x.InitializeStatement(1);
        x.Identifier("S");
        x.Constant("L  a = a ' ' N; N = lt(N,10) N + 1 :S(L)F(DONE)");
        x._BinaryEquals();
        x.FinalizeStatement();
        return 1;
    }

    private int Statement0000001(Executive x)
    {
        // Test.sno(4):     code(S)
        x.InitializeStatement(2);
        x.FunctionName("code");
        x.Identifier("S");
        x.Function(1);
        x.FinalizeStatement();
        return 2;
    }

    private int Statement0000002(Executive x)
    {
        // Test.sno(6):     :(L)
        x.InitializeStatement(3);
        x.FinalizeStatement();
        // Process unconditional goto
        bool bSaveStatus = x.Failure;
        x.Failure = false;
        x.Identifier("L");
        if (x.Failure)
            x.LogRuntimeException(20);
        x.SaveStatus(bSaveStatus);
        if (x.Labels.ContainsKey(x.SystemStack.Peek().Symbol))
            return x.Labels[x.SystemStack.Pop().Symbol];
        x.LogRuntimeException(23);
        return -1;
    }

    private int Statement0000003(Executive x)
    {
        // Test.sno(8): DONE output = a
        x.InitializeStatement(4);
        x.Identifier("output");
        x.Identifier("a");
        x._BinaryEquals();
        x.FinalizeStatement();
        return 4;
    }

    private int Statement0000004(Executive x)
    {
        // Test.sno(9): end
        x.InitializeStatement(5);
        return -1;
    }
}
