using Test.TestLexer;

namespace Test.Memory;

[TestClass]
public class Dump
{
    [TestMethod]
    public void TEST_Dump_001()
    {
        var s = @"

        I = 10;
        S = 'STRING'
        T = TABLE(10)
        T<1> = 20
        T<2> = 'string'

        DUMP(1)
end";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
    }

    [TestMethod]
    public void TEST_Dump_002()
    {
        var s = @"

        I = 10;
        S = 'STRING'
        T = TABLE(10)
        T<1> = 20
        T<2> = 'string'

        DUMP(2)
end";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
    }

    [TestMethod]
    public void TEST_Dump_003()
    {
        var s = @"

        I = 10;
        S = 'STRING'
        T = TABLE(10)
        T<1> = 20
        T<2> = 'string'

        DUMP(3)
end";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
    }
    [TestMethod]
    public void TEST_Dump_004_zero()
    {
        // DUMP(0) is a no-op / suppressed dump
        var s = @"
        I = 99
        DUMP(0)
        result = 'ok'
end";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("ok", build.Execute!.IdentifierTable[build.FoldCase("result")].ToString());
    }

    [TestMethod]
    public void TEST_Dump_005_with_array()
    {
        // DUMP works when arrays are in scope
        var s = @"
        A = ARRAY(5)
        A[1] = 'first'
        A[5] = 'last'
        DUMP(1)
        result = 'ok'
end";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("ok", build.Execute!.IdentifierTable[build.FoldCase("result")].ToString());
    }
}
