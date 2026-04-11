using Snobol4.Common;
using Test.TestLexer;

namespace Test.Memory;

[TestClass]
public class Collect
{
    [TestMethod]
    public void TEST_Collect_002_returns_integer()
    {
        // COLLECT() with no arg returns available memory as integer (non-negative)
        var s = @"
        c = collect()
        ge(c, 0)   :s(ok)f(bad)
ok      result = 'ok'  :(end)
bad     result = 'bad'
end";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("ok",
            ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("result")]).Data);
    }

    [TestMethod]
    public void TEST_Collect_003_after_alloc()
    {
        // COLLECT() can be called after allocating data — still succeeds
        var s = @"
        a = array(100)
        t = table(50)
        collect()
        result = 'ok'
end";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("ok",
            ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("result")]).Data);
    }

    [TestMethod]
    public void TEST_Collect_001()
    {
        var s = @"

        COLLECT(1)
end";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
    }
    [TestMethod]
    public void TEST_Collect_004_twice()
    {
        // COLLECT can be called multiple times without error
        var s = @"
        collect()
        collect()
        result = 'ok'
end";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("ok",
            ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("result")]).Data);
    }

    [TestMethod]
    public void TEST_Collect_005_returns_nonneg()
    {
        // COLLECT() result is always >= 0
        var s = @"
        c1 = collect()
        c2 = collect()
        ge(c1, 0)   :f(bad)
        ge(c2, 0)   :f(bad)
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
