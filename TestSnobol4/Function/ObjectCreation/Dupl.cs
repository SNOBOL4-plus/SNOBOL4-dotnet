using Snobol4.Common;
using Test.TestLexer;

namespace Test.ObjectCreation;

[TestClass]
public class Dupl
{
    [TestMethod]
    public void TEST_Dupl_001()
    {
        var s = @"
        R = DUPL('123', 3)
end";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("123123123", build.Execute!.IdentifierTable[build.FoldCase("R")].ToString());
    }

    [TestMethod]
    public void TEST_Dupl_003_zero_count()
    {
        // DUPL with count 0 → empty string
        var s = @"
        R = DUPL('abc', 0)
end";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("", build.Execute!.IdentifierTable[build.FoldCase("R")].ToString());
    }

    [TestMethod]
    public void TEST_Dupl_004_single()
    {
        // DUPL with count 1 → original string
        var s = @"
        R = DUPL('hello', 1)
end";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("hello", build.Execute!.IdentifierTable[build.FoldCase("R")].ToString());
    }

    [TestMethod]
    public void TEST_Dupl_002()
    {
        var s = @"
        P = 'A' | '1'
        R = DUPL(P, 5)
        '11111' R . R1
        '1A1A1' R . R2
        'AAAAA' R . R3
end";

        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("11111", build.Execute!.IdentifierTable[build.FoldCase("R1")].ToString());
        Assert.AreEqual("1A1A1", build.Execute!.IdentifierTable[build.FoldCase("R2")].ToString());
        Assert.AreEqual("AAAAA", build.Execute!.IdentifierTable[build.FoldCase("R3")].ToString());
    }

    [TestMethod]
    public void TEST_Dupl_005_large()
    {
        // DUPL with large count
        var s = @"
        R = DUPL('ab', 5)
end";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("ababababab", build.Execute!.IdentifierTable[build.FoldCase("R")].ToString());
    }

    [TestMethod]
    public void TEST_Dupl_006_size()
    {
        // DUPL result length = len(str) * count
        var s = @"
        R = DUPL('xyz', 4)
        S = SIZE(R)
end";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("xyzxyzxyzxyz", build.Execute!.IdentifierTable[build.FoldCase("R")].ToString());
        Assert.AreEqual(12L, ((IntegerVar)build.Execute!.IdentifierTable[build.FoldCase("S")]).Data);
    }

    [TestMethod]
    public void TEST_Dupl_007_single_char()
    {
        // DUPL of single char is a repeat string
        var s = @"
        R = DUPL('*', 6)
end";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("******", build.Execute!.IdentifierTable[build.FoldCase("R")].ToString());
    }
}