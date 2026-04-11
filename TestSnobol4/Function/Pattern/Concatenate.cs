using Snobol4.Common;
using Test.TestLexer;

// ReSharper disable StringLiteralTypo

namespace Test.Pattern;

[TestClass]
public class Concatenate
{

    [TestMethod]
    public void TEST_Concatenate_001()
    {
        var s = @"
        &anchor = 0
        ss = 'PLEASEHELPMEOUT'
        p1 = 'PLEASE'
        p2 = 'HELP'
        p3 = 'ME'
        p4 = 'OUT'
        p = p2 p3
end";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual("HELPME", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("p")]).Data);
    }

    [TestMethod]
    public void TEST_Concatenate_003_three_way()
    {
        // Three-pattern concatenation — all three must match in sequence
        var s = @"
        &anchor = 0
        subject = 'abcdef'
        p = 'ab' 'cd' 'ef'
        subject p :f(n)
y       result = 'success'   :(end)
n       result = 'fail'
end";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual("success",
            ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("result")]).Data);
    }

    [TestMethod]
    public void TEST_Concatenate_004_partial_fail()
    {
        // Second component fails — whole concat fails
        var s = @"
        &anchor = 1
        subject = 'abXX'
        p = 'ab' 'cd'
        subject p :f(n)
y       result = 'wrong'    :(end)
n       result = 'correct'
end";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual("correct",
            ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("result")]).Data);
    }

    [TestMethod]
    public void TEST_Concatenate_002()
    {
        var s = @"
        &anchor = 0
        ss = 'PLEASEHELPMEOUT'
        p1 = 'PLEASE'
        p2 = 'HELP'
        p3 = 'ME'
        p4 = 'OUT'
        p = p2 p3
        ss p		:f(n)
        r = 'success' :(end)
n		r = 'fail'  :(end)

end";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual("HELPME", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("p")]).Data);
        Assert.AreEqual("success", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("r")]).Data);
    }

}