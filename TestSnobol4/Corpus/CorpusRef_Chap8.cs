using Test.TestLexer;

// ReSharper disable StringLiteralTypo

namespace Test.Corpus;

/// <summary>
/// Tests derived from corpus/programs/dotnet/chap8_funcs.sno — Spitbol Chapter 8 examples.
/// Covers: SHIFT (string rotation), ASC (char code via &ALPHABET), SWAP (indirect),
/// FACT (recursive factorial), Roman numeral conversion.
/// All self-contained, no -INCLUDE, no file I/O.
/// </summary>
[DoNotParallelize]
[TestClass]
public class CorpusRef_Chap8
{
    // ── SHIFT — rotate string left N positions ───────────────────────────────

    [TestMethod]
    public void TEST_Chap8_shift_basic()
    {
        var s = @"
        DEFINE('SHIFT(S,N)FRONT,REST')
        SHIFT_PAT   =  LEN(*N) . FRONT REM . REST    :(SHIFT_END)
SHIFT   S           ?  SHIFT_PAT                     :F(FRETURN)
        SHIFT       =  REST FRONT                    :(RETURN)
SHIFT_END
        OUTPUT = SHIFT('hello', 2)
        OUTPUT = SHIFT('abcde', 1)
        OUTPUT = SHIFT('ABCDE', 0)
END";
        Assert.AreEqual("llohe\nbcdea\nABCDE", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Chap8_shift_freturn_on_overflow()
    {
        // SHIFT fails if N > SIZE(S)
        var s = @"
        DEFINE('SHIFT(S,N)FRONT,REST')
        SHIFT_PAT   =  LEN(*N) . FRONT REM . REST    :(SHIFT_END)
SHIFT   S           ?  SHIFT_PAT                     :F(FRETURN)
        SHIFT       =  REST FRONT                    :(RETURN)
SHIFT_END
        SHIFT('hi', 5)                               :S(FAIL)
        OUTPUT = 'PASS'                              :(END)
FAIL    OUTPUT = 'FAIL'
END";
        Assert.AreEqual("PASS", SetupTests.RunWithInput(s));
    }

    // ── SWAP — exchange two variables via name indirection ───────────────────

    [TestMethod]
    public void TEST_Chap8_swap_strings()
    {
        var s = @"
        DEFINE('SWAP(X,Y)TEMP')                      :(SWAP_END)
SWAP    TEMP        =  $X
        $X          =  $Y
        $Y          =  TEMP                          :(RETURN)
SWAP_END
        A = 'alpha'
        B = 'beta'
        SWAP(.A, .B)
        OUTPUT = A
        OUTPUT = B
END";
        Assert.AreEqual("beta\nalpha", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Chap8_swap_integers()
    {
        var s = @"
        DEFINE('SWAP(X,Y)TEMP')                      :(SWAP_END)
SWAP    TEMP        =  $X
        $X          =  $Y
        $Y          =  TEMP                          :(RETURN)
SWAP_END
        P = 100
        Q = 200
        SWAP(.P, .Q)
        OUTPUT = P
        OUTPUT = Q
END";
        Assert.AreEqual("200\n100", SetupTests.RunWithInput(s));
    }

    // ── FACT — recursive factorial ────────────────────────────────────────────

    [TestMethod]
    public void TEST_Chap8_fact_small()
    {
        var s = @"
        DEFINE('FACT(N)')                            :(FACT_END)
FACT    FACT        =  LE(N, 1) 1                    :S(RETURN)
        FACT        =  N * FACT(N - 1)               :(RETURN)
FACT_END
        OUTPUT = FACT(0)
        OUTPUT = FACT(1)
        OUTPUT = FACT(5)
        OUTPUT = FACT(7)
END";
        Assert.AreEqual("1\n1\n120\n5040", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Chap8_fact_ten()
    {
        var s = @"
        DEFINE('FACT(N)')                            :(FACT_END)
FACT    FACT        =  LE(N, 1) 1                    :S(RETURN)
        FACT        =  N * FACT(N - 1)               :(RETURN)
FACT_END
        OUTPUT = FACT(10)
END";
        Assert.AreEqual("3628800", SetupTests.RunWithInput(s));
    }

    // ── ASC — ASCII code of first character via &ALPHABET scan ───────────────

    [TestMethod]
    public void TEST_Chap8_asc_basic()
    {
        // ASC uses @-capture against &ALPHABET to get ordinal position
        var s = @"
        DEFINE('ASC(S)C')
        ASC_ONE     =  LEN(1) . C
        ASC_PAT     =  BREAK(*C) @ASC                :(ASC_END)
ASC     S           ?  ASC_ONE                       :F(FRETURN)
        &ALPHABET   ?  ASC_PAT                       :(RETURN)
ASC_END
        OUTPUT = ASC('A')
        OUTPUT = ASC(' ')
END";
        // 'A' is at position 65 in ASCII (0-based in &ALPHABET)
        Assert.AreEqual("65\n32", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Chap8_asc_freturn_on_null()
    {
        // ASC fails (FRETURN) when given null string
        var s = @"
        DEFINE('ASC(S)C')
        ASC_ONE     =  LEN(1) . C
        ASC_PAT     =  BREAK(*C) @ASC                :(ASC_END)
ASC     S           ?  ASC_ONE                       :F(FRETURN)
        &ALPHABET   ?  ASC_PAT                       :(RETURN)
ASC_END
        ASC('')                                      :S(FAIL)
        OUTPUT = 'PASS'                              :(END)
FAIL    OUTPUT = 'FAIL'
END";
        Assert.AreEqual("PASS", SetupTests.RunWithInput(s));
    }

    // ── Roman — Chap8 Roman numeral conversion (recursive, digit-by-digit) ───

    [TestMethod]
    public void TEST_Chap8_roman_basic()
    {
        var s = @"
        DEFINE('Roman(n)units')
        RomanXlat   =  '0,1I,2II,3III,4IV,5V,6VI,7VII,8VIII,9IX,'      :(RomanEnd)
Roman   n           RPOS(1) LEN(1) . units =                            :F(RETURN)
        RomanXlat   units BREAK(',') . units                            :F(FRETURN)
        Roman       =  REPLACE(Roman(n), 'IVXLCDM', 'XLCDM**') units   :S(RETURN)F(FRETURN)
RomanEnd
        OUTPUT = Roman(4)
        OUTPUT = Roman(9)
        OUTPUT = Roman(14)
        OUTPUT = Roman(42)
END";
        Assert.AreEqual("IV\nIX\nXIV\nXLII", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Chap8_roman_large()
    {
        var s = @"
        DEFINE('Roman(n)units')
        RomanXlat   =  '0,1I,2II,3III,4IV,5V,6VI,7VII,8VIII,9IX,'      :(RomanEnd)
Roman   n           RPOS(1) LEN(1) . units =                            :F(RETURN)
        RomanXlat   units BREAK(',') . units                            :F(FRETURN)
        Roman       =  REPLACE(Roman(n), 'IVXLCDM', 'XLCDM**') units   :S(RETURN)F(FRETURN)
RomanEnd
        OUTPUT = Roman(1999)
        OUTPUT = Roman(2024)
END";
        Assert.AreEqual("MCMXCIX\nMMXXIV", SetupTests.RunWithInput(s));
    }
}

