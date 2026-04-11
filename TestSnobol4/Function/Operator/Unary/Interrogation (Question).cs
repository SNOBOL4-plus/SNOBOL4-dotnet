using Snobol4.Common;
using Test.TestLexer;

namespace Test.Operator;

[TestClass]
public class Interrogation
{
    [TestMethod]
    public void TEST_Interrogation_001()
    {
        var s = @"
        S = 'this'
        P = 'is'
        N = 123
        N = ?(S ? P) N + 1
end";
        var directives = "-b ";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual(124, ((IntegerVar)build.Execute!.IdentifierTable[build.FoldCase("N")]).Data);
    }

    [TestMethod]
    public void TEST_Interrogation_002()
    {
        var s = @"
	    int = ?integer('a')   :f(n)
	    results = 'succeed'  :(end)
n	    results = 'failure'
end";
        var directives = "-b ";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("failure", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("results")]).Data);
        Assert.AreEqual("", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("int")]).Data);
    }

    [TestMethod]
    public void TEST_Interrogation_00e()
    {
        var s = @"
	    int = ?integer('42')   :f(n)
	    results = 'succeed'  :(end)
n	    results = 'failure'
end";
        var directives = "-b ";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("succeed", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("results")]).Data);
        Assert.AreEqual("", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("int")]).Data);
    }
    [TestMethod]
    public void TEST_Interrogation_003()
    {
        // ? on a failing built-in: result is null, branch taken on success
        var s = @"
        result = ?differ('abc', 'abc')   :s(succeeded)
        result = 'null-result'           :(end)
succeeded
        result = 'should-not-reach'
end";
        var directives = "-b ";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("null-result", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("result")]).Data);
    }

    [TestMethod]
    public void TEST_Interrogation_004()
    {
        // ? on a succeeding built-in: preserves the success/failure signal
        var s = @"
        result = ?differ('abc', 'xyz')   :f(failed)
        result = 'success-path'          :(end)
failed
        result = 'failure-path'
end";
        var directives = "-b ";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("success-path", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("result")]).Data);
    }

    [TestMethod]
    public void TEST_Interrogation_005()
    {
        // ? suppresses the return value: even on success, result is null/unset
        var s = @"
        x = ?integer('42')   :f(n)
        result = 'success'   :(end)
n       result = 'failure'
end";
        var directives = "-b ";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        // integer('42') succeeds, ? succeeds (branches :f not taken), x stays empty
        Assert.AreEqual("success", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("result")]).Data);
        Assert.AreEqual("", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("x")]).Data);
    }
}
