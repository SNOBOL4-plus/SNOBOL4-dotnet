using Snobol4.Common;
using Test.TestLexer;

// ReSharper disable StringLiteralTypo

namespace Test.Pattern;

[TestClass]
public class Rem
{

    [TestMethod]
    public void TEST_Rem_001()
    {
        var s = @"
        &anchor = 0
        subject = 'programmer'
        pattern = 'gra' rem . test
        subject pattern      :s(y)f(n)
y       result = 'success'   :(end)
n       result = 'fail' 
end";

        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual("success", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("result")]).Data);
        Assert.AreEqual("mmer", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("test")]).Data);
    }

    [TestMethod]
    public void TEST_Rem_002()
    {
        var s = @"
        &anchor = 0
        subject = 'THE WINTER WINDS'
        pattern = 'WIN' rem . test
        subject pattern      :s(y)f(n)
y       result = 'success'   :(end)
n       result = 'fail' 
end";

        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual("success", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("result")]).Data);
        Assert.AreEqual("TER WINDS", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("test")]).Data);
    }

    [TestMethod]
    public void TEST_Rem_003()
    {
        var s = @"
        &anchor = 0
        subject = 'THE WINTER WINDS'
        pattern = 'WINDS' rem . test
        subject pattern      :s(y)f(n)
y       result = 'success'   :(end)
n       result = 'fail' 
end";

        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual("success", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("result")]).Data);
        Assert.AreEqual("", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("test")]).Data);
    }
    [TestMethod]
    public void TEST_Rem_004_after_pattern()
    {
        // REM after a fixed prefix captures the rest
        var s = @"
        subject = 'hello world'
        subject 'hello ' rem . rest   :f(bad)
        result = rest   :(end)
bad     result = 'bad'
end";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("world",
            ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("result")]).Data);
    }
    [TestMethod]
    public void TEST_Rem_005_at_start()
    {
        // REM at start captures entire string
        var s = @"
        subject = 'hello'
        &anchor = 1
        subject rem . cap   :f(bad)
        result = cap   :(end)
bad     result = 'bad'
end";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("hello",
            ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("result")]).Data);
    }

    [TestMethod]
    public void TEST_Rem_006_empty_subject()
    {
        // REM on empty subject captures null
        var s = @"
        subject = ''
        subject rem . cap   :f(bad)
        result = 'ok'   :(end)
bad     result = 'bad'
end";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("ok",
            ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("result")]).Data);
    }

    [TestMethod]
    public void TEST_Rem_007_rem_in_table_split()
    {
        // Classic REM use: skip prefix, capture rest
        var s = @"
        line = 'key: value here'
        line 'key: ' rem . val   :f(bad)
        result = val   :(end)
bad     result = 'bad'
end";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("value here", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("result")]).Data);
    }

    [TestMethod]
    public void TEST_Rem_008_rem_always_succeeds()
    {
        // REM always succeeds (even mid-string with nothing left)
        var s = @"
        subject = 'ab'
        subject 'ab' rem . cap   :s(ok)f(bad)
ok      result = 'ok:' cap   :(end)
bad     result = 'bad'
end";
        var directives = "-b";
        var build = SetupTests.SetupScript(directives, s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("ok:", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("result")]).Data);
    }

}
