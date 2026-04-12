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
        var input = "She saw the cat of the hat sitting near the dog a bone." + Environment.NewLine +
                    "Nothing interesting on this line." + Environment.NewLine +
                    "I know the house of cards and the tower a bridge.";
        Assert.AreEqual("cat"+ Environment.NewLine + "house", SetupTests.RunWithInput(s, input));
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
        var input = "1769  Watt             : Steam Engine" + Environment.NewLine +
                    "1876  Bell             : Telephone" + Environment.NewLine +
                    "1903  Wright           : Airplane" + Environment.NewLine +
                    "1928  Fleming          : Penicillin";
        Assert.AreEqual(
            "Watt             invented the  Steam Engine in 1769" + Environment.NewLine +
            "Bell             invented the  Telephone in 1876" + Environment.NewLine +
            "Wright           invented the  Airplane in 1903" + Environment.NewLine +
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
        var input = "1769  Watt : Steam Engine" + Environment.NewLine +
                    "1876  Bell : Telephone" + Environment.NewLine +
                    "1903  Wright : Airplane" + Environment.NewLine +
                    "1928  Fleming : Penicillin";
        Assert.AreEqual(
            "Watt invented the Steam Engine in 1769" + Environment.NewLine +
            "Bell invented the Telephone in 1876" + Environment.NewLine +
            "Wright invented the Airplane in 1903" + Environment.NewLine +
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
        var input = "it's a well-known fact that the quick brown fox"+ Environment.NewLine + "jumped over the lazy dog";
        Assert.AreEqual("14 words", SetupTests.RunWithInput(s, input));
    }

    [TestMethod]
    public void TEST_Corpus_065_builtin_size()
    {
        var s = @"
        OUTPUT = SIZE('hello')
END";
        Assert.AreEqual("5", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_066_builtin_substr()
    {
        var s = @"
        OUTPUT = SUBSTR('hello world', 7, 5)
END";
        Assert.AreEqual("world", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_067_builtin_replace()
    {
        var s = @"
        OUTPUT = REPLACE('hello', 'aeiou', 'AEIOU')
END";
        Assert.AreEqual("hEllO", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_068_builtin_trim()
    {
        var s = @"
        OUTPUT = SIZE(TRIM('hello   '))
END";
        Assert.AreEqual("5", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_069_builtin_dupl()
    {
        var s = @"
        OUTPUT = DUPL('ab', 3)
END";
        Assert.AreEqual("ababab", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_070_builtin_reverse()
    {
        var s = @"
        OUTPUT = REVERSE('hello')
END";
        Assert.AreEqual("olleh", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_071_builtin_ucase_keyword()
    {
        var s = @"
        OUTPUT = &UCASE
END";
        Assert.AreEqual("ABCDEFGHIJKLMNOPQRSTUVWXYZ", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_072_builtin_lcase_keyword()
    {
        var s = @"
        OUTPUT = &LCASE
END";
        Assert.AreEqual("abcdefghijklmnopqrstuvwxyz", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_073_builtin_lpad()
    {
        var s = @"
        OUTPUT = LPAD('hi', 6)
END";
        Assert.AreEqual("    hi", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_074_builtin_rpad()
    {
        var s = @"
        OUTPUT = SIZE(RPAD('hi', 6))
END";
        Assert.AreEqual("6", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_075_builtin_integer_test()
    {
        var s = @"
        INTEGER('42')                                               :S(YES)F(NO)
YES     OUTPUT = 'numeric'
        :(NEXT)
NO      OUTPUT = 'not numeric'
NEXT    INTEGER('abc')                                              :S(YES2)F(NO2)
YES2    OUTPUT = 'numeric'
        :(END)
NO2     OUTPUT = 'not numeric'
END";
        Assert.AreEqual("numeric"+ Environment.NewLine + "not numeric", SetupTests.RunWithInput(s));
    }
}
