using Test.TestLexer;

// ReSharper disable StringLiteralTypo

namespace Test.Corpus;

/// <summary>
/// Corpus hello/ — minimal output and literal coercion tests.
/// All programs use OUTPUT =; output compared against .ref oracle files.
/// </summary>
[DoNotParallelize]
[TestClass]
public class CorpusRef_Hello
{
    [TestMethod]
    public void TEST_Corpus_hello_hello()
    {
        var s = @"
        OUTPUT = 'HELLO WORLD'
END";
        Assert.AreEqual("HELLO WORLD", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_hello_empty_string()
    {
        var s = @"
        OUTPUT = ''
END";
        Assert.AreEqual("", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_hello_multi()
    {
        var s = @"
        OUTPUT = 'LINE ONE'
        OUTPUT = 'LINE TWO'
        OUTPUT = 'LINE THREE'
END";
        Assert.AreEqual("LINE ONE\nLINE TWO\nLINE THREE", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_hello_literals()
    {
        var s = @"
START
      OUTPUT =
      OUTPUT = """"
      OUTPUT = ""Hello World!""
      OUTPUT = 0
      OUTPUT = 1
      OUTPUT = -1
      OUTPUT = 1.0
      OUTPUT = '1'
      OUTPUT = '1'
      OUTPUT = '1.0'
      OUTPUT = ""I'm here""
      OUTPUT = '""Quote of the day""'
      OUTPUT = '' + ''
      OUTPUT = '' + 1
      OUTPUT = 1 + ''
      OUTPUT = '' ''
      OUTPUT = '' 'Z'
      OUTPUT = 'A' ''
      OUTPUT = 'A' 'Z'
      OUTPUT = 1 + 2
      OUTPUT = 1 + 2 * 3
      OUTPUT = (1 + 2) * 3
      OUTPUT = 1 + (2 * 3)
END";
        var expected = "\n\nHello World!\n0\n1\n-1\n1.\n1\n1\n1.0\nI'm here\n\"Quote of the day\"\n0\n1\n1\n\nZ\nA\nAZ\n3\n7\n9\n7";
        Assert.AreEqual(expected, SetupTests.RunWithInput(s));
    }
}
