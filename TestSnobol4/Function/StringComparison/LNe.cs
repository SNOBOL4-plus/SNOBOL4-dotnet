using Snobol4.Common;
using Test.TestLexer;

namespace Test.StringComparison;

[TestClass]
public class LNe
{
    [TestMethod]
    public void TEST_LNe_001()
    {
        var s = @"
        r = 'success'        
        a = 'this is a test'
        b = 'this is a test'
        lne(a,b) :s(end)
        r = 'failure'
end
";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("failure", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("r")]).Data);
    }

    [TestMethod]
    public void TEST_LNe_002()
    {
        var s = @"
        r = 'success'        
        a = 'this is a test'
        b = 'this is not a test'
        lne(a,b) :s(end)
        r = 'failure'
end
";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("success", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("r")]).Data);
    }

    [TestMethod]
    public void TEST_LNe_003()
    {
        var s = @"
        r = 'success'        
        a = 'this is a test'
        b = 'this is not a test'
        lne(b,a) :s(end)
        r = 'failure'
end
";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("success", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("r")]).Data);
    }

    [TestMethod]
    public void TEST_LNe_004()
    {
        var s = @"
        r = 'success'        
        a = any('this is a test')
        b = 'this is not a test'
        lne(a,b) :s(end)
        r = 'failure'
end
";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreNotEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual(132, build.ErrorCodeHistory[0]);
    }

    [TestMethod]
    public void TEST_LNe_006_ordinal_upper_ne_lower()
    {
        // Ordinal: 'A'(65) != 'a'(97) — lne('A','a') must SUCCEED
        var s = @"
        r = 'success'
        lne('A', 'a') :s(end)
        r = 'failure'
end
";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("success", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("r")]).Data);
    }

    [TestMethod]
    public void TEST_LNe_007_null_equals_null_fails()
    {
        // lne('','') must FAIL — null equals null
        var s = @"
        r = 'success'
        lne('', '') :s(end)
        r = 'failure'
end
";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("failure", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("r")]).Data);
    }

    [TestMethod]
    public void TEST_LNe_005()
    {
        var s = @"
        a = 'this is a test'
        b = any('this is a test')
        lne(a,b)
end
";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreNotEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual(133, build.ErrorCodeHistory[0]);
    }
}