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
    [TestMethod]
    public void TEST_Concat_005_three_parts()
    {
        // Three-way concatenation matches all three in sequence
        var s = @"
        subject = 'abc123xyz'
        subject (len(3) . a) (len(3) . b) (len(3) . c)   :f(bad)
        result = a ' ' b ' ' c   :(end)
bad     result = 'bad'
end";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("abc 123 xyz",
            ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("result")]).Data);
    }

    [TestMethod]
    public void TEST_Concat_006_order_matters()
    {
        // Concatenation is ordered: 'ab' 'cd' matches 'abcd' not 'cdab'
        var s = @"
        subject = 'abcd'
        subject 'ab' 'cd'   :s(ok)f(bad)
ok      result = 'ok'   :(end)
bad     result = 'bad'
end";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("ok",
            ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("result")]).Data);
    }

    [TestMethod]
    public void TEST_Concatenate_007_three_literals()
    {
        // Three-way concatenation: 'a' 'b' 'c' — the . capture after 'c' captures only 'c'
        // SPITBOL confirmed: 'a' 'b' 'c' . cap captures 'c' (last element only)
        var s = @"
        subject = 'abcXYZ'
        subject 'a' 'b' 'c' . cap   :f(bad)
        result = cap   :(end)
bad     result = 'bad'
end";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("c", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("result")]).Data);
    }

    [TestMethod]
    public void TEST_Concatenate_008_with_span()
    {
        // Concatenation of literal and SPAN — . capture after SPAN captures only SPAN's match
        // SPITBOL confirmed: 'x' span('0123456789') . digits captures '123' not 'x123'
        var s = @"
        subject = 'x123y'
        subject 'x' span('0123456789') . digits   :f(bad)
        result = digits   :(end)
bad     result = 'bad'
end";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("123", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("result")]).Data);
    }

}
