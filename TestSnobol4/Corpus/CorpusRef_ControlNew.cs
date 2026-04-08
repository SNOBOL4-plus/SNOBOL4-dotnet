using Test.TestLexer;

// ReSharper disable StringLiteralTypo

namespace Test.Corpus;

/// <summary>
/// Corpus control_new/ — goto, conditional branches, loops (031–037).
/// </summary>
[DoNotParallelize]
[TestClass]
public class CorpusRef_ControlNew
{
    [TestMethod]
    public void TEST_Corpus_031_goto_unconditional()
    {
        var s = @"
        OUTPUT = 'before'
        :(DONE)
        OUTPUT = 'skipped'
DONE
END";
        Assert.AreEqual("before", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_032_goto_loop_count()
    {
        var s = @"
        N = 0
LOOP    N = N + 1
        GT(N, 5)                        :S(DONE)
        :(LOOP)
DONE    OUTPUT = N
END";
        Assert.AreEqual("6", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_033_goto_success()
    {
        var s = @"
        X = 'hello world'
        X 'hello'                       :S(FOUND)F(NOTFOUND)
FOUND   OUTPUT = 'found'
        :(END)
NOTFOUND OUTPUT = 'not found'
END";
        Assert.AreEqual("found", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_034_goto_failure()
    {
        var s = @"
        X = 'hello world'
        X 'goodbye'                     :S(FOUND)F(NOTFOUND)
FOUND   OUTPUT = 'found'
        :(END)
NOTFOUND OUTPUT = 'not found'
END";
        Assert.AreEqual("not found", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_035_goto_both_branches()
    {
        var s = @"
        X = 'abc'
        X 'abc'                         :S(YES)
        OUTPUT = 'no'
        :(END)
YES     OUTPUT = 'yes'
END";
        Assert.AreEqual("yes", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_036_goto_skip_to_end()
    {
        var s = @"
        OUTPUT = 'one'
        :(END)
        OUTPUT = 'two'
END";
        Assert.AreEqual("one", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_037_goto_nested_labels()
    {
        var s = @"
        :(A)
        OUTPUT = 'skip'
A       OUTPUT = 'a'
        :(B)
        OUTPUT = 'skip'
B       OUTPUT = 'b'
END";
        Assert.AreEqual("a\nb", SetupTests.RunWithInput(s));
    }
}
