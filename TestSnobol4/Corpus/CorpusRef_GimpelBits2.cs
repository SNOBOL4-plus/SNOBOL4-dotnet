using Test.TestLexer;

// ReSharper disable StringLiteralTypo

namespace Test.Corpus;

/// <summary>
/// Gimpel building-block snippets (second batch) — all inlined, no -INCLUDE.
/// Sources: BASE10.sno, RANDOM.sno, PUSH.sno, MDY.sno, FLOOR.sno, RAISE.sno.
/// Each test exercises one function with known, deterministic inputs.
/// </summary>
[DoNotParallelize]
[TestClass]
public class CorpusRef_GimpelBits2
{
    // ── BASE10 — convert N-base string to decimal ────────────────────────────

    private const string Base10Lib = @"
        DEFINE('BASE10(N,B)T')
        BASEB_ALPHA = '0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ'
                                                    :(BASE10_END)
BASE10  N LEN(1) . T =                             :F(RETURN)
        BASEB_ALPHA BREAK(*T) @T                   :F(ERROR)
        BASE10 = (BASE10 * B) + T                  :(BASE10)
BASE10_END
";

    [TestMethod]
    public void TEST_Gimpel2_base10_hex()
    {
        var s = Base10Lib + @"
        OUTPUT = BASE10('FF', 16)
        OUTPUT = BASE10('10', 16)
        OUTPUT = BASE10('A', 16)
END";
        Assert.AreEqual("255\n16\n10", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Gimpel2_base10_octal()
    {
        var s = Base10Lib + @"
        OUTPUT = BASE10('10', 8)
        OUTPUT = BASE10('77', 8)
        OUTPUT = BASE10('100', 8)
END";
        Assert.AreEqual("8\n63\n64", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Gimpel2_base10_binary()
    {
        var s = Base10Lib + @"
        OUTPUT = BASE10('1010', 2)
        OUTPUT = BASE10('1111', 2)
        OUTPUT = BASE10('1', 2)
END";
        Assert.AreEqual("10\n15\n1", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Gimpel2_base10_decimal()
    {
        // Base 10 → base 10 is identity
        var s = Base10Lib + @"
        OUTPUT = BASE10('42', 10)
        OUTPUT = BASE10('0', 10)
END";
        Assert.AreEqual("42\n0", SetupTests.RunWithInput(s));
    }

    // ── RANDOM — deterministic LCG with fixed seed ───────────────────────────

    private const string RandomLib = @"
        DEFINE('RANDOM(N)')
        RAN_VAR = 1                                :(RANDOM_END)
RANDOM  RAN_VAR = REMDR(RAN_VAR * 4676., 414971.)
        RANDOM = RAN_VAR / 414971.
        RANDOM = NE(N,0) CONVERT(RANDOM * N,'INTEGER') + 1
                                                    :(RETURN)
RANDOM_END
";

    [TestMethod]
    public void TEST_Gimpel2_random_fraction()
    {
        // RANDOM(0) returns real in (0,1)
        var s = RandomLib + @"
        R = RANDOM(0)
        GT(R, 0.0)                                 :F(FAIL)
        LT(R, 1.0)                                 :F(FAIL)
        OUTPUT = 'ok'
        :(END)
FAIL    OUTPUT = 'out of range'
END";
        Assert.AreEqual("ok", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Gimpel2_random_range()
    {
        // RANDOM(N) returns integer in 1..N
        var s = RandomLib + @"
        I = 1
LOOP    R = RANDOM(6)
        GE(R, 1)                                   :F(FAIL)
        LE(R, 6)                                   :F(FAIL)
        I = I + 1
        LE(I, 20)                                  :S(LOOP)
        OUTPUT = 'ok'
        :(END)
FAIL    OUTPUT = 'out of range'
END";
        Assert.AreEqual("ok", SetupTests.RunWithInput(s));
    }

    // ── PUSH/POP — Gimpel linked-list stack via DATA + NRETURN ───────────────

    private const string PushLib = @"
        DEFINE('PUSH(X)')
        DEFINE('POP()')
        DEFINE('TOP()')
        DATA('LINK(LNEXT,LVALUE)')
                                                    :(PUSH_END)
PUSH    PUSH_POP  =  LINK(PUSH_POP, X)
        PUSH  =  .LVALUE(PUSH_POP)                 :(NRETURN)
POP     IDENT(PUSH_POP)                            :S(FRETURN)
        POP  =  LVALUE(PUSH_POP)
        PUSH_POP  =  LNEXT(PUSH_POP)              :(RETURN)
TOP     IDENT(PUSH_POP)                            :S(FRETURN)
        TOP  =  .LVALUE(PUSH_POP)                  :(NRETURN)
PUSH_END
";

    [TestMethod]
    public void TEST_Gimpel2_push_pop_basic()
    {
        var s = PushLib + @"
        PUSH('a')
        PUSH('b')
        PUSH('c')
        OUTPUT = POP()
        OUTPUT = POP()
        OUTPUT = POP()
END";
        Assert.AreEqual("c\nb\na", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Gimpel2_push_pop_empty_freturn()
    {
        var s = PushLib + @"
        POP()                                      :S(FAIL)
        OUTPUT = 'empty ok'
        :(END)
FAIL    OUTPUT = 'FAIL: should FRETURN'
END";
        Assert.AreEqual("empty ok", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Gimpel2_push_integers()
    {
        var s = PushLib + @"
        PUSH(10)
        PUSH(20)
        PUSH(30)
        OUTPUT = POP() + POP() + POP()
END";
        Assert.AreEqual("60", SetupTests.RunWithInput(s));
    }

    // ── FLOOR — largest integer not greater than X ───────────────────────────

    private const string FloorLib = @"
        DEFINE('FLOOR(X)')                         :(FLOOR_END)
FLOOR   FLOOR  =  CONVERT(X,'INTEGER')
        GE(X,0)                                    :S(RETURN)
        FLOOR  =  NE(X,FLOOR)  FLOOR - 1           :(RETURN)
FLOOR_END
";

    [TestMethod]
    public void TEST_Gimpel2_floor_positive()
    {
        var s = FloorLib + @"
        OUTPUT = FLOOR(3.7)
        OUTPUT = FLOOR(3.0)
        OUTPUT = FLOOR(0.1)
END";
        Assert.AreEqual("3\n3\n0", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Gimpel2_floor_negative()
    {
        var s = FloorLib + @"
        OUTPUT = FLOOR(-2.3)
        OUTPUT = FLOOR(-2.0)
        OUTPUT = FLOOR(-0.1)
END";
        Assert.AreEqual("-3\n-2\n-1", SetupTests.RunWithInput(s));
    }

    // ── MDY — day-of-year to M/D/Y ───────────────────────────────────────────

    private const string MdyLib = @"
        DEFINE('MDY(Y,DY)X,T')
        DAY_MONTH  =  '(334,12)(304,11)(273,10)(243,9)'
+       '(212,8)(181,7)(151,6)(120,5)(90,4)(59,3)(31,2)(0,1)'
        LY_DAY_MONTH  =  '(335,12)(305,11)(274,10)(244,9)'
+       '(213,8)(182,7)(152,6)(121,5)(91,4)(60,3)(31,2)(0,1)'
        I  =  SPAN('0123456789')
        SEARCH.X.M = '(' I $ X *GT(DY,X) ',' I $ M    :(MDY_END)
MDY     T = EQ(REMDR(Y,400),0) LY_DAY_MONTH           :S(MDY_1)
        T = EQ(REMDR(Y,100),0) DAY_MONTH               :S(MDY_1)
        T = EQ(REMDR(Y,4),0)   LY_DAY_MONTH            :S(MDY_1)
        T  =  DAY_MONTH
MDY_1   T   SEARCH.X.M                                :F(FRETURN)
        D  =  DY - X
        GT(D, 31)                                      :S(FRETURN)
        MDY  =  M  '/'  D  '/'  Y                      :(RETURN)
MDY_END
";

    [TestMethod]
    public void TEST_Gimpel2_mdy_basic()
    {
        var s = MdyLib + @"
        OUTPUT = MDY(71, 83)
        OUTPUT = MDY(71, 1)
        OUTPUT = MDY(71, 365)
END";
        Assert.AreEqual("3/24/71\n1/1/71\n12/31/71", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Gimpel2_mdy_leapyear()
    {
        // 1972 is a leap year — day 60 = Feb 29
        var s = MdyLib + @"
        OUTPUT = MDY(72, 60)
        OUTPUT = MDY(72, 1)
END";
        Assert.AreEqual("2/29/72\n1/1/72", SetupTests.RunWithInput(s));
    }

    // ── REVERSE string (loop version, no REVERSE builtin dependency) ─────────

    [TestMethod]
    public void TEST_Gimpel2_reverse_loop()
    {
        // Manual reverse via LEN(1) strip loop — tests basic string accumulation
        var s = @"
        DEFINE('REV(S)R,C')                        :(REV_END)
REV     R = ''
RVLOOP  S LEN(1) . C =                            :F(RVDONE)
        R = C R                                    :(RVLOOP)
RVDONE  REV = R                                    :(RETURN)
REV_END
        OUTPUT = REV('hello')
        OUTPUT = REV('abcde')
END";
        Assert.AreEqual("olleh\nedcba", SetupTests.RunWithInput(s));
    }

    // ── ROT13 via REPLACE ────────────────────────────────────────────────────

    [TestMethod]
    public void TEST_Gimpel2_rot13()
    {
        // REPLACE as character translation table — tests 52-char table
        var s = @"
        UPPER = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ'
        ROT13U = 'NOPQRSTUVWXYZABCDEFGHIJKLM'
        LOWER = 'abcdefghijklmnopqrstuvwxyz'
        ROT13L = 'nopqrstuvwxyzabcdefghijklm'
        FROM = UPPER LOWER
        TO   = ROT13U ROT13L
        OUTPUT = REPLACE('Hello World', FROM, TO)
        OUTPUT = REPLACE(REPLACE('Hello World', FROM, TO), FROM, TO)
END";
        Assert.AreEqual("Uryyb Jbeyq\nHello World", SetupTests.RunWithInput(s));
    }
}
