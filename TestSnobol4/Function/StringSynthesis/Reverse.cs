using Snobol4.Common;
using Test.TestLexer;

namespace Test.StringSynthesis;

[TestClass]
public class Reverse
{
    //"reverse argument is not a string" /* 177 */,

    [TestMethod]
    public void TEST_Reverse_001()
    {
        var s = @"
        a = 'this is a test'
        b = reverse(a)
end
";

        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("tset a si siht", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("b")]).Data);
    }

    [TestMethod]
    public void TEST_Reverse_003_empty()
    {
        var s = @"
        a = ''
        b = reverse(a)
end
";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("b")]).Data);
    }

    [TestMethod]
    public void TEST_Reverse_004_single_char()
    {
        var s = @"
        b = reverse('x')
end
";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("x", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("b")]).Data);
    }

    [TestMethod]
    public void TEST_Reverse_002()
    {
        var s = @"
        a = any('this is a test')
        b = reverse(a)
end
";

        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreNotEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual(177, build.ErrorCodeHistory[0]);
    }
    [TestMethod]
    public void TEST_Reverse_003_single_char()
    {
        var s = @"
        r = reverse('x')
        differ(r, 'x')   :s(bad)
        result = 'ok'    :(end)
bad     result = 'bad'
end";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("ok", build.Execute!.IdentifierTable[build.FoldCase("result")].ToString());
    }

    [TestMethod]
    public void TEST_Reverse_004_twice_identity()
    {
        var s = @"
        s = 'abcde'
        r = reverse(reverse(s))
        differ(r, s)   :s(bad)
        result = 'ok'  :(end)
bad     result = 'bad'
end";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("ok", build.Execute!.IdentifierTable[build.FoldCase("result")].ToString());
    }
}
