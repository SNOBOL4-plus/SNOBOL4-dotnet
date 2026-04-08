using Test.TestLexer;

// ReSharper disable StringLiteralTypo

namespace Test.Corpus;

/// <summary>
/// String utility tests — new coverage only (not in SimpleOutput_Strings.cs).
/// word1/word2/word3/wordcount: multi-line input scanning patterns.
/// Edge cases: trim_trailing_only, lpad/rpad custom char, replace_identity, empty/single.
/// </summary>
[DoNotParallelize]
[TestClass]
public class CorpusRef_Strings
{
    // ── Additional SIZE / SUBSTR edge cases ──────────────────────────────────

    [TestMethod]
    public void TEST_Corpus_strings_size_empty()
    {
        var s = @"
        OUTPUT = SIZE('')
END";
        Assert.AreEqual("0", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_strings_substr_first_char()
    {
        var s = @"
        OUTPUT = SUBSTR('hello', 1, 1)
END";
        Assert.AreEqual("h", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_strings_trim_trailing_only()
    {
        // TRIM removes trailing spaces; leading spaces are preserved
        var s = @"
        OUTPUT = TRIM('  hello  ')
END";
        Assert.AreEqual("  hello", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_strings_dupl_zero()
    {
        var s = @"
        OUTPUT = SIZE(DUPL('abc', 0))
END";
        Assert.AreEqual("0", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_strings_reverse_single()
    {
        var s = @"
        OUTPUT = REVERSE('x')
END";
        Assert.AreEqual("x", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_strings_lpad_custom_char()
    {
        var s = @"
        OUTPUT = LPAD('hi', 6, '0')
END";
        Assert.AreEqual("0000hi", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_strings_rpad_custom_char()
    {
        var s = @"
        OUTPUT = RPAD('hi', 6, '-')
END";
        Assert.AreEqual("hi----", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_strings_replace_identity()
    {
        // REPLACE with same from/to = identity
        var s = @"
        OUTPUT = REPLACE('hello', 'hello', 'hello')
END";
        Assert.AreEqual("hello", SetupTests.RunWithInput(s));
    }

    // ── Word / scan pattern tests (input via stdin) ───────────────────────────

    [TestMethod]
    public void TEST_Corpus_strings_word1_phrase_scan()
    {
        // Extract noun phrases matching "the X of Y" or "the X a Y"
        // ARB captures into OUTPUT directly as side effect of pattern
        var s = @"
      PAT      =  ' the ' ARB . OUTPUT (' of ' | ' a ')
LOOP  LINE     =  INPUT                            :F(END)
      LINE     ?  PAT                              :(LOOP)
END";
        var input = "She saw the cat of the hat sitting near the dog a bone.\n" +
                    "Nothing interesting on this line.\n" +
                    "I know the house of cards and the tower a bridge.";
        Assert.AreEqual("cat\nhouse", SetupTests.RunWithInput(s, input));
    }

    [TestMethod]
    public void TEST_Corpus_strings_word2_fixed_column()
    {
        // Fixed-column record parsing using TAB — name field keeps padding
        var s = @"
      PAT      =  POS(0) LEN(4) . WHEN
+                 TAB(6) ARB . WHO ' :'
+                 TAB(24) REM . WHAT
LOOP  LINE     =  INPUT                            :F(END)
      LINE     ?  PAT                              :F(LOOP)
      OUTPUT   =  WHO ' invented the ' WHAT ' in ' WHEN  :(LOOP)
END";
        var input = "1769  Watt             : Steam Engine\n" +
                    "1876  Bell             : Telephone\n" +
                    "1903  Wright           : Airplane\n" +
                    "1928  Fleming          : Penicillin";
        Assert.AreEqual(
            "Watt             invented the  Steam Engine in 1769\n" +
            "Bell             invented the  Telephone in 1876\n" +
            "Wright           invented the  Airplane in 1903\n" +
            "Fleming          invented the  Penicillin in 1928",
            SetupTests.RunWithInput(s, input));
    }

    [TestMethod]
    public void TEST_Corpus_strings_word3_break_span()
    {
        // BREAK/SPAN/ARB flexible parsing — trims padding naturally
        var s = @"
      PAT      =  POS(0) BREAK(' ') . WHEN (' ' SPAN(' '))
+                 ARB . WHO (' ' SPAN(' :'))
+                 REM . WHAT
LOOP  LINE     =  INPUT                            :F(END)
      LINE     ?  PAT                              :F(LOOP)
      OUTPUT   =  WHO ' invented the ' WHAT ' in ' WHEN  :(LOOP)
END";
        var input = "1769  Watt : Steam Engine\n" +
                    "1876  Bell : Telephone\n" +
                    "1903  Wright : Airplane\n" +
                    "1928  Fleming : Penicillin";
        Assert.AreEqual(
            "Watt invented the Steam Engine in 1769\n" +
            "Bell invented the Telephone in 1876\n" +
            "Wright invented the Airplane in 1903\n" +
            "Fleming invented the Penicillin in 1928",
            SetupTests.RunWithInput(s, input));
    }

    [TestMethod]
    public void TEST_Corpus_strings_wordcount()
    {
        // Count words using BREAK/SPAN; hyphenated and apostrophe words count as one
        var s = @"
      &TRIM    =  1
      NUMERALS =  '0123456789'
      WORD     =  ""'-"" NUMERALS &UCASE &LCASE
      WPAT     =  BREAK(WORD) SPAN(WORD)
NEXTL LINE     =  INPUT                            :F(DONE)
NEXTW LINE     ?  WPAT =                           :F(NEXTL)
      N        =  N + 1                            :(NEXTW)
DONE  OUTPUT   =  +N ' words'
END";
        var input = "it's a well-known fact that the quick brown fox\njumped over the lazy dog";
        Assert.AreEqual("14 words", SetupTests.RunWithInput(s, input));
    }
}
