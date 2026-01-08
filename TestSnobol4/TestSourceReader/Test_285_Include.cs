using Test.TestLexer;

namespace Test.TestSourceReader;

public partial class TestSourceReader
{
    [TestMethod]
    public void TEST_285_001()
    {
        var s = @"
-include 'WTF'
end
";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreNotEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual(285, build.ErrorCodeHistory[0]);
    }
}
