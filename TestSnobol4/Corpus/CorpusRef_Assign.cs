using Test.TestLexer;

// ReSharper disable StringLiteralTypo

namespace Test.Corpus;

/// <summary>
/// Corpus assign/ — basic variable assignment (009–016).
/// </summary>
[DoNotParallelize]
[TestClass]
public class CorpusRef_Assign
{
    [TestMethod]
    public void TEST_Corpus_009_assign_string()
    {
        var s = @"
        X = 'hello'
        OUTPUT = X
END";
        Assert.AreEqual("hello", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_010_assign_integer()
    {
        var s = @"
        N = 42
        OUTPUT = N
END";
        Assert.AreEqual("42", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_011_assign_chain()
    {
        var s = @"
        X = 'alpha'
        Y = X
        OUTPUT = Y
END";
        Assert.AreEqual("alpha", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_012_assign_null()
    {
        var s = @"
        X = 'something'
        X =
        OUTPUT = X
END";
        Assert.AreEqual("", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_013_assign_overwrite()
    {
        var s = @"
        X = 'first'
        X = 'second'
        OUTPUT = X
END";
        Assert.AreEqual("second", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_014_assign_indirect_dollar()
    {
        var s = @"
        $'X' = 'hello'
        OUTPUT = X
END";
        Assert.AreEqual("hello", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_015_assign_indirect_var()
    {
        var s = @"
        V = 'X'
        $V = 'world'
        OUTPUT = X
END";
        Assert.AreEqual("world", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_016_assign_to_output()
    {
        var s = @"
        OUTPUT = 'alpha'
        OUTPUT = 'beta'
END";
        Assert.AreEqual("alpha"+ Environment.NewLine + "beta", SetupTests.RunWithInput(s));
    }
}
