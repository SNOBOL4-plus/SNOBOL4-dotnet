using Snobol4.Common;
using Test.TestLexer;

namespace Test.FunctionControl;

[TestClass]
public class Define
{
    [TestMethod]
    public void TEST_Function_Bump1()
    {
        var s = @"
        define('bump(var)','bump') :(bumpend)
bump    $var = $var + 1 :(return)
bumpend
        a = array(10)
        j = 0
        i = 0
loop    bump(.j)
        r = r j
        i = i + 1
        lt(i,10)     :s(loop)
        
end";
        var directives = "-b -f";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("12345678910", ((StringVar)build.Execute!.IdentifierTable["r"]).Data);
    }

    [TestMethod]
    public void TEST_Function_Bump2()
    {
        var s = @"
        define('bump(var)','bump') :(bumpend)
bump    output = $var
        $var = $var + 1 :(return)
bumpend
        a = array(10)
        a[2] = 0
        i = 0
loop    bump(.a[2])
        r = r a[2]
        i = i + 1
        lt(i,10)     :s(loop)
        
end";
        var directives = "-b -f";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("12345678910", ((StringVar)build.Execute!.IdentifierTable["r"]).Data);
    }

    [TestMethod]
    public void TEST_Function_Double1()
    {
        var s = @"				define('double(s)')  :(double_end)
double			double = 2 * s	:(return)
double_end		b = double(5)
				output = b";
        var directives = "-b -f";
        var build = SetupTests.SetupScript(directives, s + ";end");
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual(10L, ((IntegerVar)build.Execute!.IdentifierTable["b"]).Data);
    }

    [TestMethod]
    public void TEST_Function_Pythagoras()
    {
        var s = @"

                define('pythagoras(a,b)')  :(double_end)
pythagoras      pythagoras = sqrt(a * a + b * b)  :(return)
double_end      b = pythagoras(4,12)
                output = b
end
";
        var directives = "-b -f";
        var build = SetupTests.SetupScript(directives, s + ";end");
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual(Math.Sqrt(160), ((RealVar)build.Execute!.IdentifierTable["b"]).Data);
    }

    [TestMethod]
    public void TEST_Function_Fibonacci()
    {
        // Not the way to do fibonacci, but a good stress test of recursion
        var s = @"
                    define('fibonacci(n)')  :(fibonacci_end)
fibonacci           output = n
                    le(n,1)  :s(next)
                    fibonacci = fibonacci(n - 1) 
                    fibonacci = fibonacci + fibonacci(n - 2)  
                         :(return)
next                fibonacci = n  :(return)    
fibonacci_end       f = fibonacci(12)
                    output = f
end";
        var directives = "-b -f";
        var build = SetupTests.SetupScript(directives, s + ";end");
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual(144L, ((IntegerVar)build.Execute!.IdentifierTable["f"]).Data);
    }

    [TestMethod]
    public void TEST_Define_001()
    {
        var s = @"
        define('shift(s,n)front,rest') :(end)
        shift('abc', 2)
end";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
    }

    [TestMethod]
    public void TEST_Define_002()
    {
        var s = @"
        define('shift(s,n)')
end";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s + ";end");
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
    }

    [TestMethod]
    public void TEST_Define_003()
    {
        var s = @"
        define('shift(s,)front,rest')
end";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s + ";end");
        Assert.AreNotEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual(85, build.ErrorCodeHistory[0]);
    }

    [TestMethod]
    public void TEST_Define_004()
    {
        var s = @"
        define('shift(,)front,rest')
end";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s + ";end");
        Assert.AreNotEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual(85, build.ErrorCodeHistory[0]);
    }

    [TestMethod]
    public void TEST_Define_005()
    {
        var s = @"
        define('shift()front,rest')
end";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s + ";end");
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
    }

    [TestMethod]
    public void TEST_Define_006()
    {
        var s = @"
        define('ADD(X,Y)FRONT,REST')
        front = 99
        rest = 88
        x = 77
        y = 66             :(addend)
add     add = x + y        
        front = 9
        rest = 8            :(return)
addend  output = a = add(3,4)
end";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s + ";end");
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual(7L, ((IntegerVar)build.Execute!.IdentifierTable["A"]).Data);
        Assert.AreEqual(99L, ((IntegerVar)build.Execute!.IdentifierTable["FRONT"]).Data);
        Assert.AreEqual(88L, ((IntegerVar)build.Execute!.IdentifierTable["REST"]).Data);
    }

    [TestMethod]
    public void TEST_Define_81()
    {
        var s = @"
        define( 'a' | 'b')
end";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s + ";end");
        Assert.AreNotEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual(81, build.ErrorCodeHistory[0]);
    }

    [TestMethod]
    public void TEST_Define_82a()
    {
        var s = @"
        define()
end";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s + ";end");
        Assert.AreNotEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual(82, build.ErrorCodeHistory[0]);
    }

    [TestMethod]
    public void TEST_Define_82b()
    {
        var s = @"
        define('')
end";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s + ";end");
        Assert.AreNotEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual(82, build.ErrorCodeHistory[0]);
    }

    [TestMethod]
    public void TEST_Define_85h()
    {
        var s = @"
        define('shift[]')
end";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s + ";end");
        Assert.AreNotEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual(85, build.ErrorCodeHistory[0]);
    }

    [TestMethod]
    public void TEST_Define_85i()
    {
        var s = @"
        define('shift')
end";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s + ";end");
        Assert.AreNotEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual(85, build.ErrorCodeHistory[0]);
    }

    [TestMethod]
    public void TEST_Define_86j()
    {
        var s = @"
        define('(s,n)')
end";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s + ";end");
        Assert.AreNotEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual(85, build.ErrorCodeHistory[0]);
    }

    [TestMethod]
    public void TEST_Define_85a()
    {
        var s = @"
        define('shift( s,n)front,rest')
end";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s + ";end");
        Assert.AreNotEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual(85, build.ErrorCodeHistory[0]);
    }

    [TestMethod]
    public void TEST_Define_85b()
    {
        var s = @"
        define('shift(s ,n)front,rest')
end";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s + ";end");
        Assert.AreNotEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual(85, build.ErrorCodeHistory[0]);
    }

    [TestMethod]
    public void TEST_Define_85c()
    {
        var s = @"
        define('shift(s, n)front,rest')
end";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s + ";end");
        Assert.AreNotEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual(85, build.ErrorCodeHistory[0]);
    }

    [TestMethod]
    public void TEST_Define_85d()
    {
        var s = @"
        define('shift(s,n )front,rest')
end";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s + ";end");
        Assert.AreNotEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual(85, build.ErrorCodeHistory[0]);
    }

    [TestMethod]
    public void TEST_Define_85e()
    {
        var s = @"
        define('shift(s,n]front,rest')
end";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s + ";end");
        Assert.AreNotEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual(85, build.ErrorCodeHistory[0]);
    }

    [TestMethod]
    public void TEST_Define_85f()
    {
        var s = @"
        define('shift(s,n) front,rest')
end";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s + ";end");
        Assert.AreNotEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual(85, build.ErrorCodeHistory[0]);
    }
    [TestMethod]

    public void TEST_Define_85g()
    {
        var s = @"
        define('shift(s,n)front,rest ')
end";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s + ";end");
        Assert.AreNotEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual(85, build.ErrorCodeHistory[0]);
    }


}