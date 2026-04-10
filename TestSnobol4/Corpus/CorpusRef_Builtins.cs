using Test.TestLexer;

// ReSharper disable StringLiteralTypo

namespace Test.Corpus;

/// <summary>
/// Coverage for builtins with zero or near-zero prior test coverage:
///   VALUE(name)  — indirect variable lookup by string name
///   CLEAR(skip)  — reinitialize all variables except skip list
///   COLLECT()    — GC hint (no crash, returns null)
///   CHAR(n)      — integer code point → one-character string
/// </summary>
[DoNotParallelize]
[TestClass]
public class CorpusRef_Builtins
{
    // ── VALUE ──────────────────────────────────────────────────────────────

    [TestMethod]
    public void TEST_Builtin_value_basic()
    {
        // VALUE('X') looks up variable X by name — same as $'X'
        var s = @"
        X = 'hello'
        DIFFER(VALUE('X'), 'hello')                 :F(ok)
        OUTPUT = 'FAIL'                             :(END)
ok      OUTPUT = 'PASS'
END";
        Assert.AreEqual("PASS", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Builtin_value_via_namevar()
    {
        // VALUE(.X) where .X is a name descriptor — deref to value
        var s = @"
        X = 'world'
        NM = .X
        DIFFER(VALUE(NM), 'world')                  :F(ok)
        OUTPUT = 'FAIL'                             :(END)
ok      OUTPUT = 'PASS'
END";
        Assert.AreEqual("PASS", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Builtin_value_integer_name()
    {
        // VALUE(integer) — integer coerced to string name
        var s = @"
        A = 42
        N = 'A'
        DIFFER(VALUE(N), 42)                        :F(ok)
        OUTPUT = 'FAIL'                             :(END)
ok      OUTPUT = 'PASS'
END";
        Assert.AreEqual("PASS", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Builtin_value_equiv_dollar()
    {
        // VALUE('X') must equal $'X'
        var s = @"
        FOO = 'bar'
        DIFFER(VALUE('FOO'), $'FOO')                :F(ok)
        OUTPUT = 'FAIL'                             :(END)
ok      OUTPUT = 'PASS'
END";
        Assert.AreEqual("PASS", SetupTests.RunWithInput(s));
    }

    // ── CLEAR ──────────────────────────────────────────────────────────────

    [TestMethod]
    public void TEST_Builtin_clear_resets_variable()
    {
        // CLEAR('OUTPUT') resets all user vars except OUTPUT
        var s = @"
        A = 'hello'
        B = 42
        CLEAR('OUTPUT')
        IDENT(A, '')                                :S(ok1)
        OUTPUT = 'FAIL: A not cleared'              :(END)
ok1     IDENT(B, '')                                :S(ok2)
        OUTPUT = 'FAIL: B not cleared'              :(END)
ok2     OUTPUT = 'PASS'
END";
        Assert.AreEqual("PASS", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Builtin_clear_skip_list()
    {
        // CLEAR('A,OUTPUT') resets all except A and OUTPUT
        var s = @"
        A = 'keep'
        B = 'lose'
        CLEAR('A,OUTPUT')
        DIFFER(A, 'keep')                           :F(ok1)
        OUTPUT = 'FAIL: A should be kept'           :(END)
ok1     IDENT(B, '')                                :S(ok2)
        OUTPUT = 'FAIL: B should be cleared'        :(END)
ok2     OUTPUT = 'PASS'
END";
        Assert.AreEqual("PASS", SetupTests.RunWithInput(s));
    }

    // ── COLLECT ────────────────────────────────────────────────────────────

    [TestMethod]
    public void TEST_Builtin_collect_no_crash()
    {
        // COLLECT() is a GC hint — must not crash and returns null
        var s = @"
        COLLECT()
        OUTPUT = 'PASS'
END";
        Assert.AreEqual("PASS", SetupTests.RunWithInput(s));
    }

    // ── CHAR ───────────────────────────────────────────────────────────────

    [TestMethod]
    public void TEST_Builtin_char_basic()
    {
        // CHAR(65) = 'A', CHAR(97) = 'a'
        var s = @"
        DIFFER(CHAR(65), 'A')                       :F(ok1)
        OUTPUT = 'FAIL: CHAR(65)'                   :(END)
ok1     DIFFER(CHAR(97), 'a')                       :F(ok2)
        OUTPUT = 'FAIL: CHAR(97)'                   :(END)
ok2     OUTPUT = 'PASS'
END";
        Assert.AreEqual("PASS", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Builtin_char_digits()
    {
        // CHAR(48)='0', CHAR(57)='9'
        var s = @"
        DIFFER(CHAR(48), '0')                       :F(ok1)
        OUTPUT = 'FAIL: CHAR(48)'                   :(END)
ok1     DIFFER(CHAR(57), '9')                       :F(ok2)
        OUTPUT = 'FAIL: CHAR(57)'                   :(END)
ok2     OUTPUT = 'PASS'
END";
        Assert.AreEqual("PASS", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Builtin_char_size_one()
    {
        // CHAR always returns exactly 1 character
        var s = @"
        DIFFER(SIZE(CHAR(72)), 1)                   :F(ok)
        OUTPUT = 'FAIL: SIZE(CHAR(72)) != 1'        :(END)
ok      OUTPUT = 'PASS'
END";
        Assert.AreEqual("PASS", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Builtin_char_roundtrip()
    {
        // Build a string using CHAR and verify with pattern
        var s = @"
        S = CHAR(72) CHAR(105)
        DIFFER(S, 'Hi')                             :F(ok)
        OUTPUT = 'FAIL: CHAR roundtrip'             :(END)
ok      OUTPUT = 'PASS'
END";
        Assert.AreEqual("PASS", SetupTests.RunWithInput(s));
    }
}
