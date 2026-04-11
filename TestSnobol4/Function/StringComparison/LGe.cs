using Snobol4.Common;
using Test.TestLexer;

namespace Test.StringComparison;

[TestClass]
public class LGe
{
    [TestMethod]
    public void TEST_LGe_001()
    {
        var s = @"
        r = 'success'        
        a = 'this is a test'
        b = 'this is a test'
        lge(a,b) :s(end)
        r = 'failure'
end
";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("success", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("r")]).Data);
    }

    [TestMethod]
    public void TEST_LGe_002()
    {
        var s = @"
        r = 'success'        
        a = 'this is a test'
        b = 'this is not a test'
        lge(a,b) :s(end)
        r = 'failure'
end
";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("failure", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("r")]).Data);
    }

    [TestMethod]
    public void TEST_LGe_003()
    {
        var s = @"
        r = 'success'        
        a = 'this is a test'
        b = 'this is not a test'
        lge(b,a) :s(end)
        r = 'failure'
end
";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("success", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("r")]).Data);
    }


    [TestMethod]
    public void TEST_LGe_004()
    {
        var s = @"
        a = any('this is a test')
        b = 'this is a test'
        lge(a,b)
end
";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreNotEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual(124, build.ErrorCodeHistory[0]);
    }

    [TestMethod]
    public void TEST_LGe_006_ordinal_lower_ge_upper()
    {
        // Ordinal: 'a'=97 >= 'A'=65 — lge('a','A') must SUCCEED
        var s = @"
        r = 'success'
        lge('a', 'A') :s(end)
        r = 'failure'
end
";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("success", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("r")]).Data);
    }

    [TestMethod]
    public void TEST_LGe_007_null_ge_null()
    {
        // lge('','') must SUCCEED — null >= null
        var s = @"
        r = 'success'
        lge('', '') :s(end)
        r = 'failure'
end
";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("success", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("r")]).Data);
    }

    [TestMethod]
    public void TEST_LGe_005()
    {
        var s = @"
        a = 'this is a test'
        b = any('this is a test')
        lge(a,b)
end
";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreNotEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual(125, build.ErrorCodeHistory[0]);
    }

    [TestMethod]
    public void TEST_LGe_008_nonempty_ge_null()
    {
        // lge('a','') must SUCCEED — any nonempty >= null
        var s = @"
        r = 'success'
        lge('a', '') :s(end)
        r = 'failure'
end
";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("success", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("r")]).Data);
    }

}
