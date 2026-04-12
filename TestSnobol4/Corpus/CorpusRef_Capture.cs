using Test.TestLexer;

// ReSharper disable StringLiteralTypo

namespace Test.Corpus;

/// <summary>
/// Pattern capture tests — new coverage only (058-064 already in SimpleOutput_CaptureKeywords.cs).
/// New: SPAN/BREAK/LEN/TAB/ARB/ANY captures, replacement loop, arb_between.
/// </summary>
[DoNotParallelize]
[TestClass]
public class CorpusRef_Capture
{
    // ── Additional capture building blocks ───────────────────────────────────

    [TestMethod]
    public void TEST_Corpus_capture_span_into_var()
    {
        // SPAN captures a run of matching characters
        var s = @"
        X = 'abc123def'
        X SPAN('abcdefghijklmnopqrstuvwxyz') . WORD
        OUTPUT = WORD
END";
        Assert.AreEqual("abc", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_capture_break_into_var()
    {
        // BREAK captures everything up to (not including) delimiter
        var s = @"
        X = 'hello:world'
        X BREAK(':') . LEFT
        OUTPUT = LEFT
END";
        Assert.AreEqual("hello", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_capture_len_into_var()
    {
        // LEN(N) captures exactly N characters
        var s = @"
        X = 'abcdef'
        X LEN(3) . FIRST REM . REST
        OUTPUT = FIRST
        OUTPUT = REST
END";
        Assert.AreEqual("abc"+ Environment.NewLine + "def", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_capture_tab_into_var()
    {
        // TAB(N) advances to column N, captures intervening characters
        var s = @"
        X = 'hello world'
        X POS(0) TAB(5) . LEFT REM . RIGHT
        OUTPUT = LEFT
        OUTPUT = RIGHT
END";
        Assert.AreEqual("hello"+ Environment.NewLine + " world", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_capture_replacement_loop()
    {
        // Loop replacing each occurrence of a char
        var s = @"
        X = 'aababc'
LOOP    X 'a' = 'X'                                :F(DONE)
        :(LOOP)
DONE    OUTPUT = X
END";
        Assert.AreEqual("XXbXbc", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_capture_arb_between()
    {
        // ARB captures between two anchors
        var s = @"
        X = '[hello]'
        X '[' ARB . INNER ']'
        OUTPUT = INNER
END";
        Assert.AreEqual("hello", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_capture_any_into_var()
    {
        // ANY captures a single character from a set
        var s = @"
        X = 'hello'
        X ANY('aeiou') . V
        OUTPUT = V
END";
        Assert.AreEqual("e", SetupTests.RunWithInput(s));
    }
}
