using Test.TestLexer;

namespace Test.Miscellaneous;

//"size argument is not a string" /* 189 */,



[TestClass]
public class Size
{
    [TestMethod]
    public void TEST_Size_001()
    {
        var s = @"
        r = size('123456')
end";

        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("6", build.Execute!.IdentifierTable[build.FoldCase("r")].ToString());
    }

    [TestMethod]
    public void TEST_Size_002()
    {
        var s = @"
        r = size(123456)
end";

        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("6", build.Execute!.IdentifierTable[build.FoldCase("r")].ToString());
    }

    [TestMethod]
    public void TEST_Size_004_empty_string()
    {
        var s = @"
        r = size('')
end";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("0", build.Execute!.IdentifierTable[build.FoldCase("r")].ToString());
    }

    [TestMethod]
    public void TEST_Size_005_single_char()
    {
        var s = @"
        r = size('x')
end";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("1", build.Execute!.IdentifierTable[build.FoldCase("r")].ToString());
    }

    [TestMethod]
    public void TEST_Size_003()
    {
        var s = @"

        T = TABLE(3)
        R = SIZE(T)
end";

        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(1, build.ErrorCodeHistory.Count);
        Assert.AreEqual(189, build.ErrorCodeHistory[0]);
    }

    [TestMethod]
    public void TEST_Size_006_integer_convert()
    {
        // SIZE of integer: converts to string first
        var s = @"
        r = size(0)
end";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("1", build.Execute!.IdentifierTable[build.FoldCase("r")].ToString());
    }

    [TestMethod]
    public void TEST_Size_007_after_reverse()
    {
        var s = @"
        r = size(reverse('hello'))
end";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("5", build.Execute!.IdentifierTable[build.FoldCase("r")].ToString());
    }

    [TestMethod]
    public void TEST_Size_008_after_dupl()
    {
        var s = @"
        r = size(dupl('ab', 3))
end";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("6", build.Execute!.IdentifierTable[build.FoldCase("r")].ToString());
    }

}
