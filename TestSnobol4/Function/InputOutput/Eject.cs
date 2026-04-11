using Test.TestLexer;

namespace Test.InputOutput;

[TestClass]
public class Eject
{
    [TestMethod]
    public void TEST_Eject_001f()
    {
        var s = @"

        EJECT(1)
end";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
    }
    [TestMethod]
    public void TEST_Eject_002()
    {
        // EJECT(0) is also valid (no-op or page break suppressed)
        var s = @"
        EJECT(0)
        result = 'ok'
end";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("ok", build.Execute!.IdentifierTable[build.FoldCase("result")].ToString());
    }

    [TestMethod]
    public void TEST_Eject_003()
    {
        // EJECT with no arg
        var s = @"
        EJECT()
        result = 'ok'
end";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("ok", build.Execute!.IdentifierTable[build.FoldCase("result")].ToString());
    }

    [TestMethod]
    public void TEST_Eject_004()
    {
        // Multiple EJECT calls in sequence are fine
        var s = @"
        EJECT(1)
        EJECT(1)
        EJECT(1)
        result = 'ok'
end";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("ok", build.Execute!.IdentifierTable[build.FoldCase("result")].ToString());
    }
}
