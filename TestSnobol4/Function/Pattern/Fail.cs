using Snobol4.Common;
using Test.TestLexer;

namespace Test.Pattern;

[TestClass]
public class Fail
{

    [TestMethod]
    public void TEST_Fail_002_always_fails_goto()
    {
        // FAIL always causes pattern to fail; :F branch taken, :S branch never taken
        var s = @"
        subject = 'hello'
        subject fail    :s(success)f(done)
success result = 'wrong'   :(end)
done    result = 'correct'
end";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("correct",
            ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("result")]).Data);
    }

    [TestMethod]
    public void TEST_Fail_003_empty_subject()
    {
        // FAIL on empty subject also fails
        var s = @"
        subject = ''
        subject fail    :s(success)f(done)
success result = 'wrong'   :(end)
done    result = 'correct'
end";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("correct",
            ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("result")]).Data);
    }

    [TestMethod]
    public void TEST_Fail_004_exhaust_via_fail()
    {
        // FAIL after capture forces exhaustive backtrack — collects all matches via OUTPUT
        var s = @"
        &anchor = 0
        subject = 'abc'
        n = 0
        subject (len(1) . c fail) :f(done)
done    result = 'done'
end";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("done",
            ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("result")]).Data);
    }
    [TestMethod]
    public void TEST_Fail_005_in_alternation()
    {
        // FAIL in alternation with a real pattern — real pattern wins
        var s = @"
        subject = 'hello'
        subject (fail | 'hello') . cap   :s(ok)f(bad)
ok      result = cap   :(end)
bad     result = 'bad'
end";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("hello",
            ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("result")]).Data);
    }
}
