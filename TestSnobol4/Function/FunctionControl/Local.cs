using Snobol4.Common;
using Test.TestLexer;

// ReSharper disable StringLiteralTypo

namespace Test.FunctionControl;

[TestClass]
public class Local
{
    [TestMethod]
    public void TEST_Local_001()
    {
        var s = @"
                define('pythagoras(a,b)c,d')  :(double_end)
pythagoras      pythagoras = sqrt(a * a + b * b)  :(return)
double_end      b = pythagoras(4,12)
                r1 = local('pythagoras',1);
                r2 = local('pythagoras',2);

end
";
        var directives = "-b -f";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("c", ((StringVar)build.Execute!.IdentifierTable["r1"]).Data);
        Assert.AreEqual("d", ((StringVar)build.Execute!.IdentifierTable["r2"]).Data);
    }

    [TestMethod]
    public void TEST_Local_002()
    {
        var s = @"
                define('pythagoras(a,b)c,d')  :(double_end)
pythagoras      pythagoras = sqrt(a * a + b * b)  :(return)
double_end      b = pythagoras(4,12)
                r1 = local('pythagoras',0)  :f(n)
                r2 = 'success' :(end)
n               r2 = 'failure'
end
";
        var directives = "-b -f";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("", ((StringVar)build.Execute!.IdentifierTable["r1"]).Data);
        Assert.AreEqual("failure", ((StringVar)build.Execute!.IdentifierTable["r2"]).Data);
    }

    [TestMethod]
    public void TEST_Local_003()
    {
        var s = @"
                define('pythagoras(a,b)c,d')  :(double_end)
pythagoras      pythagoras = sqrt(a * a + b * b)  :(return)
double_end      b = pythagoras(4,12)
                r1 = local('pythagoras',3)  :f(n)
                r2 = 'success' :(end)
n               r2 = 'failure'
end
";
        var directives = "-b -f";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("", ((StringVar)build.Execute!.IdentifierTable["r1"]).Data);
        Assert.AreEqual("failure", ((StringVar)build.Execute!.IdentifierTable["r2"]).Data);
    }

    [TestMethod]
    public void TEST_Local_004()
    {
        var s = @"
                define('pythagoras(a,b)c,d')  :(double_end)
pythagoras      pythagoras = sqrt(a * a + b * b)  :(return)
double_end      b = pythagoras(4,12)
                r1 = local('pithagoras',1)  :f(n)
                r2 = 'success' :(end)
n               r2 = 'failure'
end
";
        var directives = "-b -f";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreNotEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual(135, build.ErrorCodeHistory[0]);
    }

    [TestMethod]
    public void TEST_Local_005()
    {
        var s = @"
                define('pythagoras(a,b)c,d')  :(double_end)
pythagoras      pythagoras = sqrt(a * a + b * b)  :(return)
double_end      b = pythagoras(4,12)
                r1 = local('pythagoras',any('123'))  :f(n)
                r2 = 'success' :(end)
n               r2 = 'failure'
end
";
        var directives = "-b -f";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreNotEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual(134, build.ErrorCodeHistory[0]);
    }
}