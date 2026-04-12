using Test.TestLexer;

// ReSharper disable StringLiteralTypo

namespace Test.Corpus;

/// <summary>
/// Corpus crosscheck/patterns 058–067 — FENCE keyword and FENCE(P) function tests.
/// All 10 tests inlined from corpus oracle .ref files.
/// 058: FENCE keyword passes (0-arg). 059–067: FENCE(P) — passes only when FENCE(P) is implemented.
/// </summary>
[DoNotParallelize]
[TestClass]
public class CorpusRef_FenceTests
{
    [TestMethod]
    public void TEST_058_pat_fence_keyword()
    {
        var s = @"
        X = 'AB'
        X  LEN(1)  FENCE  LEN(2)                              :S(YES)F(NO)
YES     OUTPUT = 'should not reach'                            :(END)
NO      OUTPUT = 'aborted correctly'
END";
        Assert.AreEqual("aborted correctly", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_059_pat_fence_fn_basic()
    {
        var s = @"
        X = 'hello'
        X  FENCE('hello')                                      :F(NO)
        OUTPUT = 'matched'                                     :(END)
NO      OUTPUT = 'failed'
END";
        Assert.AreEqual("matched", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_060_pat_fence_fn_fail()
    {
        var s = @"
        X = 'hello'
        X  FENCE('world')                                      :S(YES)F(NO)
YES     OUTPUT = 'should not reach'                            :(END)
NO      OUTPUT = 'failed gracefully'
END";
        Assert.AreEqual("failed gracefully", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_061_pat_fence_fn_seal()
    {
        var s = @"
        X = 'AB'
        X  FENCE(LEN(1) | LEN(2))  RPOS(0)                   :S(YES)F(NO)
YES     OUTPUT = 'should not reach'                            :(END)
NO      OUTPUT = 'sealed correctly'
END";
        Assert.AreEqual("sealed correctly", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_062_pat_fence_fn_outer()
    {
        var s = @"
        X = 'bx'
        X  ('a' | 'b')  FENCE('x')                            :F(NO)
        OUTPUT = 'outer alt worked'                            :(END)
NO      OUTPUT = 'failed'
END";
        Assert.AreEqual("outer alt worked", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_063_pat_fence_fn_optional()
    {
        var s = @"
        digits = '0123456789'
        &ANCHOR = 1
        X = '123abc'
        X  FENCE(SPAN(digits) | '') . N
        OUTPUT = N
END";
        Assert.AreEqual("123", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_064_pat_fence_fn_capture()
    {
        var s = @"
        alnum = '0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_'
        &ANCHOR = 1
        X = 'hello_world rest'
        X  ANY(&UCASE &LCASE)  FENCE(SPAN(alnum) | '') . ID
        OUTPUT = ID
END";
        Assert.AreEqual("hello_world", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_065_pat_fence_fn_decimal()
    {
        var s = @"
        digits = '0123456789'
        &ANCHOR = 1
        X = '42rest'
        X  ANY('123456789')  FENCE(SPAN(digits) | '') . N
        OUTPUT = N
END";
        Assert.AreEqual("42", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_066_pat_fence_fn_nested()
    {
        var s = @"
        digits = '0123456789'
        &ANCHOR = 1
        Real = SPAN(digits) '.' FENCE(SPAN(digits) | '')
+          |   SPAN(digits)
        X = '3.14rest'
        X  FENCE(Real) . N
        OUTPUT = N
END";
        Assert.AreEqual("3.14", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_067_pat_fence_fn_vs_kw()
    {
        var s = @"
        X = 'AX'
        X  LEN(1)  FENCE  LEN(2)                              :S(BAD)
        X  FENCE('Z')                                         :S(BAD)
        OUTPUT = 'both correct'                               :(END)
BAD     OUTPUT = 'wrong'
END";
        Assert.AreEqual("both correct", SetupTests.RunWithInput(s));
    }
}
