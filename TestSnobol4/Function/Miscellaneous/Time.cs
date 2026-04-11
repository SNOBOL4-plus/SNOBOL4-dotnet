using Snobol4.Common;
using Test.TestLexer;

namespace Test.Miscellaneous;

[TestClass]
public class Time
{
    [TestMethod]
    public void TEST_Time_002_returns_integer()
    {
        // TIME() returns milliseconds as integer — must be non-negative
        var s = @"
        t = time()
        ge(t, 0)   :s(ok)f(bad)
ok      result = 'nonneg'   :(end)
bad     result = 'negative'
end";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("nonneg",
            ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("result")]).Data);
    }

    [TestMethod]
    public void TEST_Time_003_two_calls_nondecreasing()
    {
        // Second call to TIME() returns value >= first call
        var s = @"
        t1 = time()
        t2 = time()
        ge(t2, t1)   :s(ok)f(bad)
ok      result = 'ok'   :(end)
bad     result = 'bad'
end";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("ok",
            ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("result")]).Data);
    }

    [TestMethod]
    public void TEST_Time_001()
    {
        var s = @"
        OUTPUT = TIME()
end";

        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
    }
    [TestMethod]
    public void TEST_Time_004_is_integer()
    {
        // TIME() result converts to integer without error
        var s = @"
        t = time()
        t2 = integer(t)   :f(bad)
        result = 'ok'     :(end)
bad     result = 'bad'
end";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("ok",
            ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("result")]).Data);
    }

    [TestMethod]
    public void TEST_Time_005_used_in_arith()
    {
        // TIME() result can be used in arithmetic
        var s = @"
        t = time()
        t2 = t + 0
        ge(t2, 0)   :f(bad)
        result = 'ok'   :(end)
bad     result = 'bad'
end";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("ok",
            ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("result")]).Data);
    }
}
