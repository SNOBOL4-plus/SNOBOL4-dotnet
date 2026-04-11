using Snobol4.Common;
using Test.TestLexer;

namespace Test.Pattern;

[TestClass]
public class At
{

    [TestMethod]
    public void TEST_At_001()
    {
        var s = @"
        &anchor = 0
        'valley' 'a' @at1 arb 'e' @at2  :s(y)f(n)
y       result = 'success'   :(end)
n       result = 'fail' 
end";

        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual("success", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("result")]).Data);
        Assert.AreEqual(2, ((IntegerVar)build.Execute!.IdentifierTable[build.FoldCase("at1")]).Data);
        Assert.AreEqual(5, ((IntegerVar)build.Execute!.IdentifierTable[build.FoldCase("at2")]).Data);
    }
    [TestMethod]

    public void TEST_At_002()
    {
        var s = @"
        &anchor = 0
        'DOUBT' @at1 'B'  :s(y)f(n)
y       result = 'success'   :(end)
n       result = 'fail' 
end";

        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual("success", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("result")]).Data);
        Assert.AreEqual(3, ((IntegerVar)build.Execute!.IdentifierTable[build.FoldCase("at1")]).Data);
    }

    [TestMethod]
    public void TEST_At_003()
    {
        var s = @"
        &anchor = 0
        'FIX' @at1 'B'  :s(y)f(n)
y       result = 'success'   :(end)
n       result = 'fail' 
end";

        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual("fail", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("result")]).Data);
    }
    [TestMethod]
    public void TEST_At_004()
    {
        // @var captures cursor position — after LEN(2) it should be 2
        var s = @"
        subject = 'abcdef'
        subject len(2) @pos   :f(bad)
        result = pos   :(end)
bad     result = 'bad'
end";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual(2L, ((IntegerVar)build.Execute!.IdentifierTable[build.FoldCase("pos")]).Data);
    }

    [TestMethod]
    public void TEST_At_005()
    {
        // @var at start of match = 0
        var s = @"
        subject = 'hello'
        &anchor = 1
        subject @pos 'hello'   :f(bad)
        result = pos   :(end)
bad     result = 'bad'
end";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual(0L, ((IntegerVar)build.Execute!.IdentifierTable[build.FoldCase("pos")]).Data);
    }
}
