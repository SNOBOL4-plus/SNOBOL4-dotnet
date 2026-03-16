using Snobol4.Common;
using Test.TestLexer;

// ReSharper disable StringLiteralTypo

namespace Test.Corpus;

/// <summary>
/// Corpus strings/ — builtin string functions.
/// </summary>
[TestClass]
public class SimpleOutput_Strings
{
    private static Builder Run(string script) =>
        SetupTests.SetupScript("-b", script);

    [TestMethod]
    public void TEST_Corpus_065_builtin_size()
    {
        var s = @"
        RESULT = SIZE('hello')
end";
        var build = Run(s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual(5L, ((IntegerVar)build.Execute!.IdentifierTable[build.FoldCase("RESULT")]).Data);
    }

    [TestMethod]
    public void TEST_Corpus_066_builtin_substr()
    {
        var s = @"
        RESULT = SUBSTR('hello world', 7, 5)
end";
        var build = Run(s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("world", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("RESULT")]).Data);
    }

    [TestMethod]
    public void TEST_Corpus_067_builtin_replace()
    {
        var s = @"
        RESULT = REPLACE('hello', 'aeiou', 'AEIOU')
end";
        var build = Run(s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("hEllO", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("RESULT")]).Data);
    }

    [TestMethod]
    public void TEST_Corpus_068_builtin_trim()
    {
        var s = @"
        RESULT = SIZE(TRIM('hello   '))
end";
        var build = Run(s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual(5L, ((IntegerVar)build.Execute!.IdentifierTable[build.FoldCase("RESULT")]).Data);
    }

    [TestMethod]
    public void TEST_Corpus_069_builtin_dupl()
    {
        var s = @"
        RESULT = DUPL('ab', 3)
end";
        var build = Run(s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("ababab", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("RESULT")]).Data);
    }

    [TestMethod]
    public void TEST_Corpus_070_builtin_reverse()
    {
        var s = @"
        RESULT = REVERSE('hello')
end";
        var build = Run(s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("olleh", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("RESULT")]).Data);
    }

    [TestMethod]
    public void TEST_Corpus_071_builtin_ucase()
    {
        var s = @"
        RESULT = SIZE(&UCASE)
end";
        var build = Run(s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        // DOTNET &UCASE may include extended characters; assert >= 26
        var result = ((IntegerVar)build.Execute!.IdentifierTable[build.FoldCase("RESULT")]).Data;
        Assert.IsTrue(result >= 26L, $"&UCASE SIZE expected >= 26, got {result}");
    }

    [TestMethod]
    public void TEST_Corpus_072_builtin_lcase()
    {
        var s = @"
        RESULT = SIZE(&LCASE)
end";
        var build = Run(s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        // DOTNET &LCASE may include extended characters; assert >= 26
        var result = ((IntegerVar)build.Execute!.IdentifierTable[build.FoldCase("RESULT")]).Data;
        Assert.IsTrue(result >= 26L, $"&LCASE SIZE expected >= 26, got {result}");
    }

    [TestMethod]
    public void TEST_Corpus_073_builtin_lpad()
    {
        var s = @"
        RESULT = LPAD('hi', 6)
end";
        var build = Run(s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("    hi", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("RESULT")]).Data);
    }

    [TestMethod]
    public void TEST_Corpus_074_builtin_rpad()
    {
        var s = @"
        RESULT = SIZE(RPAD('hi', 6))
end";
        var build = Run(s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual(6L, ((IntegerVar)build.Execute!.IdentifierTable[build.FoldCase("RESULT")]).Data);
    }

    [TestMethod]
    public void TEST_Corpus_075_builtin_integer_test_numeric()
    {
        var s = @"
        INTEGER('42')                                               :S(YES)F(NO)
YES     RESULT = 'numeric'
        :(END)
NO      RESULT = 'not numeric'
end";
        var build = Run(s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("numeric", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("RESULT")]).Data);
    }

    [TestMethod]
    public void TEST_Corpus_075_builtin_integer_test_alpha()
    {
        var s = @"
        INTEGER('abc')                                              :S(YES)F(NO)
YES     RESULT = 'numeric'
        :(END)
NO      RESULT = 'not numeric'
end";
        var build = Run(s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("not numeric", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("RESULT")]).Data);
    }
}
