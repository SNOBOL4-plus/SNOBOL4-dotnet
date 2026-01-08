namespace Test.TestLexer;

public partial class TestLexer
{
    [TestMethod]
    public void TEST_BRACKET_001()
    {
        var s = "   123.45e67<0>";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s + ";end");
        Assert.AreNotEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual(235, build.ErrorCodeHistory[0]);
        Assert.AreEqual(4, build.Code.SourceLines[0].LexBody.Count);
        Assert.AreEqual(s[3..12], build.Code.SourceLines[0].LexBody[0].MatchedString);
    }

    [TestMethod]
    public void TEST_BRACKET_002()
    {
        var s = "   (123.45e67)<0>";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s + ";end");
        Assert.AreNotEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual(235, build.ErrorCodeHistory[0]);
        Assert.AreEqual(6, build.Code.SourceLines[0].LexBody.Count);
        Assert.AreEqual(s[4..13], build.Code.SourceLines[0].LexBody[1].MatchedString);
    }

    [TestMethod]
    public void TEST_BRACKET_003()
    {
        var s = "   12[0]";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s + ";end");
        Assert.AreNotEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual(235, build.ErrorCodeHistory[0]);
        Assert.AreEqual(4, build.Code.SourceLines[0].LexBody.Count);
        Assert.AreEqual(s[3..5], build.Code.SourceLines[0].LexBody[0].MatchedString);
    }

    [TestMethod]
    public void TEST_BRACKET_004()
    {
        var s = "   (12)[0]";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s + ";end");
        Assert.AreNotEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual(235, build.ErrorCodeHistory[0]);
        Assert.AreEqual(6, build.Code.SourceLines[0].LexBody.Count);
        Assert.AreEqual(s[4..6], build.Code.SourceLines[0].LexBody[1].MatchedString);
    }

    [TestMethod]
    public void TEST_BRACKET_005()
    {
        var s = "   'abc'[0]";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s + ";end");
        Assert.AreNotEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual(235, build.ErrorCodeHistory[0]);
        Assert.AreEqual(4, build.Code.SourceLines[0].LexBody.Count);
        Assert.AreEqual(s[4..7], build.Code.SourceLines[0].LexBody[0].MatchedString);
    }

    [TestMethod]
    public void TEST_BRACKET_006()
    {
        var s = "   ('abc')[0]";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s + ";end");
        Assert.AreNotEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual(235, build.ErrorCodeHistory[0]);
        Assert.AreEqual(6, build.Code.SourceLines[0].LexBody.Count);
        Assert.AreEqual(s[5..8], build.Code.SourceLines[0].LexBody[1].MatchedString);
    }

    [TestMethod]
    public void TEST_BRACKET_007()
    {

        var s = "   123.45e67[0]";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s + ";end");
        Assert.AreNotEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual(235, build.ErrorCodeHistory[0]);
        Assert.AreEqual(4, build.Code.SourceLines[0].LexBody.Count);
        Assert.AreEqual(s[3..12], build.Code.SourceLines[0].LexBody[0].MatchedString);
    }

    [TestMethod]
    public void TEST_BRACKET_008()
    {
        var s = "   (123.45e67)[0]";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s + ";end");
        Assert.AreNotEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual(235, build.ErrorCodeHistory[0]);
        Assert.AreEqual(6, build.Code.SourceLines[0].LexBody.Count);
        Assert.AreEqual(s[4..13], build.Code.SourceLines[0].LexBody[1].MatchedString);
    }

    [TestMethod]
    public void TEST_BRACKET_009()
    {
        var s = "   12<0>";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s + ";end");
        Assert.AreNotEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual(235, build.ErrorCodeHistory[0]);
        Assert.AreEqual(4, build.Code.SourceLines[0].LexBody.Count);
        Assert.AreEqual(s[3..5], build.Code.SourceLines[0].LexBody[0].MatchedString);
    }

    [TestMethod]
    public void TEST_BRACKET_010()
    {
        var s = "   (12)<0>";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s + ";end");
        Assert.AreNotEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual(235, build.ErrorCodeHistory[0]);
        Assert.AreEqual(6, build.Code.SourceLines[0].LexBody.Count);
        Assert.AreEqual(s[4..6], build.Code.SourceLines[0].LexBody[1].MatchedString);
    }

    [TestMethod]
    public void TEST_BRACKET_011()
    {
        var s = "   'abc'<0>";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s + ";end");
        Assert.AreNotEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual(235, build.ErrorCodeHistory[0]);
        Assert.AreEqual(4, build.Code.SourceLines[0].LexBody.Count);
        Assert.AreEqual(s[4..7], build.Code.SourceLines[0].LexBody[0].MatchedString);
    }

    [TestMethod]
    public void TEST_BRACKET_012()
    {
        var s = "   ('abc')<0>";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s + ";end");
        Assert.AreNotEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual(235, build.ErrorCodeHistory[0]);
        Assert.AreEqual(6, build.Code.SourceLines[0].LexBody.Count);
        Assert.AreEqual(s[5..8], build.Code.SourceLines[0].LexBody[1].MatchedString);
    }
}
