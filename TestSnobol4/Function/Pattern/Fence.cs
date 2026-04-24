using Snobol4.Common;
using Test.TestLexer;

namespace Test.Pattern;

[TestClass]
public class Fence
{

    [TestMethod]
    public void TEST_Abort_001()
    {
        var s = @"
        P = FENCE(BREAK(',') | REM) $ STR *DIFFER(STR)
        'ABC' P . R1
        '123,,456' P . R2
end
";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual("ABC", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("R1")]).Data);
        Assert.AreEqual("123", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("R2")]).Data);
    }
    
    [TestMethod]
    public void TEST_Abort_002()
    {
        var s = @"
	    '1AB+' ANY('AB') FENCE('+') :S(Y)F(N)
Y	    R1 = 'SUCCESS' :(end)
N	    R1 = 'FAILURE'
end
";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual("SUCCESS", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("R1")]).Data);
    }

    [TestMethod]
    public void TEST_Abort_003()
    {
        var s = @"
        '1AB+' ANY('AB') FENCE '+' :S(Y)F(N)
Y	    R1 = 'SUCCESS' :(end)
N	    R1 = 'FAILURE'
end
";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual("FAILURE", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("R1")]).Data);
    }

    [TestMethod]
    public void TEST_Abort_004()
    {
        var s = @"
	    'ABC'  FENCE('B') :S(Y)F(N)
Y	    R1 = 'SUCCESS' :(end)
N	    R1 = 'FAILURE'
end
";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual("SUCCESS", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("R1")]).Data);
    }

    [TestMethod]
    public void TEST_Abort_005()
    {
        var s = @"
	    'ABC'  FENCE 'B' :S(Y)F(N)
Y	    R1 = 'SUCCESS' :(end)
N	    R1 = 'FAILURE'
end
";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual("FAILURE", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("R1")]).Data);
    }

    [TestMethod]
    public void TEST_Abort_006()
    {
        var s = @"
        B = *'B'
	    'ABC' FENCE(B) :S(Y)F(N)
Y	    R1 = 'SUCCESS' :(end)
N	    R1 = 'FAILURE'
end
";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual("SUCCESS", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("R1")]).Data);
    }

    [TestMethod]
    public void TEST_Fence_007_nullary_blocks_backtrack()
    {
        // Nullary FENCE: once position passes FENCE, no backtrack past it
        var s = @"
        &anchor = 0
        subject = 'abc'
        subject ('a' fence 'x')   :s(bad)f(ok)
bad     result = 'bad'   :(end)
ok      result = 'ok'
end";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("ok", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("result")]).Data);
    }

    [TestMethod]
    public void TEST_Fence_008_unary_inner_match()
    {
        // Unary FENCE(p): if p matches, commit — no backtrack into p
        var s = @"
        &anchor = 0
        subject = 'abc'
        subject fence('abc') . cap   :s(ok)f(bad)
ok      result = cap   :(end)
bad     result = 'bad'
end";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("abc", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("result")]).Data);
    }

}
