using Snobol4.Common;
using Test.TestLexer;

namespace Test.Gimpel;

[TestClass]
public class ROMAN
{

    [TestMethod]
    public void ROMAN0()
    {
        var s = @"
* ROMAN.inc - ROMAN(N) will return the roman numeral representation
*             of the integer N.  0 < N < 4000.
*
	    DEFINE('ROMAN(N)T')			:(ROMAN_END)
ROMAN	OUTPUT = N
        N   RPOS(1)  LEN(1) . T  =		:F(RETURN)
	    '0,1I,2II,3III,4IV,5V,6VI,7VII,8VIII,9IX,'
+	    T   BREAK(',') . T			:F(FRETURN)
	    ROMAN = REPLACE(ROMAN(N), 'IVXLCDM', 'XLCDM**') T
+						:S(RETURN)F(FRETURN)
ROMAN_END
        R1 = ROMAN('1776')
        R2 = ROMAN('9')
        R3 = ROMAN('45')
        R4 = ROMAN('2026')
end

";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("MDCCLXXVI", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("R1")]).Data);
        Assert.AreEqual("IX", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("R2")]).Data);
        Assert.AreEqual("XLV", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("R3")]).Data);
        Assert.AreEqual("MMXXVI", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("R4")]).Data);
    }

    [TestMethod]
    public void ROMAN2()
    {
        var s = @"
* ROMAN.inc - ROMAN(N) will return the roman numeral representation
*             of the integer N.  0 < N < 4000.
*
	    DEFINE('ROMAN(N)T')			:(ROMAN_END)
ROMAN	OUTPUT = N
        N   RPOS(1)  LEN(1) . T  =		:F(RETURN)
	    '0,1I,2II,3III,4IV,5V,6VI,7VII,8VIII,9IX,'
+	    T   BREAK(',') . T			:F(FRETURN)
	    ROMAN = REPLACE(ROMAN(N), 'IVXLCDM', 'XLCDM**') T
+						:S(RETURN)F(FRETURN)
ROMAN_END
        R1 = ROMAN('BA')   :F(F)
        R2 = 'Success' :(END)
F	    R2 = 'Fail'
end

";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("R1")]).Data);
        Assert.AreEqual("Fail", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("R2")]).Data);
    }
    [TestMethod]
    public void ROMAN3()
    {
        // boundary values: 1, 3999, subtractive forms
        var s = @"
	    DEFINE('ROMAN(N)T')			:(ROMAN_END)
ROMAN	OUTPUT = N
        N   RPOS(1)  LEN(1) . T  =		:F(RETURN)
	    '0,1I,2II,3III,4IV,5V,6VI,7VII,8VIII,9IX,'
+	    T   BREAK(',') . T			:F(FRETURN)
	    ROMAN = REPLACE(ROMAN(N), 'IVXLCDM', 'XLCDM**') T
+						:S(RETURN)F(FRETURN)
ROMAN_END
        R1 = ROMAN('1')
        R2 = ROMAN('3999')
        R3 = ROMAN('400')
        R4 = ROMAN('900')
end
";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("I",      ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("R1")]).Data);
        Assert.AreEqual("MMMCMXCIX", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("R2")]).Data);
        Assert.AreEqual("CD",     ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("R3")]).Data);
        Assert.AreEqual("CM",     ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("R4")]).Data);
    }

    [TestMethod]
    public void ROMAN4()
    {
        // common historical years
        var s = @"
	    DEFINE('ROMAN(N)T')			:(ROMAN_END)
ROMAN	OUTPUT = N
        N   RPOS(1)  LEN(1) . T  =		:F(RETURN)
	    '0,1I,2II,3III,4IV,5V,6VI,7VII,8VIII,9IX,'
+	    T   BREAK(',') . T			:F(FRETURN)
	    ROMAN = REPLACE(ROMAN(N), 'IVXLCDM', 'XLCDM**') T
+						:S(RETURN)F(FRETURN)
ROMAN_END
        R1 = ROMAN('1066')
        R2 = ROMAN('1492')
        R3 = ROMAN('1984')
        R4 = ROMAN('2000')
end
";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("MLXVI",       ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("R1")]).Data);
        Assert.AreEqual("MCDXCII",     ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("R2")]).Data);
        Assert.AreEqual("MCMLXXXIV",   ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("R3")]).Data);
        Assert.AreEqual("MM",          ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("R4")]).Data);
    }
}
