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

    [TestMethod]
    public void TEST_Corpus_coverage_sno_nodes()
    {
        // Every major SNOBOL4 IR node kind. Oracle: sbl /tmp/cov2.sno → 25 lines.
        var s = @"
        OUTPUT = 'hello'
        OUTPUT = 42
        X = 1.5
        OUTPUT = X
        A = 'world'
        OUTPUT = A
        OUTPUT = SIZE(&ALPHABET)
        OUTPUT = 3 + 4
        OUTPUT = 10 - 3
        OUTPUT = 3 * 4
        OUTPUT = 10 / 2
        N = -5
        OUTPUT = N
        OUTPUT = 2 ** 8
        OUTPUT = 'foo' 'bar'
        S4 = 'hello'
        S4 LEN(3) . PART
        OUTPUT = PART
        S5 = 'hello world'
        S5 'world' $ TRAIL
        OUTPUT = TRAIL
        S6 = 'abcdef'
        S6 'abc' @POS1
        OUTPUT = POS1
        S2 = 'abcd'
        S2 'ab' 'cd'
        OUTPUT = 'seq ok'
        S3 = 'cat'
        S3 ('dog' | 'cat')
        OUTPUT = 'alt ok'
        S7 = 'xaby'
        S7 ARB 'b'
        OUTPUT = 'arb ok'
        S8 = 'aaab'
        S8 ARBNO('a') 'b'
        OUTPUT = 'arbno ok'
        PAT = LEN(2)
        S9 = 'abcd'
        S9 *PAT
        OUTPUT = 'star ok'
        VNAME = 'A'
        OUTPUT = $VNAME
        OUTPUT = SIZE('hello')
        T = TABLE()
        T<'key'> = 'val'
        OUTPUT = T<'key'>
        IDENT(UNDEF,)                   :F(NOTNULL)
        OUTPUT = 'null ok'              :(DONULL)
NOTNULL OUTPUT = 'not null'
DONULL
        OUTPUT = 'done'
END";
        var expected = "hello\n42\n1.5\nworld\n256\n7\n7\n12\n5\n-5\n256\n" +
            "foobar\nhel\nworld\n3\nseq ok\nalt ok\narb ok\narbno ok\n" +
            "star ok\nworld\n5\nval\nnull ok\ndone";
        Assert.AreEqual(expected, SetupTests.RunWithInput(s));
    }

    // ── New coverage tests added D-190 ──────────────────────────────────────

    [TestMethod]
    public void TEST_Misc_dupl()
    {
        var s = @"
        OUTPUT = DUPL('ab', 3)
        OUTPUT = DUPL('x', 1)
        OUTPUT = DUPL('y', 0)
        OUTPUT = SIZE(DUPL('abc', 4))
END";
        Assert.AreEqual("ababab\nx\n\n12", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Misc_replace()
    {
        var s = @"
        OUTPUT = REPLACE('hello', 'helo', 'HELO')
        OUTPUT = REPLACE('aabbcc', 'abc', 'xyz')
END";
        Assert.AreEqual("HELLO\nxxyyzz", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Misc_trim()
    {
        var s = @"
        OUTPUT = TRIM('hello   ')
        OUTPUT = TRIM('  no trim leading  ')
        OUTPUT = SIZE(TRIM('abc'))
END";
        Assert.AreEqual("hello\n  no trim leading\n3", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Misc_breakx()
    {
        // BREAKX scans past the break, allowing match on rest
        var s = @"
        S = 'aaa:bbb:ccc'
        S BREAKX(':') . PART ':' 'bbb'
        OUTPUT = PART
END";
        Assert.AreEqual("aaa", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Misc_fence_abort()
    {
        // FENCE cuts backtracking; ABORT forces immediate pattern failure
        var s = @"
        S = 'hello'
        S ('hx' | FENCE 'hell') 'o'            :F(FAIL)
        OUTPUT = 'fence-ok'                    :(END)
FAIL    OUTPUT = 'FAIL'
END";
        Assert.AreEqual("fence-ok", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Misc_succeed_fail_builtins()
    {
        var s = @"
        'anything' SUCCEED                     :F(SFAIL)
        OUTPUT = 'succeed-ok'                  :(END)
SFAIL   OUTPUT = 'FAIL'
END";
        Assert.AreEqual("succeed-ok", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Misc_keyword_trim_fullscan()
    {
        // &TRIM strips trailing blanks from INPUT; &FULLSCAN enables pattern scanning modes
        var s = @"
        &TRIM = 1
        S = 'hello   '
        S RPOS(0)                              :S(TRIMMED)
        OUTPUT = 'FAIL'                        :(END)
TRIMMED OUTPUT = 'trim-ok'
END";
        Assert.AreEqual("trim-ok", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Misc_array_multidim()
    {
        var s = @"
        A = ARRAY('3,3')
        A<1,1> = 'a11'
        A<2,3> = 'a23'
        A<3,2> = 'a32'
        OUTPUT = A<1,1>
        OUTPUT = A<2,3>
        OUTPUT = A<3,2>
END";
        Assert.AreEqual("a11\na23\na32", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Misc_lpad_rpad()
    {
        var s = @"
        OUTPUT = LPAD('hi', 5)
        OUTPUT = RPAD('hi', 5)
        OUTPUT = LPAD('hi', 5, '*')
        OUTPUT = RPAD('hi', 5, '-')
END";
        Assert.AreEqual("   hi\nhi   \n***hi\nhi---", SetupTests.RunWithInput(s));
    }
}
