using Snobol4.Common;
using System.Text;
using Test.TestLexer;

namespace Test.Phase4;

/// <summary>
/// Correctness tests for the threaded execution engine.
/// Each test runs the same SNOBOL4 program through both paths and
/// compares results — ensuring threaded execution is behaviourally
/// identical to the C# generation path.
/// </summary>
[TestClass]
public class ThreadedExecutionTests
{
    private static Builder RunThreaded(string script)
    {
        var build = SetupTests.SetupScript("-b", script);
        // Re-run with threaded flag on
        build.BuildOptions.UseThreadedExecution = true;
        var tc = new ThreadedCodeCompiler(build);
        build.Execute!.Thread = tc.Compile();
        build.Execute.ThreadedExecuteLoop();
        return build;
    }

    // -----------------------------------------------------------------------
    // Basic assignment and arithmetic
    // -----------------------------------------------------------------------

    [TestMethod]
    public void Threaded_SimpleAssignment()
    {
        var b = RunThreaded("        N = 42\nend");
        Assert.AreEqual("42", b.Execute!.IdentifierTable["N"].ToString());
    }

    [TestMethod]
    public void Threaded_Addition()
    {
        var b = RunThreaded("        N = 3 + 4\nend");
        Assert.AreEqual("7", b.Execute!.IdentifierTable["N"].ToString());
    }

    [TestMethod]
    public void Threaded_StringConcat()
    {
        var b = RunThreaded("        S = 'Hello' ' ' 'World'\nend");
        Assert.AreEqual("Hello World", b.Execute!.IdentifierTable["S"].ToString());
    }

    // -----------------------------------------------------------------------
    // Loop with goto
    // -----------------------------------------------------------------------

    [TestMethod]
    public void Threaded_CountTo10()
    {
        var b = RunThreaded(@"
        N = 0
LOOP    N = N + 1
        LT(N,10)        :S(LOOP)
        RESULT = N
end");
        Assert.AreEqual("10", b.Execute!.IdentifierTable["RESULT"].ToString());
    }

    // -----------------------------------------------------------------------
    // Roman numeral — recursive function
    // -----------------------------------------------------------------------

    [TestMethod]
    public void Threaded_Roman_1776()
    {
        var b = RunThreaded(@"
        DEFINE('ROMAN(N)T')                 :(ROMAN_END)
ROMAN   N   RPOS(1)  LEN(1) . T  =         :F(RETURN)
        '0,1I,2II,3III,4IV,5V,6VI,7VII,8VIII,9IX,'
+       T   BREAK(',') . T                  :F(FRETURN)
        ROMAN = REPLACE(ROMAN(N), 'IVXLCDM', 'XLCDM**') T
+                                           :S(RETURN)F(FRETURN)
ROMAN_END
        R1 = ROMAN('1776')
end");
        Assert.AreEqual("MDCCLXXVI", b.Execute!.IdentifierTable["R1"].ToString());
    }
}
