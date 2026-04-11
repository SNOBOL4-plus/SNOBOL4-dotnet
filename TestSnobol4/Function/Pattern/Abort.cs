using Snobol4.Common;
using Test.TestLexer;

namespace Test.Pattern;

[TestClass]
public class Abort
{

    [TestMethod]
    public void TEST_Abort_001()
    {
        var s = @"
        &anchor = 0
        subject = '-ab-1-'
        pattern = any('ab') | '1' abort
        subject pattern :f(n)
        subject pattern      :s(y)f(n)
y       result = 'success'   :(end)
n       result = 'fail' 
end";

        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual("success", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("result")]).Data);
    }

    [TestMethod]
    public void TEST_Abort_003_abort_at_start()
    {
        // ABORT immediately kills entire match attempt — no backtracking
        var s = @"
        &anchor = 0
        subject = 'hello world'
        subject abort   :s(y)f(n)
y       result = 'wrong'  :(end)
n       result = 'correct'
end";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("correct",
            ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("result")]).Data);
    }

    [TestMethod]
    public void TEST_Abort_004_abort_prevents_backtrack()
    {
        // Without ABORT, alternation backtracks and tries second branch.
        // With ABORT, the entire match is killed — second branch never tried.
        var s = @"
        &anchor = 0
        subject = 'xbx'
        pattern = ('a' | abort) 'b'
        subject pattern :s(y)f(n)
y       result = 'wrong'  :(end)
n       result = 'correct'
end";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("correct",
            ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("result")]).Data);
    }

    [TestMethod]
    public void TEST_Abort_002()
    {
        var s = @"
        &anchor = 0
        subject = '-1a-b-'
        pattern = any('ab') | '1' abort
        subject pattern :f(n)
        subject pattern      :s(y)f(n)
y       result = 'success'   :(end)
n       result = 'fail' 
end";

        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual("fail", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("result")]).Data);
    }

}