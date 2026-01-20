using Test.TestLexer;
using Snobol4.Common;

namespace Test.ProgramDefinedDataType;

[TestClass]
public class Data
{
    [TestMethod]
    public void TEST_Data_001()
    {
        var s = @"
        DATA('COMPLEX(REAL,IMAG)')
        X = COMPLEX(3.2, -2.0)
        I = IMAG(X)
        R = REAL(X)
END";

        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual(-2.0, ((RealVar)(build.Execute!.IdentifierTable["I"])).Data);
        Assert.AreEqual(3.2, ((RealVar)(build.Execute!.IdentifierTable["R"])).Data);
    }

    [TestMethod]
    public void TEST_Data_002()
    {
        var s = @"
        DATA('COMPLEX(REAL,IMAG)')
        X = COMPLEX('AAA', 'BBB')
        I = IMAG(X)
        R = REAL(X)
END";

        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("BBB", ((StringVar)(build.Execute!.IdentifierTable["I"])).Data);
        Assert.AreEqual("AAA", ((StringVar)(build.Execute!.IdentifierTable["R"])).Data);
    }

    [TestMethod]
    public void TEST_Data_003()
    {
        var s = @"
        DATA('COMPLEX(REAL,IMAG)')
        X = COMPLEX(ANY('ABC'),SPAN('123'))
        I = IMAG(X)
        R = REAL(X)
END";

        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("Snobol4.Common.SpanPattern", ((PatternVar)(build.Execute!.IdentifierTable["I"])).Data.ToString());
        Assert.AreEqual("Snobol4.Common.AnyPattern", ((PatternVar)(build.Execute!.IdentifierTable["R"])).Data.ToString());
    }

    [TestMethod]
    public void TEST_Data_004()
    {
        var s = @"
        DATA('COMPLEX(REAL,IMAG)')
        X1 = COMPLEX(3.2, -2.0)
        I1 = IMAG(X1)
        R1 = REAL(X1)
        X2 = COMPLEX('AAA', 'BBB')
        I2 = IMAG(X2)
        R2 = REAL(X2)
END";

        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual(-2.0, ((RealVar)(build.Execute!.IdentifierTable["I1"])).Data);
        Assert.AreEqual(3.2, ((RealVar)(build.Execute!.IdentifierTable["R1"])).Data);
        Assert.AreEqual("BBB", ((StringVar)(build.Execute!.IdentifierTable["I2"])).Data);
        Assert.AreEqual("AAA", ((StringVar)(build.Execute!.IdentifierTable["R2"])).Data);
    }
}
