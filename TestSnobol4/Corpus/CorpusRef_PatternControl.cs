using Test.TestLexer;

// ReSharper disable StringLiteralTypo

namespace Test.Corpus;

/// <summary>
/// Corpus-style tests for pattern control builtins with thin corpus coverage:
///   BAL    — matches balanced parentheses
///   FENCE  — commits (no backtrack through this point)
///   ABORT  — immediately fails the entire match
///   SUCCEED — always succeeds (forces backtrack loop)
/// </summary>
[DoNotParallelize]
[TestClass]
public class CorpusRef_PatternControl
{
    // ── BAL ────────────────────────────────────────────────────────────────

    [TestMethod]
    public void TEST_PatCtl_bal_simple_parens()
    {
        // BAL matches a balanced parenthesized expression
        var s = @"
        X = '(a+b)'
        X BAL . V                                   :S(ok)F(END)
ok      DIFFER(V, '(a+b)')                          :F(pass)
        OUTPUT = 'FAIL: BAL mismatch'               :(END)
pass    OUTPUT = 'PASS'
END";
        Assert.AreEqual("PASS", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_PatCtl_bal_nested()
    {
        // BAL matches nested parens as a unit
        var s = @"
        X = '((a)(b))'
        X BAL . V                                   :S(ok)F(END)
ok      DIFFER(V, '((a)(b))')                       :F(pass)
        OUTPUT = 'FAIL: BAL nested mismatch'        :(END)
pass    OUTPUT = 'PASS'
END";
        Assert.AreEqual("PASS", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_PatCtl_bal_no_parens()
    {
        // BAL without parens matches a single character
        var s = @"
        X = 'abc'
        X BAL . V                                   :S(ok)F(END)
ok      DIFFER(V, 'a')                              :F(pass)
        OUTPUT = 'FAIL: BAL no-paren'               :(END)
pass    OUTPUT = 'PASS'
END";
        Assert.AreEqual("PASS", SetupTests.RunWithInput(s));
    }

    // ── FENCE ──────────────────────────────────────────────────────────────

    [TestMethod]
    public void TEST_PatCtl_fence_commits()
    {
        // FENCE: once matched, no backtrack. 'a' FENCE 'b' on 'ab' succeeds.
        var s = @"
        X = 'ab'
        X 'a' FENCE 'b'                             :S(pass)F(END)
pass    OUTPUT = 'PASS'
END";
        Assert.AreEqual("PASS", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_PatCtl_fence_blocks_backtrack()
    {
        // FENCE prevents backtrack: 'a' FENCE ('b'|'c') on 'ac'
        // matches 'a' then FENCE commits, then tries 'b' (fail), 'c' (ok)
        var s = @"
        X = 'ac'
        X 'a' FENCE ('b' | 'c') . V                :S(ok)F(END)
ok      DIFFER(V, 'c')                              :F(pass)
        OUTPUT = 'FAIL: FENCE alt'                  :(END)
pass    OUTPUT = 'PASS'
END";
        Assert.AreEqual("PASS", SetupTests.RunWithInput(s));
    }

    // ── ABORT ──────────────────────────────────────────────────────────────

    [TestMethod]
    public void TEST_PatCtl_abort_fails_match()
    {
        // ABORT immediately terminates the match with failure
        var s = @"
        X = 'hello'
        X ('hello' | ABORT) . V                     :S(ok)F(END)
ok      DIFFER(V, 'hello')                          :F(pass)
        OUTPUT = 'FAIL: ABORT branch taken'         :(END)
pass    OUTPUT = 'PASS'
END";
        Assert.AreEqual("PASS", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_PatCtl_abort_no_backtrack()
    {
        // ABORT in first alt, second alt never tried
        var s = @"
        X = 'hello'
        X (ABORT | 'hello') . V                     :S(wrong)F(ok)
wrong   OUTPUT = 'FAIL: should have aborted'        :(END)
ok      OUTPUT = 'PASS'
END";
        Assert.AreEqual("PASS", SetupTests.RunWithInput(s));
    }

    // ── SUCCEED ────────────────────────────────────────────────────────────

    [TestMethod]
    public void TEST_PatCtl_succeed_forces_success()
    {
        // SUCCEED always succeeds at zero width — use with anchors to verify
        var s = @"
        X = 'abc'
        X POS(0) SUCCEED                            :S(ok)F(END)
ok      OUTPUT = 'PASS'
END";
        Assert.AreEqual("PASS", SetupTests.RunWithInput(s));
    }
}
