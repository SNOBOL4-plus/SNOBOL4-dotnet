using Snobol4.Common;
using Test.TestLexer;

namespace Test.Operator;

[TestClass]
public class Negation
{
    [TestMethod]
    public void TEST_Negation_001()
    {
        var s = @"
	    ~integer('a')   :f(n)
	    result = 'succeed' :(end)
n	    result = 'failure'
end
";
        var directives = "-b ";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("succeed", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("result")]).Data);
    }

    [TestMethod]
    public void TEST_Negation_002()
    {
        var s = @"
	    ~integer('3')   :f(n)
	    result = 'succeed' :(end)
n	    result = 'failure'
end
";
        var directives = "-b ";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("failure", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("result")]).Data);
    }


    [TestMethod]
    public void TEST_Negation_003()
    {
        var s = @"
	    ~integer(5)   :f(n)
	    result = 'succeed' :(end)
n	    result = 'failure'
end
";
        var directives = "-b ";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("failure", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("result")]).Data);
    }
    [TestMethod]
    public void TEST_Negation_004()
    {
        // ~ applied to a string comparison: negates failure -> success
        var s = @"
        ~differ('hello', 'hello')   :f(n)
        result = 'succeed'          :(end)
n       result = 'failure'
end
";
        var directives = "-b ";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("succeed", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("result")]).Data);
    }

    [TestMethod]
    public void TEST_Negation_005()
    {
        // ~~ double negation restores original sense
        var s = @"
        ~~integer('3')   :f(n)
        result = 'succeed'   :(end)
n       result = 'failure'
end
";
        var directives = "-b ";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("succeed", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("result")]).Data);
    }

    [TestMethod]
    public void TEST_Negation_006()
    {
        // ~ on ident: negates equality check
        var s = @"
        a = 'hello'
        b = 'world'
        ~ident(a, b)   :f(n)
        result = 'differ'  :(end)
n       result = 'same'
end
";
        var directives = "-b ";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        // ident('hello','world') fails -> ~ flips to success -> :f not taken
        Assert.AreEqual("differ", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("result")]).Data);
    }
}
