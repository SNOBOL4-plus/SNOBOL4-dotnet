using Test.TestLexer;

// ReSharper disable StringLiteralTypo

namespace Test.Corpus;

/// <summary>
/// Corpus miscellaneous — tests not fitting other categories.
/// Covers: assign/016, capture/061, functions/087, strings/075, rung2/213.
/// </summary>
[DoNotParallelize]
[TestClass]
public class CorpusRef_Misc
{
    [TestMethod]
    public void TEST_Corpus_016_assign_to_output()
    {
        var s = @"
        OUTPUT = 'alpha'
        OUTPUT = 'beta'
END";
        Assert.AreEqual("alpha\nbeta", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_061_capture_in_arbno()
    {
        var s = @"
        X = 'aaa'
        N = 0
LOOP    X POS(N) 'a' . V                                           :F(DONE)
        OUTPUT = V
        N = N + 1
        :(LOOP)
DONE
END";
        Assert.AreEqual("a\na\na", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_087_define_freturn()
    {
        var s = @"
        DEFINE('ispos(x)')                                          :(ispos_end)
ispos   GT(x, 0)                                                   :S(RETURN)F(FRETURN)
ispos_end
        ispos(5)                                                    :S(A)F(B)
A       OUTPUT = 'positive'
        :(NEXT)
B       OUTPUT = 'wrong'
NEXT    ispos(-3)                                                   :S(C)F(D)
C       OUTPUT = 'wrong'
        :(END)
D       OUTPUT = 'not positive'
END";
        Assert.AreEqual("positive\nnot positive", SetupTests.RunWithInput(s));
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
        Assert.AreEqual("numeric\nnot numeric", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_213_indirect_name()
    {
        var s = @"
        A = 42
        X = 'A'
        differ($X, 42)                             :F(e001)
        OUTPUT = 'FAIL 213/001: dollar-X indirect'    :(END)
e001
        NM = .A
        differ($NM, 42)                            :F(e002)
        OUTPUT = 'FAIL 213/002: dollar-NM DT_N deref' :(END)
e002
        $X = 99
        differ(A, 99)                              :F(e003)
        OUTPUT = 'FAIL 213/003: dollar-X lvalue assign' :(END)
e003
        A = 77
        differ($.A, 77)                            :F(e004)
        OUTPUT = 'FAIL 213/004: dollar-dot literal'   :(END)
e004
        define('ref_b()')                          :(ref_b_end)
ref_b   ref_b = .A                                 :(NRETURN)
ref_b_end
        differ(ref_b(), 77)                        :F(e005)
        OUTPUT = 'FAIL 213/005: NRETURN read value'   :(END)
e005
        OUTPUT = 'PASS 213_indirect_name (5/5)'
END";
        Assert.AreEqual("PASS 213_indirect_name (5/5)", SetupTests.RunWithInput(s));
    }
}
