using Test.TestLexer;

// ReSharper disable StringLiteralTypo

namespace Test.Corpus;

/// <summary>
/// Corpus keywords/ — builtin predicates, keywords, lexical comparison.
/// Tests not already covered in SimpleOutput_CaptureKeywords.
/// </summary>
[DoNotParallelize]
[TestClass]
public class CorpusRef_Keywords
{
    [TestMethod]
    public void TEST_Corpus_076_builtin_ident()
    {
        var s = @"
        IDENT('abc', 'abc')                                         :S(YES)F(NO)
YES     OUTPUT = 'equal'
        :(NEXT)
NO      OUTPUT = 'not equal'
NEXT    IDENT('abc', 'xyz')                                         :S(YES2)F(NO2)
YES2    OUTPUT = 'equal'
        :(END)
NO2     OUTPUT = 'not equal'
END";
        Assert.AreEqual("equal\nnot equal", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_082_keyword_stcount()
    {
        var s = @"
        X = 1
        X = 2
        GT(&STNO, 1)                                                :S(YES)F(NO)
YES     OUTPUT = 'stno ok'
        :(END)
NO      OUTPUT = 'wrong'
END";
        Assert.AreEqual("stno ok", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_098_keyword_anchor()
    {
        var s = @"
        &ANCHOR = 1
        X = 'hello world'
        X 'hello'                                                   :S(YES)F(NO)
YES     OUTPUT = 'anchored match ok'
        X 'world'                                                   :S(YES2)F(NO2)
YES2    OUTPUT = 'should not reach'
        :(END)
NO      OUTPUT = 'wrong'
        :(END)
NO2     OUTPUT = 'anchor prevented mid-string match'
END";
        Assert.AreEqual("anchored match ok\nanchor prevented mid-string match", SetupTests.RunWithInput(s));
    }

    [TestMethod, Ignore("M-NET-PAT-PRIMITIVES: &ANCHOR='0' string coercion (ASGNIC) throws error 208 — fix pending")]
    public void TEST_Corpus_099_keyword_rw()
    {
        var s = @"
        differ(&ANCHOR, 0)                         :F(e001)
        OUTPUT = 'FAIL 099/001: &ANCHOR default'   :(END)
e001
        &ANCHOR = 1
        X = 'hello world'
        X 'hello'                                  :S(e002ok)F(e002)
e002    OUTPUT = 'FAIL 099/002: anchor=1 int'      :(END)
e002ok  X 'world'                                  :S(e002b)F(e002bok)
e002b   OUTPUT = 'FAIL 099/002b: anchor blocked mid' :(END)
e002bok
        &ANCHOR = '0'
        X = 'hello world'
        X 'world'                                  :S(e003ok)F(e003)
e003    OUTPUT = 'FAIL 099/003: anchor=0 str coerce' :(END)
e003ok
        differ(DATATYPE(&STLIMIT), 'INTEGER')      :F(e004)
        OUTPUT = 'FAIL 099/004: STLIMIT datatype'  :(END)
e004
        &ANCHOR = 1
        differ(&ANCHOR, 1)                         :F(e005)
        OUTPUT = 'FAIL 099/005: ANCHOR round-trip' :(END)
e005
        &ANCHOR = 0
        OUTPUT = 'PASS 099_keyword_rw (5/5)'
END";
        Assert.AreEqual("PASS 099_keyword_rw (5/5)", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_100_roman_numeral()
    {
        var s = @"
        DEFINE('roman(n)s,v,r,i')                                     :(roman_end)
roman   s = ''
        v = ARRAY(13)
        v<1> = 1000
        v<2> = 900
        v<3> = 500
        v<4> = 400
        v<5> = 100
        v<6> = 90
        v<7> = 50
        v<8> = 40
        v<9> = 10
        v<10> = 9
        v<11> = 5
        v<12> = 4
        v<13> = 1
        r = ARRAY(13)
        r<1> = 'M'
        r<2> = 'CM'
        r<3> = 'D'
        r<4> = 'CD'
        r<5> = 'C'
        r<6> = 'XC'
        r<7> = 'L'
        r<8> = 'XL'
        r<9> = 'X'
        r<10> = 'IX'
        r<11> = 'V'
        r<12> = 'IV'
        r<13> = 'I'
        i = 1
RLOOP   GT(n, 0)                                                       :F(RDONE)
        GE(n, v<i>)                                                    :F(RNEXT)
        s = s r<i>
        n = n - v<i>                                                   :(RLOOP)
RNEXT   i = i + 1                                                      :(RLOOP)
RDONE   roman = s                                                      :(RETURN)
roman_end
        OUTPUT = roman(1)
        OUTPUT = roman(4)
        OUTPUT = roman(9)
        OUTPUT = roman(42)
        OUTPUT = roman(1999)
        OUTPUT = roman(2024)
END";
        Assert.AreEqual("I\nIV\nIX\nXLII\nMCMXCIX\nMMXXIV", SetupTests.RunWithInput(s));
    }
}
