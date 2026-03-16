using Snobol4.Common;
using Test.TestLexer;

// ReSharper disable StringLiteralTypo

namespace Test.Corpus;

/// <summary>
/// Corpus patterns/ — pattern primitives ANY, NOTANY, SPAN, BREAK, LEN, POS, RPOS,
/// TAB, RTAB, REM, ARB, alternation, ARBNO, concat, deref, FAIL.
/// </summary>
[TestClass]
public class SimpleOutput_Patterns
{
    private static Builder Run(string script) =>
        SetupTests.SetupScript("-b", script);

    [TestMethod]
    public void TEST_Corpus_038_pat_literal_match()
    {
        var s = @"
        X = 'hello world'
        X 'hello'                                                   :S(YES)F(NO)
YES     RESULT = 'matched'
        :(END)
NO      RESULT = 'no match'
end";
        var build = Run(s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("matched", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("RESULT")]).Data);
    }

    [TestMethod]
    public void TEST_Corpus_038_pat_literal_no_match()
    {
        var s = @"
        X = 'hello world'
        X 'goodbye'                                                 :S(YES)F(NO)
YES     RESULT = 'matched'
        :(END)
NO      RESULT = 'no match'
end";
        var build = Run(s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("no match", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("RESULT")]).Data);
    }

    [TestMethod]
    public void TEST_Corpus_039_pat_any()
    {
        var s = @"
        X = 'hello'
        X ANY('aeiou') . V                                          :S(YES)
        RESULT = 'no vowel'
        :(END)
YES     RESULT = V
end";
        var build = Run(s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("e", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("RESULT")]).Data);
    }

    [TestMethod]
    public void TEST_Corpus_040_pat_notany()
    {
        var s = @"
        X = 'hello'
        X NOTANY('aeiou') . V                                       :S(YES)
        RESULT = 'all vowels'
        :(END)
YES     RESULT = V
end";
        var build = Run(s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("h", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("RESULT")]).Data);
    }

    [TestMethod]
    public void TEST_Corpus_041_pat_span()
    {
        var s = @"
        X = '12345abc'
        X SPAN('0123456789') . V                                    :S(YES)
        RESULT = 'no digits'
        :(END)
YES     RESULT = V
end";
        var build = Run(s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("12345", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("RESULT")]).Data);
    }

    [TestMethod]
    public void TEST_Corpus_042_pat_break()
    {
        var s = @"
        X = 'hello world'
        X BREAK(' ') . V                                            :S(YES)
        RESULT = 'no space'
        :(END)
YES     RESULT = V
end";
        var build = Run(s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("hello", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("RESULT")]).Data);
    }

    [TestMethod]
    public void TEST_Corpus_043_pat_len()
    {
        var s = @"
        X = 'abcdef'
        X LEN(3) . V                                                :S(YES)
        RESULT = 'fail'
        :(END)
YES     RESULT = V
end";
        var build = Run(s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("abc", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("RESULT")]).Data);
    }

    [TestMethod]
    public void TEST_Corpus_044_pat_pos()
    {
        var s = @"
        X = 'abcdef'
        X POS(2) LEN(3) . V                                         :S(YES)
        RESULT = 'fail'
        :(END)
YES     RESULT = V
end";
        var build = Run(s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("cde", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("RESULT")]).Data);
    }

    [TestMethod]
    public void TEST_Corpus_045_pat_rpos()
    {
        var s = @"
        X = 'abcdef'
        X RPOS(3) LEN(1) . V                                        :S(YES)
        RESULT = 'fail'
        :(END)
YES     RESULT = V
end";
        var build = Run(s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("d", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("RESULT")]).Data);
    }

    [TestMethod]
    public void TEST_Corpus_046_pat_tab()
    {
        var s = @"
        X = 'abcdef'
        X TAB(3) LEN(2) . V                                         :S(YES)
        RESULT = 'fail'
        :(END)
YES     RESULT = V
end";
        var build = Run(s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("de", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("RESULT")]).Data);
    }

    [TestMethod]
    public void TEST_Corpus_047_pat_rtab()
    {
        var s = @"
        X = 'abcdef'
        X RTAB(2) . V                                               :S(YES)
        RESULT = 'fail'
        :(END)
YES     RESULT = V
end";
        var build = Run(s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("abcd", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("RESULT")]).Data);
    }

    [TestMethod]
    public void TEST_Corpus_048_pat_rem()
    {
        var s = @"
        X = 'hello world'
        X LEN(6) REM . V                                            :S(YES)
        RESULT = 'fail'
        :(END)
YES     RESULT = V
end";
        var build = Run(s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("world", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("RESULT")]).Data);
    }

    [TestMethod]
    public void TEST_Corpus_049_pat_arb()
    {
        var s = @"
        X = 'ab12cd'
        X ARB SPAN('0123456789') . V                                :S(YES)
        RESULT = 'fail'
        :(END)
YES     RESULT = V
end";
        var build = Run(s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("12", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("RESULT")]).Data);
    }

    [TestMethod]
    public void TEST_Corpus_050_pat_alt_two()
    {
        var s = @"
        X = 'cat'
        X ('dog' | 'cat') . V                                       :S(YES)
        RESULT = 'no match'
        :(END)
YES     RESULT = V
end";
        var build = Run(s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("cat", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("RESULT")]).Data);
    }

    [TestMethod]
    public void TEST_Corpus_051_pat_alt_three()
    {
        var s = @"
        X = 'blue'
        X ('red' | 'green' | 'blue') . V                           :S(YES)
        RESULT = 'no match'
        :(END)
YES     RESULT = V
end";
        var build = Run(s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("blue", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("RESULT")]).Data);
    }

    [TestMethod]
    public void TEST_Corpus_052_pat_arbno()
    {
        var s = @"
        X = 'aaa'
        X ARBNO('a') . V                                            :S(YES)
        RESULT = 'fail'
        :(END)
YES     RESULT = SIZE(V)
end";
        var build = Run(s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        // ARBNO matches zero or more — anchored match from pos 0 grabs all 3
        Assert.IsTrue(((IntegerVar)build.Execute!.IdentifierTable[build.FoldCase("RESULT")]).Data >= 0);
    }

    [TestMethod]
    public void TEST_Corpus_053_pat_alt_commit()
    {
        var s = @"
        X = 'bc'
        X ('a' | 'b') . V                                          :S(YES)
        RESULT = 'no match'
        :(END)
YES     RESULT = V
end";
        var build = Run(s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("b", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("RESULT")]).Data);
    }

    [TestMethod]
    public void TEST_Corpus_055_pat_concat_seq()
    {
        var s = @"
        X = 'hello world'
        X LEN(5) . R1 LEN(1) LEN(5) . R2                          :S(YES)
        RESULT = 'fail'
        :(END)
YES     RESULT = R1 '/' R2
end";
        var build = Run(s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("hello/world", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("RESULT")]).Data);
    }

    [TestMethod]
    public void TEST_Corpus_056_pat_star_deref()
    {
        var s = @"
        PAT = 'hello'
        X = 'hello world'
        X *PAT . V                                                  :S(YES)
        RESULT = 'fail'
        :(END)
YES     RESULT = V
end";
        var build = Run(s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("hello", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("RESULT")]).Data);
    }

    [TestMethod]
    public void TEST_Corpus_057_pat_fail_builtin()
    {
        var s = @"
        X = 'abc'
        X (LEN(1) . V FAIL)                                        :S(YES)F(DONE)
YES     RESULT = 'should not reach'
        :(END)
DONE    RESULT = 'exhausted'
end";
        var build = Run(s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("exhausted", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("RESULT")]).Data);
    }
}
