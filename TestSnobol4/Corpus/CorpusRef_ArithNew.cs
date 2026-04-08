using Test.TestLexer;

// ReSharper disable StringLiteralTypo

namespace Test.Corpus;

/// <summary>
/// Corpus arith_new/ — arithmetic operators (023–030).
/// </summary>
[DoNotParallelize]
[TestClass]
public class CorpusRef_ArithNew
{
    [TestMethod]
    public void TEST_Corpus_023_arith_add()
    {
        var s = @"
        OUTPUT = 1 + 2
END";
        Assert.AreEqual("3", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_024_arith_subtract()
    {
        var s = @"
        OUTPUT = 10 - 3
END";
        Assert.AreEqual("7", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_025_arith_multiply()
    {
        var s = @"
        OUTPUT = 6 * 7
END";
        Assert.AreEqual("42", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_026_arith_divide()
    {
        // Integer / integer = integer (truncates toward zero)
        var s = @"
        OUTPUT = 10 / 4
END";
        Assert.AreEqual("2", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_027_arith_exponent()
    {
        var s = @"
        OUTPUT = 2 ** 8
END";
        Assert.AreEqual("256", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_028_arith_unary_minus()
    {
        var s = @"
        OUTPUT = -5
END";
        Assert.AreEqual("-5", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_029_arith_precedence()
    {
        // 2 + 3 * 4 = 14  (multiply binds tighter)
        var s = @"
        OUTPUT = 2 + 3 * 4
END";
        Assert.AreEqual("14", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_030_arith_remdr()
    {
        var s = @"
        OUTPUT = REMDR(10, 3)
END";
        Assert.AreEqual("1", SetupTests.RunWithInput(s));
    }
}
