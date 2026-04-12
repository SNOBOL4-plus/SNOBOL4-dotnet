using Test.TestLexer;

// ReSharper disable StringLiteralTypo

namespace Test.Corpus;

/// <summary>
/// Library case.sno — lwr / upr / cap / icase functions.
/// Inline definitions (no -include support needed in test harness).
/// Oracle: corpus/crosscheck/library/test_case.ref
/// </summary>
[DoNotParallelize]
[TestClass]
public class CorpusRef_LibCase
{
    private const string CaseDefs = @"
               DEFINE('lwr(lwr)')                                  :(lwr_end)
lwr            lwr            =   REPLACE(lwr, &UCASE, &LCASE)    :(RETURN)
lwr_end
               DEFINE('upr(upr)')                                  :(upr_end)
upr            upr            =   REPLACE(upr, &LCASE, &UCASE)    :(RETURN)
upr_end
               DEFINE('cap(cap)')                                  :(cap_end)
cap            cap            =   REPLACE(SUBSTR(cap,1,1), &LCASE, &UCASE)
+                                 REPLACE(SUBSTR(cap,2),   &UCASE, &LCASE) :S(RETURN)F(FRETURN)
cap_end
               DEFINE('icase(str)letter,ch')                       :(icase_end)
icase          IDENT(str)                                          :S(RETURN)
               str POS(0) ANY(&UCASE &LCASE) . letter =            :F(icase1)
               icase          =   icase (upr(letter) | lwr(letter)) :(icase)
icase1         str POS(0) LEN(1) . ch =
               icase          =   icase ch                         :(icase)
icase_end
";

    [TestMethod]
    public void TEST_Corpus_lib_case_lwr()
    {
        var s = CaseDefs + @"
        OUTPUT = lwr('HELLO WORLD')
END";
        Assert.AreEqual("hello world", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_lib_case_upr()
    {
        var s = CaseDefs + @"
        OUTPUT = upr('hello world')
END";
        Assert.AreEqual("HELLO WORLD", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_lib_case_cap()
    {
        var s = CaseDefs + @"
        OUTPUT = cap('hELLO wORLD')
END";
        Assert.AreEqual("Hello world", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_lib_case_icase_lower()
    {
        var s = CaseDefs + @"
        'Hello' icase('hello')          :F(bad)
        OUTPUT = 'ok: icase hello'      :(END)
bad     OUTPUT = 'FAIL'
END";
        Assert.AreEqual("ok: icase hello", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_lib_case_icase_upper()
    {
        var s = CaseDefs + @"
        'HELLO' icase('hello')          :F(bad)
        OUTPUT = 'ok: icase HELLO'      :(END)
bad     OUTPUT = 'FAIL'
END";
        Assert.AreEqual("ok: icase HELLO", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_lib_case_icase_mixed()
    {
        var s = CaseDefs + @"
        'HeLLo' icase('hello')          :F(bad)
        OUTPUT = 'ok: icase HeLLo'      :(END)
bad     OUTPUT = 'FAIL'
END";
        Assert.AreEqual("ok: icase HeLLo", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_lib_case_icase_no_match()
    {
        var s = CaseDefs + @"
        'world' icase('hello')          :S(bad)
        OUTPUT = 'no match ok'          :(END)
bad     OUTPUT = 'FAIL: icase matched wrong string'
END";
        Assert.AreEqual("no match ok", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_lib_case_all()
    {
        // Full oracle for test_case.ref
        var s = CaseDefs + @"
        &TRIM = 1
        OUTPUT = lwr('HELLO WORLD')
        OUTPUT = upr('hello world')
        OUTPUT = cap('hELLO wORLD')
        'Hello' icase('hello')          :F(bad_ic1)
        OUTPUT = 'ok: icase hello'
bad_ic1
        'HELLO' icase('hello')          :F(bad_ic2)
        OUTPUT = 'ok: icase HELLO'
bad_ic2
        'HeLLo' icase('hello')          :F(bad_ic3)
        OUTPUT = 'ok: icase HeLLo'
bad_ic3
        'world' icase('hello')          :S(bad_ic4)
        OUTPUT = 'no match ok'          :(END)
bad_ic4 OUTPUT = 'FAIL: icase matched wrong string'
END";
        var expected = "hello world"+ Environment.NewLine + "HELLO WORLD"+ Environment.NewLine + "Hello world"+ Environment.NewLine + "" +
                       "ok: icase hello"+ Environment.NewLine + "ok: icase HELLO"+ Environment.NewLine + "ok: icase HeLLo"+ Environment.NewLine + "no match ok";
        Assert.AreEqual(expected, SetupTests.RunWithInput(s));
    }
}
