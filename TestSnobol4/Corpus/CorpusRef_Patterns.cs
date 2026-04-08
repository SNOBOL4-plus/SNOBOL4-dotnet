using Test.TestLexer;

// ReSharper disable StringLiteralTypo

namespace Test.Corpus;

/// <summary>
/// Corpus patterns/ — additional pattern tests not already in SimpleOutput_Patterns.
/// Covers 038_pat_literal (OUTPUT= form) and 054_pat_arbno_alt.
/// </summary>
[DoNotParallelize]
[TestClass]
public class CorpusRef_Patterns
{
    [TestMethod]
    public void TEST_Corpus_038_pat_literal_output()
    {
        // Output-form version (corpus .ref oracle): matched
        var s = @"
        X = 'hello world'
        X 'hello'                                                   :S(YES)F(NO)
YES     OUTPUT = 'matched'
        :(END)
NO      OUTPUT = 'no match'
END";
        Assert.AreEqual("matched", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_054_pat_arbno_alt()
    {
        var s = @"
        X = 'abba'
        X POS(0) ARBNO('a' | 'b') . V RPOS(0)                     :S(YES)F(NO)
YES     OUTPUT = V
        :(END)
NO      OUTPUT = 'no match'
END";
        Assert.AreEqual("abba", SetupTests.RunWithInput(s));
    }
}
