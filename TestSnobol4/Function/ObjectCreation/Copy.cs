using Test.TestLexer;

namespace Test.ObjectCreation;

[TestClass]
public class Copy
{
    [TestMethod]
    public void TEST_Copy_001()
    {
        var s = @"
        A = '123456'
        B = COPY(A)
end";

        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreNotEqual(build.Execute!.IdentifierTable[build.FoldCase("a")].SequenceId, build.Execute!.IdentifierTable[build.FoldCase("b")].SequenceId);
    }

    [TestMethod]
    public void TEST_Copy_002()
    {
        var s = @"
        A = 123456
        B = COPY(A)
end";

        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreNotEqual(build.Execute!.IdentifierTable[build.FoldCase("a")].SequenceId, build.Execute!.IdentifierTable[build.FoldCase("b")].SequenceId);
    }

    [TestMethod]
    public void TEST_Copy_003()
    {
        var s = @"
        A = 'a' | 'b'
        B = COPY(A)
end";

        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreNotEqual(build.Execute!.IdentifierTable[build.FoldCase("a")].SequenceId, build.Execute!.IdentifierTable[build.FoldCase("b")].SequenceId);
    }
    [TestMethod]
    public void TEST_Copy_004_value_matches()
    {
        // COPY produces a value equal to original
        var s = @"
        A = 'hello'
        B = COPY(A)
        differ(A, B)   :s(bad)
        result = 'ok'  :(end)
bad     result = 'bad'
end";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("ok", build.Execute!.IdentifierTable[build.FoldCase("result")].ToString());
    }

    [TestMethod]
    public void TEST_Copy_005_null()
    {
        // COPY of uninitialized (null) variable
        var s = @"
        B = COPY(A)
        differ(B, '')   :s(bad)
        result = 'ok'   :(end)
bad     result = 'bad'
end";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("ok", build.Execute!.IdentifierTable[build.FoldCase("result")].ToString());
    }

    [TestMethod]
    public void TEST_Copy_006_integer()
    {
        // COPY of integer preserves value
        var s = @"
        A = 42
        B = COPY(A)
        differ(A, B)   :s(bad)
        result = 'ok'  :(end)
bad     result = 'bad'
end";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("ok", build.Execute!.IdentifierTable[build.FoldCase("result")].ToString());
    }
}
