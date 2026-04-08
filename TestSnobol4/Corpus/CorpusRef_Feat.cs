using Test.TestLexer;

// ReSharper disable StringLiteralTypo

namespace Test.Corpus;

/// <summary>
/// Feature smoke tests from corpus/programs/snobol4/feat/.
/// Each program outputs 'PASS' or 'FAIL' — wraps as a single assertion.
/// Covers: string ops, pattern primitives, builtins/predicates, data structures, functions.
/// </summary>
[DoNotParallelize]
[TestClass]
public class CorpusRef_Feat
{
    [TestMethod]
    public void TEST_Feat_f02_string_ops()
    {
        // SUBSTR REPLACE DUPL SIZE REVERSE TRIM
        var s = @"
        S = 'abcdef'
        IDENT(SIZE(S), 6)                          :F(FAIL)
        IDENT(SUBSTR(S, 2, 3), 'bcd')              :F(FAIL)
        IDENT(DUPL('ab', 3), 'ababab')             :F(FAIL)
        IDENT(REPLACE('aeiou','aeiou','AEIOU'), 'AEIOU') :F(FAIL)
        IDENT(REVERSE('abc'), 'cba')               :F(FAIL)
        IDENT(TRIM('hello   '), 'hello')            :F(FAIL)
        OUTPUT = 'PASS'                            :(END)
FAIL    OUTPUT = 'FAIL'
END";
        Assert.AreEqual("PASS", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Feat_f04_pattern_primitives()
    {
        // LEN POS RPOS TAB RTAB ANY SPAN BREAK ARB ARBNO REM
        var s = @"
        'hello' POS(0) 'he' RPOS(3)               :F(FAIL)
        'abcde' ANY('abc')                         :F(FAIL)
        'abcxyz' SPAN('abc') . V
        IDENT(V, 'abc')                            :F(FAIL)
        'abcxyz' BREAK('x') . V
        IDENT(V, 'abc')                            :F(FAIL)
        'hi' ARB                                   :F(FAIL)
        'aaa' ARBNO('a')                           :F(FAIL)
        'hello' LEN(3)                             :F(FAIL)
        'abc' TAB(2)                               :F(FAIL)
        'abc' RTAB(1)                              :F(FAIL)
        'abc' REM                                  :F(FAIL)
        OUTPUT = 'PASS'                            :(END)
FAIL    OUTPUT = 'FAIL'
END";
        Assert.AreEqual("PASS", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Feat_f06_builtins_predicates()
    {
        // IDENT DIFFER INTEGER LGT DATATYPE CONVERT
        var s = @"
        IDENT('a', 'a')                            :F(FAIL)
        DIFFER('a', 'b')                           :F(FAIL)
        INTEGER(42)                                :F(FAIL)
        LGT('b', 'a')                              :F(FAIL)
        IDENT(REPLACE(DATATYPE(42),&LCASE,&UCASE), 'INTEGER')  :F(FAIL)
        IDENT(REPLACE(DATATYPE('hi'),&LCASE,&UCASE), 'STRING') :F(FAIL)
        IDENT(CONVERT(3, 'STRING'), '3')           :F(FAIL)
        OUTPUT = 'PASS'                            :(END)
FAIL    OUTPUT = 'FAIL'
END";
        Assert.AreEqual("PASS", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Feat_f08_data_array_table()
    {
        // DATA ARRAY TABLE — create and access
        var s = @"
        DATA('POINT(PX,PY)')
        P = POINT(3, 4)
        IDENT(REPLACE(DATATYPE(P),&LCASE,&UCASE), 'POINT') :F(FAIL)
        EQ(PX(P), 3)                               :F(FAIL)
        A = ARRAY(3, 0)
        A<2> = 99
        EQ(A<2>, 99)                               :F(FAIL)
        T = TABLE()
        T<'key'> = 'val'
        IDENT(T<'key'>, 'val')                     :F(FAIL)
        OUTPUT = 'PASS'                            :(END)
FAIL    OUTPUT = 'FAIL'
END";
        Assert.AreEqual("PASS", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Feat_f09_functions_recursion()
    {
        // DEFINE / RETURN / FRETURN + recursive factorial
        var s = @"
        DEFINE('FACT(N)')
        EQ(FACT(5), 120)                           :F(FAIL)
        OUTPUT = 'PASS'                            :(END)
FAIL    OUTPUT = 'FAIL'                            :(END)
FACT    EQ(N, 0)                                   :S(FACT_Z)
        FACT = N * FACT(N - 1)                     :(RETURN)
FACT_Z  FACT = 1                                   :(RETURN)
END";
        Assert.AreEqual("PASS", SetupTests.RunWithInput(s));
    }

    // ── Additional targeted feature tests ────────────────────────────────────

    [TestMethod]
    public void TEST_Feat_goto_conditional()
    {
        // :S() :F() conditional branching
        var s = @"
        X = 5
        GT(X, 3)                                   :S(BIG)F(SMALL)
BIG     OUTPUT = 'big'
        :(END)
SMALL   OUTPUT = 'small'
END";
        Assert.AreEqual("big", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Feat_goto_unconditional()
    {
        // :(LABEL) unconditional goto
        var s = @"
        I = 1
LOOP    OUTPUT = I
        I = I + 1
        LE(I, 3)                                   :S(LOOP)
END";
        Assert.AreEqual("1\n2\n3", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Feat_freturn_predicate()
    {
        // FRETURN from user function used as predicate
        var s = @"
        DEFINE('EVEN(N)')                          :(EVEN_END)
EVEN    EQ(REMDR(N,2), 0)                         :S(RETURN)F(FRETURN)
EVEN_END
        EVEN(4)                                    :F(FAIL)
        EVEN(3)                                    :S(FAIL)
        OUTPUT = 'PASS'                            :(END)
FAIL    OUTPUT = 'FAIL'
END";
        Assert.AreEqual("PASS", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Feat_indirect_reference()
    {
        // $VAR indirect lookup and assignment
        var s = @"
        VARNAME = 'X'
        $VARNAME = 'hello'
        OUTPUT = X
        OUTPUT = $VARNAME
END";
        Assert.AreEqual("hello\nhello", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Feat_local_variables()
    {
        // Function locals do not bleed into caller scope
        var s = @"
        DEFINE('F(A)local1')                       :(F_END)
F       local1 = 'inside'
        F = A ' ' local1                           :(RETURN)
F_END
        local1 = 'outside'
        OUTPUT = F('hi')
        OUTPUT = local1
END";
        Assert.AreEqual("hi inside\noutside", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Feat_pattern_alternation()
    {
        // Pattern alternation with |
        var s = @"
        P = 'cat' | 'dog' | 'bird'
        'I have a dog' P . ANIMAL
        OUTPUT = ANIMAL
        'She has a cat' P . ANIMAL
        OUTPUT = ANIMAL
END";
        Assert.AreEqual("dog\ncat", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Feat_numeric_predicates()
    {
        // GT LT GE LE EQ NE succeed/fail correctly
        // Note: return-value form (OUTPUT = GT(5,3)) is D-NET-188 — tested separately
        var s = @"
        GT(5, 3)                                   :F(FAIL)
        LT(3, 5)                                   :F(FAIL)
        GE(5, 5)                                   :F(FAIL)
        LE(3, 3)                                   :F(FAIL)
        EQ(4, 4)                                   :F(FAIL)
        GT(3, 5)                                   :S(FAIL)
        LT(5, 3)                                   :S(FAIL)
        OUTPUT = 'PASS'                            :(END)
FAIL    OUTPUT = 'FAIL'
END";
        Assert.AreEqual("PASS", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    [Ignore("D-NET-188: numeric predicates GT/LT/GE/LE/EQ/NE do not return first arg on success")]
    public void TEST_Feat_numeric_predicates_return_value()
    {
        // Per SPITBOL spec: GT(A,B) returns A on success
        var s = @"
        OUTPUT = GT(5, 3)
        OUTPUT = LT(3, 5)
        OUTPUT = GE(5, 5)
        OUTPUT = LE(3, 3)
        OUTPUT = EQ(4, 4)
END";
        Assert.AreEqual("5\n3\n5\n3\n4", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Feat_string_concat_implicit()
    {
        // Implicit concatenation (space between values)
        var s = @"
        A = 'hello'
        B = ' '
        C = 'world'
        OUTPUT = A B C
END";
        Assert.AreEqual("hello world", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Feat_f03_numeric()
    {
        // INTEGER/REAL arithmetic predicates GT LT EQ NE GE LE REMDR
        var s = @"
        EQ(2 + 3, 5)                               :F(FAIL)
        GT(10, 5)                                  :F(FAIL)
        LT(3, 7)                                   :F(FAIL)
        NE(1, 2)                                   :F(FAIL)
        GT(1.5, 1.0)                               :F(FAIL)
        EQ(REMDR(10, 3), 1)                        :F(FAIL)
        OUTPUT = 'PASS'                            :(END)
FAIL    OUTPUT = 'FAIL'
END";
        Assert.AreEqual("PASS", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Feat_f05_capture_operators()
    {
        // . conditional assign, $ immediate assign, @ cursor position
        var s = @"
        S = 'hello world'
        S SPAN(&LCASE) . W
        IDENT(W, 'hello')                          :F(FAIL)
        N = 99
        'abcdef' LEN(3) $ V @N
        IDENT(V, 'abc')                            :F(FAIL)
        EQ(N, 3)                                   :F(FAIL)
        OUTPUT = 'PASS'                            :(END)
FAIL    OUTPUT = 'FAIL'
END";
        Assert.AreEqual("PASS", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Feat_f07_keywords()
    {
        // &STLIMIT &ANCHOR &TRIM &ALPHABET &UCASE &LCASE
        var s = @"
        IDENT(SIZE(&ALPHABET), 256)                :F(FAIL)
        IDENT(SIZE(&UCASE), 26)                    :F(FAIL)
        IDENT(SIZE(&LCASE), 26)                    :F(FAIL)
        &ANCHOR = 1
        'xyz' POS(0) 'x'                           :F(FAIL)
        &ANCHOR = 0
        OUTPUT = 'PASS'                            :(END)
FAIL    OUTPUT = 'FAIL'
END";
        Assert.AreEqual("PASS", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Feat_f18_error_handling()
    {
        // SETEXIT jumps to handler on error
        var s = @"
        &ERRLIMIT = 1
        SETEXIT('HANDLER')
        X = 1 / 0
        OUTPUT = 'FAIL (should have jumped to HANDLER)'  :(END)
HANDLER OUTPUT = 'PASS'
END";
        Assert.AreEqual("PASS", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    [Ignore("D-NET-189: INTEGER(real) succeeds in dotnet — should fail for non-integer real")]
    public void TEST_Feat_f19_real_numbers()
    {
        // REAL datatype, GT on reals
        var s = @"
        R = 3.14
        INTEGER(R)                                 :S(FAIL)
        IDENT(REPLACE(DATATYPE(R),&LCASE,&UCASE), 'REAL') :F(FAIL)
        GT(R, 3.0)                                 :F(FAIL)
        OUTPUT = 'PASS'                            :(END)
FAIL    OUTPUT = 'FAIL'
END";
        Assert.AreEqual("PASS", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    [Ignore("D-NET-189: CHOP(3.9) does not return 3.0 — investigate CHOP semantics")]
    public void TEST_Feat_f19_chop()
    {
        // CHOP truncates real toward zero — returns real not integer
        var s = @"
        EQ(CHOP(3.9), 3.0)                         :F(FAIL)
        EQ(CHOP(-2.7), -2.0)                       :F(FAIL)
        OUTPUT = 'PASS'                            :(END)
FAIL    OUTPUT = 'FAIL'
END";
        Assert.AreEqual("PASS", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Feat_integer_arithmetic()
    {
        // Basic integer ops: + - * / REMDR **
        var s = @"
        OUTPUT = 3 + 4
        OUTPUT = 10 - 3
        OUTPUT = 6 * 7
        OUTPUT = 15 / 3
        OUTPUT = REMDR(17, 5)
        OUTPUT = 2 ** 8
END";
        Assert.AreEqual("7\n7\n42\n5\n2\n256", SetupTests.RunWithInput(s));
    }
}
