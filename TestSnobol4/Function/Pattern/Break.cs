using Snobol4.Common;
using Test.TestLexer;

// ReSharper disable StringLiteralTypo

namespace Test.Pattern;

[TestClass]
public class Break
{

    [TestMethod]
    public void TEST_Break_001()
    {
        var s = @"
        &anchor = 0
        letters = 'abcdefghijklmnopqrstuvwxyz'
        gap = break(letters) 
        subject = ':= one,,, two,..  three';
        subject gap . temp1  :s(y)f(n)
y       result = 'success'   :(end)
n       result = 'fail' 
end";

        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual("success", ((StringVar)build.Execute!.IdentifierTable["RESULT"]).Data);
        Assert.AreEqual(":= ", ((StringVar)build.Execute!.IdentifierTable["TEMP1"]).Data);
    }

    [TestMethod]
    public void TEST_Break_002()
    {
        var s = @"
        &anchor = 0
        letters = 'abcdefghijklmnopqrstuvwxyz'
        gap = notany(letters) break(letters) 
        subject = 'one,,, two,..  three';
        subject gap . temp1  :s(y)f(n)
y       result = 'success'   :(end)
n       result = 'fail' 
end";

        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual("success", ((StringVar)build.Execute!.IdentifierTable["RESULT"]).Data);
        Assert.AreEqual(",,, ", ((StringVar)build.Execute!.IdentifierTable["TEMP1"]).Data);
    }

    [TestMethod]
    public void TEST_Break_003()
    {
        var s = @"
        &anchor = 0
        letters = 'abcdefghijklmnopqrstuvwxyz'
        gap = break(letters) 
        subject = 'one,,, two,..  three';
        subject gap . temp1  :s(y)f(n)
y       result = 'success'   :(end)
n       result = 'fail' 
end";

        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual("success", ((StringVar)build.Execute!.IdentifierTable["RESULT"]).Data);
        Assert.AreEqual("", ((StringVar)build.Execute!.IdentifierTable["TEMP1"]).Data);
    }

    [TestMethod]
    public void TEST_Break_004()
    {
        var s = @"
        &anchor = 0
        letters = ''
        gap = break(letters) 
        subject = 'one,,, two,..  three';
        subject gap . temp1  :s(y)f(n)
y       result = 'success'   :(end)
n       result = 'fail' 
end";

        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreNotEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual(69, build.ErrorCodeHistory[0]);
    }

    [TestMethod]
    public void TEST_Break_005()
    {
        var s = @"
        &anchor = 0
        subject = 'c'
        pattern = break('c')
        subject pattern = '****'   :f(n)
        temp1 = '[' subject ']'   
y       result = 'success'   :(end)
n       result = 'fail' 
end";

        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual("success", ((StringVar)build.Execute!.IdentifierTable["RESULT"]).Data);
        Assert.AreEqual("[****c]", ((StringVar)build.Execute!.IdentifierTable["TEMP1"]).Data);
    }

    [TestMethod]
    public void TEST_Break_006()
    {
        var s = @"
        &anchor = 0
        letters = 'abcdefghijklmnopqrstuvwxyz'
        gap = break(letters) 
        subject = '5675765()*)(*)';
        subject gap . temp1  :s(y)f(n)
y       result = 'success'   :(end)
n       result = 'fail' 
end";

        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual("fail", ((StringVar)build.Execute!.IdentifierTable["RESULT"]).Data);
    }

    [TestMethod]
    public void TEST_Break_007()
    {
        var s = @"
        &anchor = 0
        subject = '12345'
        char = '3'
        gap = break(char)
        subject gap . temp1  :s(y)f(n)
y       result = 'success'   :(end)
n       result = 'fail' 
end";

        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual("success", ((StringVar)build.Execute!.IdentifierTable["RESULT"]).Data);
        Assert.AreEqual("12", ((StringVar)build.Execute!.IdentifierTable["TEMP1"]).Data);
    }


    [TestMethod]
    public void TEST_Break_008()
    {
        var s = @"
        &anchor = 0
        subject = '12345'
        char = ''
        gap = break(char)
        subject gap . temp1  :s(y)f(n)
y       result = 'success'   :(end)
n       result = 'fail' 
end";

        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreNotEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual(69, build.ErrorCodeHistory[0]);
    }

    [TestMethod]
    public void TEST_Break_009()
    {
        var s = @"
        &anchor = 0
        subject = '12345'
        char = '8'
        gap = break(char)
        subject gap . temp1  :s(y)f(n)
y       result = 'success'   :(end)
n       result = 'fail' 
end";

        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual("fail", ((StringVar)build.Execute!.IdentifierTable["RESULT"]).Data);
    }

}