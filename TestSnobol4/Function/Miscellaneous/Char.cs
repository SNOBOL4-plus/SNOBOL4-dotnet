using Snobol4.Common;
using Test.TestLexer;

namespace Test.Miscellaneous;

[TestClass]
public class Char
{
    [TestMethod]
    public void TEST_Char_1()
    {
        var s = @"
        b = char(array(20))
end";

        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreNotEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual(281, build.ErrorCodeHistory[0]);
    }

    [TestMethod]
    public void TEST_Char_2()
    {
        var s = @"
        b = char(-1)
end";

        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreNotEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual(282, build.ErrorCodeHistory[0]);
    }

    [TestMethod]
    public void TEST_Char_3()
    {
        var s = @"
        b = char(32768)
end";

        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreNotEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual(282, build.ErrorCodeHistory[0]);
    }

    [TestMethod]
    public void TEST_Char_5_ascii_space()
    {
        // CHAR(32) = space
        var s = @"
        b = char(32)
end";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual(" ", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("b")]).Data);
    }

    [TestMethod]
    public void TEST_Char_6_ascii_A()
    {
        // CHAR(65) = 'A'
        var s = @"
        b = char(65)
end";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("A", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("b")]).Data);
    }

    [TestMethod]
    public void TEST_Char_7_zero()
    {
        // CHAR(0) = null character — size is 1
        var s = @"
        b = char(0)
        r = size(b)
end";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("1", build.Execute!.IdentifierTable[build.FoldCase("r")].ToString());
    }

    [TestMethod]
    public void TEST_Char_4()
    {
        var s = @"
        b = char(32767)
end";

        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("翿", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("b")]).Data);
    }

    [TestMethod]
    public void TEST_Char_8_size_is_one()
    {
        // CHAR produces a one-character string for any valid code point
        var s = @"
        r = size(char(72))
end";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("1", build.Execute!.IdentifierTable[build.FoldCase("r")].ToString());
    }

}
