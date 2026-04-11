using Snobol4.Common;
using Test.TestLexer;

namespace Test.Gimpel;

[TestClass]
public class UPLO
{
    [TestMethod]
    public void UPLO0()
    {
        var s = @"

* UPLO.inc - UPLO(S) will return its argument with upper case letters
*	     converted to lower case, and vice versa.  Non-alphabetic
*	     characters are ignored.
*
	    DEFINE('UPLO(S)')
	    UP_LO  =  'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz'
	    LO_UP  =  'abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ'
						:(UPLO_END)
UPLO	UPLO   =  REPLACE(S, UP_LO, LO_UP)	:(RETURN)
UPLO_END
        R = UPLO('Hello, World!')
end

";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("hELLO, wORLD!", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("R")]).Data);
    }
    [TestMethod]
    public void UPLO1()
    {
        var s = @"
	    DEFINE('UPLO(S)')
	    UP_LO  =  'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz'
	    LO_UP  =  'abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ'
					:(UPLO_END)
UPLO	UPLO   =  REPLACE(S, UP_LO, LO_UP)	:(RETURN)
UPLO_END
        R1 = UPLO('')
        R2 = UPLO('ABC')
        R3 = UPLO('abc')
        R4 = UPLO('123!@#')
        R5 = UPLO('aAbBcC')
end
";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("",       ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("R1")]).Data);
        Assert.AreEqual("abc",    ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("R2")]).Data);
        Assert.AreEqual("ABC",    ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("R3")]).Data);
        Assert.AreEqual("123!@#", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("R4")]).Data);
        Assert.AreEqual("AaBbCc", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("R5")]).Data);
    }
    [TestMethod]
    public void UPLO2()
    {
        // Swap case of a sentence with punctuation
        var s = @"
	    DEFINE('UPLO(S)')
	    UP_LO  =  'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz'
	    LO_UP  =  'abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ'
					:(UPLO_END)
UPLO	UPLO   =  REPLACE(S, UP_LO, LO_UP)	:(RETURN)
UPLO_END
        R1 = UPLO('The Quick Brown Fox')
        R2 = UPLO('sNOBOL4')
end
";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("tHE qUICK bROWN fOX", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("R1")]).Data);
        Assert.AreEqual("Snobol4",             ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("R2")]).Data);
    }
}
