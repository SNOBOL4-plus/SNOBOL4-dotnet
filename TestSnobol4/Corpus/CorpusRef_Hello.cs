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
        Assert.AreEqual("LINE ONE"+ Environment.NewLine + "LINE TWO"+ Environment.NewLine + "LINE THREE", SetupTests.RunWithInput(s));
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
        var expected = ""+ Environment.NewLine + ""+ Environment.NewLine + "Hello World!"+ Environment.NewLine + "0"+ Environment.NewLine + "1"+ Environment.NewLine + "-1"+ Environment.NewLine + "1."+ Environment.NewLine + "1"+ Environment.NewLine + "1"+ Environment.NewLine + "1.0"+ Environment.NewLine + "I'm here"+ Environment.NewLine + "\"Quote of the day\""+ Environment.NewLine + "0"+ Environment.NewLine + "1"+ Environment.NewLine + "1"+ Environment.NewLine + ""+ Environment.NewLine + "Z"+ Environment.NewLine + "A"+ Environment.NewLine + "AZ"+ Environment.NewLine + "3"+ Environment.NewLine + "7"+ Environment.NewLine + "9"+ Environment.NewLine + "7";
        Assert.AreEqual(expected, SetupTests.RunWithInput(s));
    }
    [TestMethod]
    public void TEST_Hello_005_concat_in_output()
    {
        var s = @"
      OUTPUT = 'Hello' ', ' 'World'
END";
        Assert.AreEqual("Hello, World", SetupTests.RunWithInput(s).Trim());
    }

    [TestMethod]
    public void TEST_Hello_006_arithmetic_precedence()
    {
        var s = @"
      OUTPUT = 2 + 3 * 4
      OUTPUT = (2 + 3) * 4
END";
        var result = SetupTests.RunWithInput(s).Trim();
        var lines = result.Split('\n');
        Assert.AreEqual("14", lines[0].Trim());
        Assert.AreEqual("20", lines[1].Trim());
    }
}
