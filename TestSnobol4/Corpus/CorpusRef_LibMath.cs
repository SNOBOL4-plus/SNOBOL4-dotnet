using Test.TestLexer;

// ReSharper disable StringLiteralTypo

namespace Test.Corpus;

/// <summary>
/// Tests for corpus/lib/math.sno functions inlined (no -INCLUDE).
/// Functions: max, min, abs, sign, gcd, lcm.
/// </summary>
[DoNotParallelize]
[TestClass]
public class CorpusRef_LibMath
{
    // Inline math.sno body as a C# constant for reuse across tests
    private const string MathLib = @"
               DEFINE('max(max,x)')                                :(max_end)
max            max            =   LT(max,x) x                     :(RETURN)
max_end

               DEFINE('min(min,x)')                                :(min_end)
min            min            =   GT(min,x) x                     :(RETURN)
min_end

               DEFINE('abs(abs)')                                  :(abs_end)
abs            abs            =   LT(abs,0) -abs                  :(RETURN)
abs_end

               DEFINE('sign(sign)')                                :(sign_end)
sign           sign           =   LT(sign,0) -1                   :S(RETURN)
               sign           =   GT(sign,0)  1                   :(RETURN)
sign_end

               DEFINE('gcd(gcd,b)r')                              :(gcd_end)
gcd            DIFFER(b,0)                                        :F(RETURN)
               r              =   REMDR(gcd, b)
               gcd            =   b
               b              =   r                               :(gcd)
gcd_end

               DEFINE('lcm(a,b)g')                                 :(lcm_end)
lcm            g              =   gcd(a, b)
               lcm            =   (a / g) * b                      :(RETURN)
lcm_end
";

    [TestMethod]
    public void TEST_Corpus_lib_math_max_integers()
    {
        var s = MathLib + @"
        OUTPUT = max(3, 7)
        OUTPUT = max(7, 3)
        OUTPUT = max(5, 5)
END";
        Assert.AreEqual("7"+ Environment.NewLine + "7"+ Environment.NewLine + "5", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_lib_math_min_integers()
    {
        var s = MathLib + @"
        OUTPUT = min(3, 7)
        OUTPUT = min(7, 3)
        OUTPUT = min(5, 5)
END";
        Assert.AreEqual("3"+ Environment.NewLine + "3"+ Environment.NewLine + "5", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_lib_math_max_reals()
    {
        var s = MathLib + @"
        OUTPUT = max(3.5, 2.1)
        OUTPUT = min(3.5, 2.1)
END";
        Assert.AreEqual("3.5"+ Environment.NewLine + "2.1", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_lib_math_abs()
    {
        var s = MathLib + @"
        OUTPUT = abs(-42)
        OUTPUT = abs(42)
        OUTPUT = abs(0)
END";
        Assert.AreEqual("42"+ Environment.NewLine + "42"+ Environment.NewLine + "0", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_lib_math_sign()
    {
        var s = MathLib + @"
        OUTPUT = sign(0)
        OUTPUT = sign(5)
        OUTPUT = sign(-3)
END";
        Assert.AreEqual("0"+ Environment.NewLine + "1"+ Environment.NewLine + "-1", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_lib_math_gcd()
    {
        var s = MathLib + @"
        OUTPUT = gcd(12, 8)
        OUTPUT = gcd(100, 75)
        OUTPUT = gcd(7, 13)
END";
        Assert.AreEqual("4"+ Environment.NewLine + "25"+ Environment.NewLine + "1", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_lib_math_lcm()
    {
        var s = MathLib + @"
        OUTPUT = lcm(4, 6)
        OUTPUT = lcm(3, 5)
        OUTPUT = lcm(6, 6)
END";
        Assert.AreEqual("12"+ Environment.NewLine + "15"+ Environment.NewLine + "6", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_lib_math_all()
    {
        // Full oracle match from test_math.ref
        var s = MathLib + @"
        &TRIM = 1
        OUTPUT = max(3, 7)
        OUTPUT = min(3, 7)
        OUTPUT = max(3.5, 2.1)
        OUTPUT = min(3.5, 2.1)
        OUTPUT = abs(-42)
        OUTPUT = sign(0)
        OUTPUT = sign(5)
        OUTPUT = sign(-3)
        OUTPUT = gcd(12, 8)
        OUTPUT = gcd(100, 75)
        OUTPUT = lcm(4, 6)
END";
        Assert.AreEqual("7"+ Environment.NewLine + "3"+ Environment.NewLine + "3.5"+ Environment.NewLine + "2.1"+ Environment.NewLine + "42"+ Environment.NewLine + "0"+ Environment.NewLine + "1"+ Environment.NewLine + "-1"+ Environment.NewLine + "4"+ Environment.NewLine + "25"+ Environment.NewLine + "12", SetupTests.RunWithInput(s));
    }
}
