using Test.TestLexer;

namespace Test.Miscellaneous;

[TestClass]
public class Date
{
    [TestMethod]
    public void TEST_Date_001()
    {
        var s = @"
        OUTPUT = DATE()
end";

        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
    }
    [TestMethod]
    public void TEST_Date_002_nonempty()
    {
        // DATE() returns a non-empty string
        var s = @"
        d = date()
        differ(d, '')   :f(bad)
        result = 'ok'   :(end)
bad     result = 'bad'
end";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("ok", build.Execute!.IdentifierTable[build.FoldCase("result")].ToString());
    }

    [TestMethod]
    public void TEST_Date_003_consistent()
    {
        // Two calls to DATE() in same run return same value
        var s = @"
        d1 = date()
        d2 = date()
        differ(d1, d2)   :s(bad)
        result = 'ok'    :(end)
bad     result = 'bad'
end";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("ok", build.Execute!.IdentifierTable[build.FoldCase("result")].ToString());
    }
}
