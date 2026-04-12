using Test.TestLexer;

// ReSharper disable StringLiteralTypo

namespace Test.Corpus;

/// <summary>
/// Corpus output/ — OUTPUT special variable, literal coercion (001–008).
/// </summary>
[DoNotParallelize]
[TestClass]
public class CorpusRef_Output
{
    [TestMethod]
    public void TEST_Corpus_001_output_string_literal()
    {
        var s = @"
        OUTPUT = 'hello world'
END";
        Assert.AreEqual("hello world", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_002_output_integer_literal()
    {
        var s = @"
        OUTPUT = 42
END";
        Assert.AreEqual("42", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_003_output_real_literal()
    {
        var s = @"
        OUTPUT = 3.14
END";
        Assert.AreEqual("3.14", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_004_output_empty_string()
    {
        var s = @"
        OUTPUT = ''
END";
        Assert.AreEqual("", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_005_output_multiline()
    {
        var s = @"
        OUTPUT = 'line one'
        OUTPUT = 'line two'
        OUTPUT = 'line three'
END";
        Assert.AreEqual("line one"+ Environment.NewLine + "line two"+ Environment.NewLine + "line three", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_006_output_keyword_alphabet_size()
    {
        var s = @"
        OUTPUT = SIZE(&ALPHABET)
END";
        Assert.AreEqual("256", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_007_output_null_var()
    {
        var s = @"
        X =
        OUTPUT = X
END";
        Assert.AreEqual("", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_008_output_double_quoted()
    {
        var s = @"
        OUTPUT = ""hello world""
END";
        Assert.AreEqual("hello world", SetupTests.RunWithInput(s));
    }
}
