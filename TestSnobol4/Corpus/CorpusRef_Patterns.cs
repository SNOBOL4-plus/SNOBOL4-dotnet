using Test.TestLexer;

// ReSharper disable StringLiteralTypo

namespace Test.Corpus;

/// <summary>
/// Corpus patterns/ — all 20 pattern primitive tests (038–057).
/// </summary>
[DoNotParallelize]
[TestClass]
public class CorpusRef_Patterns
{
    [TestMethod]
    public void TEST_Corpus_038_pat_literal_output()
    {
        var s = @"
        X = 'hello world'
        X 'hello'                                                   :S(YES)F(NO)
YES     OUTPUT = 'matched'
        :(END)
NO      OUTPUT = 'no match'
END";
        Assert.AreEqual("matched", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_039_pat_any()
    {
        var s = @"
        X = 'hello'
        X ANY('aeiou') . V                                          :S(YES)
        OUTPUT = 'no vowel'
        :(END)
YES     OUTPUT = V
END";
        Assert.AreEqual("e", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_040_pat_notany()
    {
        var s = @"
        X = 'hello'
        X NOTANY('aeiou') . V                                       :S(YES)
        OUTPUT = 'all vowels'
        :(END)
YES     OUTPUT = V
END";
        Assert.AreEqual("h", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_041_pat_span()
    {
        var s = @"
        X = '12345abc'
        X SPAN('0123456789') . V                                    :S(YES)
        OUTPUT = 'no digits'
        :(END)
YES     OUTPUT = V
END";
        Assert.AreEqual("12345", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_042_pat_break()
    {
        var s = @"
        X = 'hello world'
        X BREAK(' ') . V                                            :S(YES)
        OUTPUT = 'no space'
        :(END)
YES     OUTPUT = V
END";
        Assert.AreEqual("hello", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_043_pat_len()
    {
        var s = @"
        X = 'abcdef'
        X LEN(3) . V                                                :S(YES)
        OUTPUT = 'fail'
        :(END)
YES     OUTPUT = V
END";
        Assert.AreEqual("abc", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_044_pat_pos()
    {
        var s = @"
        X = 'hello'
        X POS(0) LEN(3) . V                                        :S(YES)
        OUTPUT = 'fail'
        :(END)
YES     OUTPUT = V
END";
        Assert.AreEqual("hel", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_045_pat_rpos()
    {
        var s = @"
        X = 'hello'
        X RPOS(2) LEN(2) . V                                       :S(YES)
        OUTPUT = 'fail'
        :(END)
YES     OUTPUT = V
END";
        Assert.AreEqual("lo", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_046_pat_tab()
    {
        var s = @"
        X = 'abcdef'
        X TAB(3) LEN(2) . V                                        :S(YES)
        OUTPUT = 'fail'
        :(END)
YES     OUTPUT = V
END";
        Assert.AreEqual("de", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_047_pat_rtab()
    {
        var s = @"
        X = 'abcdef'
        X RTAB(2) . V                                              :S(YES)
        OUTPUT = 'fail'
        :(END)
YES     OUTPUT = V
END";
        Assert.AreEqual("abcd", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_048_pat_rem()
    {
        var s = @"
        X = 'hello world'
        X BREAK(' ') LEN(1) REM . V                                :S(YES)
        OUTPUT = 'fail'
        :(END)
YES     OUTPUT = V
END";
        Assert.AreEqual("world", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_049_pat_arb()
    {
        // ARB matches minimum (zero) chars to allow LEN(1) to match 'X' at pos 1
        var s = @"
        X = 'aXb'
        X ARB . V LEN(1)                                           :S(YES)
        OUTPUT = 'fail'
        :(END)
YES     OUTPUT = V
END";
        Assert.AreEqual("", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_050_pat_alt_two()
    {
        var s = @"
        X = 'dog'
        X ('cat' | 'dog') . V                                      :S(YES)
        OUTPUT = 'no match'
        :(END)
YES     OUTPUT = V
END";
        Assert.AreEqual("dog", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_051_pat_alt_three()
    {
        var s = @"
        X = 'banana'
        X ('apple' | 'banana' | 'cherry') . V                      :S(YES)
        OUTPUT = 'no match'
        :(END)
YES     OUTPUT = V
END";
        Assert.AreEqual("banana", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_052_pat_arbno()
    {
        var s = @"
        X = 'aaa'
        X POS(0) ARBNO('a') . V RPOS(0)                           :S(YES)
        OUTPUT = 'fail'
        :(END)
YES     OUTPUT = V
END";
        Assert.AreEqual("aaa", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_053_pat_alt_commit()
    {
        var s = @"
        P = ('a' | 'b' | 'c')
        X = 'b'
        X P . V                                                     :S(YES)F(NO)
YES     OUTPUT = V
        :(END)
NO      OUTPUT = 'no match'
END";
        Assert.AreEqual("b", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_054_pat_arbno_alt()
    {
        var s = @"
        X = 'abba'
        X POS(0) ARBNO('a' | 'b') . V RPOS(0)                     :S(YES)F(NO)
YES     OUTPUT = V
        :(END)
NO      OUTPUT = 'no match'
END";
        Assert.AreEqual("abba", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_055_pat_concat_seq()
    {
        var s = @"
        X = 'abcdef'
        X LEN(2) . A LEN(2) . B LEN(2) . C                        :S(YES)
        OUTPUT = 'fail'
        :(END)
YES     OUTPUT = A ' ' B ' ' C
END";
        Assert.AreEqual("ab cd ef", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_056_pat_star_deref()
    {
        var s = @"
        PAT = 'hello'
        X = 'say hello world'
        X *PAT . V                                                  :S(YES)
        OUTPUT = 'fail'
        :(END)
YES     OUTPUT = V
END";
        Assert.AreEqual("hello", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_057_pat_fail_builtin()
    {
        var s = @"
        X = 'abc'
        X 'abc' FAIL                                                :S(YES)F(NO)
YES     OUTPUT = 'should not reach'
        :(END)
NO      OUTPUT = 'correctly failed'
END";
        Assert.AreEqual("correctly failed", SetupTests.RunWithInput(s));
    }
}
