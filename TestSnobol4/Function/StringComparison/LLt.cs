using Snobol4.Common;
using Test.TestLexer;

namespace Test.StringComparison;

[TestClass]
public class LLt
{
    [TestMethod]
    public void TEST_LLt_001()
    {
        var s = @"
        r = 'success'        
        a = 'this is a test'
        b = 'this is a test'
        llt(a,b) :s(end)
        r = 'failure'
end
";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("failure", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("r")]).Data);
    }

    [TestMethod]
    public void TEST_LLt_002()
    {
        var s = @"
        r = 'success'        
        a = 'this is a test'
        b = 'this is not a test'
        llt(a,b) :s(end)
        r = 'failure'
end
";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("success", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("r")]).Data);
    }

    [TestMethod]
    public void TEST_LLt_003()
    {
        var s = @"
        r = 'success'        
        a = 'this is a test'
        b = 'this is not a test'
        llt(b,a) :s(end)
        r = 'failure'
end
";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("failure", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("r")]).Data);
    }

    [TestMethod]
    public void TEST_LLt_004()
    {
        var s = @"
        a = any('this is a test')
        b = 'this is a test'
        llt(a,b)
end
";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreNotEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual(130, build.ErrorCodeHistory[0]);
    }

    [TestMethod]
    public void TEST_LLt_006_ordinal_upper_vs_lower()
    {
        // Ordinal: 'A'=65 < 'a'=97 — llt('A','a') must SUCCEED
        var s = @"
        r = 'success'
        llt('A', 'a') :s(end)
        r = 'failure'
end
";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("success", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("r")]).Data);
    }

    [TestMethod]
    public void TEST_LLt_007_ordinal_lower_vs_upper_fails()
    {
        // Ordinal: 'a'=97 > 'A'=65 — llt('a','A') must FAIL
        var s = @"
        r = 'success'
        llt('a', 'A') :s(end)
        r = 'failure'
end
";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("failure", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("r")]).Data);
    }

    [TestMethod]
    public void TEST_LLt_008_null_lt_nonempty()
    {
        // null (empty) < any non-empty string
        var s = @"
        r = 'success'
        llt('', 'a') :s(end)
        r = 'failure'
end
";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("success", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("r")]).Data);
    }

    [TestMethod]
    public void TEST_LLt_005()
    {
        var s = @"
        a = 'this is a test'
        b = any('this is a test')
        llt(a,b)
end
";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreNotEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual(131, build.ErrorCodeHistory[0]);
    }
}