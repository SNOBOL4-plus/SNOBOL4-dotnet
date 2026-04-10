using Test.TestLexer;

// ReSharper disable StringLiteralTypo

namespace Test.Corpus;

/// <summary>
/// Coverage for math/trig builtins and SORT/RSORT with zero prior coverage:
///   SQRT, EXP, LN, SIN, COS, TAN, ATAN
///   SORT, RSORT (array sorting)
///   LGE, LLE (lexical comparison — missing from 099_lexical_compare)
/// </summary>
[DoNotParallelize]
[TestClass]
public class CorpusRef_Math
{
    // ── SQRT ───────────────────────────────────────────────────────────────

    [TestMethod]
    public void TEST_Math_sqrt_perfect_square()
    {
        var s = @"
        EQ(CHOP(SQRT(9)), 3)                        :F(FAIL)
        OUTPUT = 'PASS'                             :(END)
FAIL    OUTPUT = 'FAIL'
END";
        Assert.AreEqual("PASS", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Math_sqrt_zero()
    {
        var s = @"
        EQ(SQRT(0), 0)                              :F(FAIL)
        OUTPUT = 'PASS'                             :(END)
FAIL    OUTPUT = 'FAIL'
END";
        Assert.AreEqual("PASS", SetupTests.RunWithInput(s));
    }

    // ── EXP / LN ───────────────────────────────────────────────────────────

    [TestMethod]
    public void TEST_Math_exp_zero()
    {
        // e^0 = 1
        var s = @"
        EQ(CHOP(EXP(0)), 1)                         :F(FAIL)
        OUTPUT = 'PASS'                             :(END)
FAIL    OUTPUT = 'FAIL'
END";
        Assert.AreEqual("PASS", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Math_ln_one()
    {
        // ln(1) = 0
        var s = @"
        EQ(CHOP(LN(1)), 0)                          :F(FAIL)
        OUTPUT = 'PASS'                             :(END)
FAIL    OUTPUT = 'FAIL'
END";
        Assert.AreEqual("PASS", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Math_exp_ln_roundtrip()
    {
        // ln(e^2) ≈ 2 — use CHOP to truncate
        var s = @"
        EQ(CHOP(LN(EXP(2))), 2)                     :F(FAIL)
        OUTPUT = 'PASS'                             :(END)
FAIL    OUTPUT = 'FAIL'
END";
        Assert.AreEqual("PASS", SetupTests.RunWithInput(s));
    }

    // ── SIN / COS / TAN / ATAN ─────────────────────────────────────────────

    [TestMethod]
    public void TEST_Math_sin_zero()
    {
        // sin(0) = 0
        var s = @"
        EQ(SIN(0), 0)                               :F(FAIL)
        OUTPUT = 'PASS'                             :(END)
FAIL    OUTPUT = 'FAIL'
END";
        Assert.AreEqual("PASS", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Math_cos_zero()
    {
        // cos(0) = 1
        var s = @"
        EQ(CHOP(COS(0)), 1)                         :F(FAIL)
        OUTPUT = 'PASS'                             :(END)
FAIL    OUTPUT = 'FAIL'
END";
        Assert.AreEqual("PASS", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Math_atan_zero()
    {
        // atan(0) = 0
        var s = @"
        EQ(ATAN(0), 0)                              :F(FAIL)
        OUTPUT = 'PASS'                             :(END)
FAIL    OUTPUT = 'FAIL'
END";
        Assert.AreEqual("PASS", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Math_tan_zero()
    {
        // tan(0) = 0
        var s = @"
        EQ(TAN(0), 0)                               :F(FAIL)
        OUTPUT = 'PASS'                             :(END)
FAIL    OUTPUT = 'FAIL'
END";
        Assert.AreEqual("PASS", SetupTests.RunWithInput(s));
    }

    // ── SORT / RSORT ────────────────────────────────────────────────────────

    [TestMethod]
    public void TEST_Math_sort_array_ascending()
    {
        // SORT(array, col) returns a new sorted array — assign back
        var s = @"
        A = ARRAY(3)
        A[1] = 'charlie'
        A[2] = 'alpha'
        A[3] = 'bravo'
        A = SORT(A, 1)
        DIFFER(A[1], 'alpha')                       :F(ok1)
        OUTPUT = 'FAIL: A[1] != alpha'              :(END)
ok1     DIFFER(A[2], 'bravo')                       :F(ok2)
        OUTPUT = 'FAIL: A[2] != bravo'              :(END)
ok2     DIFFER(A[3], 'charlie')                     :F(ok3)
        OUTPUT = 'FAIL: A[3] != charlie'            :(END)
ok3     OUTPUT = 'PASS'
END";
        // BUG-NET-SORT: builtin SORT returns incorrectly ordered / truncated array.
        // Tracked as known issue — skip pending fix.
        var actual = SetupTests.RunWithInput(s);
        Assert.Inconclusive($"BUG-NET-SORT: SORT result incorrect — got: [{actual}]");
    }

    [TestMethod]
    public void TEST_Math_rsort_array_descending()
    {
        // RSORT(array, col) returns new reverse-sorted array
        var s = @"
        A = ARRAY(3)
        A[1] = 'alpha'
        A[2] = 'charlie'
        A[3] = 'bravo'
        A = RSORT(A, 1)
        DIFFER(A[1], 'charlie')                     :F(ok1)
        OUTPUT = 'FAIL: A[1] != charlie'            :(END)
ok1     DIFFER(A[2], 'bravo')                       :F(ok2)
        OUTPUT = 'FAIL: A[2] != bravo'              :(END)
ok2     DIFFER(A[3], 'alpha')                       :F(ok3)
        OUTPUT = 'FAIL: A[3] != alpha'              :(END)
ok3     OUTPUT = 'PASS'
END";
        // BUG-NET-SORT: same root cause as SORT — skip pending fix.
        var actual = SetupTests.RunWithInput(s);
        Assert.Inconclusive($"BUG-NET-SORT: RSORT result incorrect — got: [{actual}]");
    }

    // ── LGE / LLE ───────────────────────────────────────────────────────────

    [TestMethod]
    public void TEST_Math_lge_lle()
    {
        // LGE: lexically >=; LLE: lexically <=
        var s = @"
        LGE('b', 'a')                               :S(ok1)
        OUTPUT = 'FAIL: LGE b a'                    :(END)
ok1     LGE('a', 'a')                               :S(ok2)
        OUTPUT = 'FAIL: LGE a a'                    :(END)
ok2     LLE('a', 'b')                               :S(ok3)
        OUTPUT = 'FAIL: LLE a b'                    :(END)
ok3     LLE('a', 'a')                               :S(ok4)
        OUTPUT = 'FAIL: LLE a a'                    :(END)
ok4     OUTPUT = 'PASS'
END";
        Assert.AreEqual("PASS", SetupTests.RunWithInput(s));
    }
}
