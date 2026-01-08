namespace Test.TestLexer;

public partial class TestLexer
{
    [TestMethod]
    public void TEST_INTEGER_001()
    {
        var s = " 123";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s + ";end");
        Assert.AreEqual(1, build.Code.SourceLines[0].LexBody.Count);
        Assert.AreEqual(s[1..], build.Code.SourceLines[0].LexBody[0].MatchedString);
    }

    [TestMethod]
    public void TEST_REAL_001()
    {
        var s = " 123.123";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s + ";end");
        Assert.AreEqual(1, build.Code.SourceLines[0].LexBody.Count);
        Assert.AreEqual(s[1..], build.Code.SourceLines[0].LexBody[0].MatchedString);
    }

    [TestMethod]
    public void TEST_REAL_002()
    {
        var s = " 123e45";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s + ";end");
        Assert.AreEqual(1, build.Code.SourceLines[0].LexBody.Count);
        Assert.AreEqual(s[1..], build.Code.SourceLines[0].LexBody[0].MatchedString);
    }

    [TestMethod]
    public void TEST_REAL_003()
    {
        var s = " 123e+45";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s + ";end");
        Assert.AreEqual(1, build.Code.SourceLines[0].LexBody.Count);
        Assert.AreEqual(s[1..], build.Code.SourceLines[0].LexBody[0].MatchedString);
    }

    [TestMethod]
    public void TEST_REAL_004()
    {
        var s = " 123e-45";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s + ";end");
        Assert.AreEqual(1, build.Code.SourceLines[0].LexBody.Count);
        Assert.AreEqual(s[1..], build.Code.SourceLines[0].LexBody[0].MatchedString);
    }

    [TestMethod]
    public void TEST_REAL_005()
    {
        var s = " 123.e45";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s + ";end");
        Assert.AreEqual(1, build.Code.SourceLines[0].LexBody.Count);
        Assert.AreEqual(s[1..], build.Code.SourceLines[0].LexBody[0].MatchedString);
    }

    [TestMethod]
    public void TEST_REAL_006()
    {
        var s = " 123.e+45";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s + ";end");
        Assert.AreEqual(1, build.Code.SourceLines[0].LexBody.Count);
        Assert.AreEqual(s[1..], build.Code.SourceLines[0].LexBody[0].MatchedString);
    }

    [TestMethod]
    public void TEST_REAL_007()
    {
        var s = " 123.e-45";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s + ";end");
        Assert.AreEqual(1, build.Code.SourceLines[0].LexBody.Count);
        Assert.AreEqual(s[1..], build.Code.SourceLines[0].LexBody[0].MatchedString);
    }

    [TestMethod]
    public void TEST_REAL_008()
    {
        var s = " 123.456e78";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s + ";end");
        Assert.AreEqual(1, build.Code.SourceLines[0].LexBody.Count);
        Assert.AreEqual(s[1..], build.Code.SourceLines[0].LexBody[0].MatchedString);
    }

    [TestMethod]
    public void TEST_REAL_009()
    {
        var s = " 123.456e+78";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s + ";end");
        Assert.AreEqual(1, build.Code.SourceLines[0].LexBody.Count);
        Assert.AreEqual(s[1..], build.Code.SourceLines[0].LexBody[0].MatchedString);
    }

    [TestMethod]
    public void TEST_REAL_010()
    {
        var s = " 123.456e-78";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s + ";end");
        Assert.AreEqual(1, build.Code.SourceLines[0].LexBody.Count);
        Assert.AreEqual(s[1..], build.Code.SourceLines[0].LexBody[0].MatchedString);
    }

}
