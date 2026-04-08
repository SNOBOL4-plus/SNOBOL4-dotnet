using Test.TestLexer;

// ReSharper disable StringLiteralTypo

namespace Test.Corpus;

/// <summary>
/// Corpus concat/ — string concatenation (017–022).
/// </summary>
[DoNotParallelize]
[TestClass]
public class CorpusRef_Concat
{
    [TestMethod]
    public void TEST_Corpus_017_concat_two_strings()
    {
        var s = @"
        OUTPUT = 'hello' ' world'
END";
        Assert.AreEqual("hello world", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_018_concat_three_strings()
    {
        var s = @"
        OUTPUT = 'a' 'b' 'c'
END";
        Assert.AreEqual("abc", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_019_concat_var_string()
    {
        var s = @"
        X = 'hello'
        OUTPUT = X ' world'
END";
        Assert.AreEqual("hello world", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_020_concat_integer_string()
    {
        var s = @"
        OUTPUT = 42 ' items'
END";
        Assert.AreEqual("42 items", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_021_concat_in_assignment()
    {
        var s = @"
        X = 'foo' 'bar'
        OUTPUT = X
END";
        Assert.AreEqual("foobar", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_022_concat_multipart()
    {
        var s = @"
        OUTPUT = 'a' 'b' 'c' 'd'
END";
        Assert.AreEqual("abcd", SetupTests.RunWithInput(s));
    }
}
