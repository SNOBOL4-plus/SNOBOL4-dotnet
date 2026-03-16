using Snobol4.Common;
using Test.TestLexer;

// ReSharper disable StringLiteralTypo

namespace Test.Corpus;

/// <summary>
/// Corpus simple OUTPUT tests: output, assign, arith_new, concat, control_new.
/// Programs store result in RESULT variable; we assert via IdentifierTable.
/// </summary>
[TestClass]
public class SimpleOutput_Basic
{
    private static Builder Run(string script) =>
        SetupTests.SetupScript("-b", script);

    // -----------------------------------------------------------------------
    // output/ — literal output
    // -----------------------------------------------------------------------

    [TestMethod]
    public void TEST_Corpus_001_output_string_literal()
    {
        var s = @"
        RESULT = 'hello world'
end";
        var build = Run(s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("hello world", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("RESULT")]).Data);
    }

    [TestMethod]
    public void TEST_Corpus_002_output_integer_literal()
    {
        var s = @"
        RESULT = 42
end";
        var build = Run(s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual(42L, ((IntegerVar)build.Execute!.IdentifierTable[build.FoldCase("RESULT")]).Data);
    }

    [TestMethod]
    public void TEST_Corpus_003_output_real_literal()
    {
        var s = @"
        RESULT = 3.14
end";
        var build = Run(s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual(3.14, ((RealVar)build.Execute!.IdentifierTable[build.FoldCase("RESULT")]).Data);
    }

    [TestMethod]
    public void TEST_Corpus_004_output_empty_string()
    {
        var s = @"
        RESULT = ''
end";
        var build = Run(s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("RESULT")]).Data);
    }

    [TestMethod]
    public void TEST_Corpus_005_output_multiline()
    {
        var s = @"
        R1 = 'line one'
        R2 = 'line two'
        R3 = 'line three'
end";
        var build = Run(s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("line one",   ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("R1")]).Data);
        Assert.AreEqual("line two",   ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("R2")]).Data);
        Assert.AreEqual("line three", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("R3")]).Data);
    }

    [TestMethod]
    public void TEST_Corpus_006_output_keyword_alphabet()
    {
        var s = @"
        RESULT = SIZE(&ALPHABET)
end";
        var build = Run(s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        // DOTNET &ALPHABET has 255 characters (0x01–0xFF; implementation-specific)
        var alphabetSize = ((IntegerVar)build.Execute!.IdentifierTable[build.FoldCase("RESULT")]).Data;
        Assert.IsTrue(alphabetSize == 255L || alphabetSize == 256L,
            $"&ALPHABET SIZE should be 255 or 256, got {alphabetSize}");
    }

    [TestMethod]
    public void TEST_Corpus_007_output_null_var()
    {
        var s = @"
        RESULT = X
end";
        var build = Run(s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("RESULT")]).Data);
    }

    [TestMethod]
    public void TEST_Corpus_008_output_double_quoted()
    {
        var s = @"
        RESULT = ""hello world""
end";
        var build = Run(s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("hello world", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("RESULT")]).Data);
    }

    // -----------------------------------------------------------------------
    // assign/ — variable assignment
    // -----------------------------------------------------------------------

    [TestMethod]
    public void TEST_Corpus_009_assign_string()
    {
        var s = @"
        X = 'hello'
        RESULT = X
end";
        var build = Run(s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("hello", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("RESULT")]).Data);
    }

    [TestMethod]
    public void TEST_Corpus_010_assign_integer()
    {
        var s = @"
        N = 42
        RESULT = N
end";
        var build = Run(s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual(42L, ((IntegerVar)build.Execute!.IdentifierTable[build.FoldCase("RESULT")]).Data);
    }

    [TestMethod]
    public void TEST_Corpus_011_assign_chain()
    {
        var s = @"
        X = 'alpha'
        Y = X
        RESULT = Y
end";
        var build = Run(s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("alpha", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("RESULT")]).Data);
    }

    [TestMethod]
    public void TEST_Corpus_012_assign_null()
    {
        var s = @"
        X = 'something'
        X =
        RESULT = X
end";
        var build = Run(s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("RESULT")]).Data);
    }

    [TestMethod]
    public void TEST_Corpus_013_assign_overwrite()
    {
        var s = @"
        X = 'first'
        X = 'second'
        RESULT = X
end";
        var build = Run(s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("second", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("RESULT")]).Data);
    }

    [TestMethod]
    public void TEST_Corpus_014_assign_indirect_dollar()
    {
        var s = @"
        $'X' = 'hello'
        RESULT = X
end";
        var build = Run(s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("hello", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("RESULT")]).Data);
    }

    [TestMethod]
    public void TEST_Corpus_015_assign_indirect_var()
    {
        var s = @"
        V = 'X'
        $V = 'world'
        RESULT = X
end";
        var build = Run(s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("world", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("RESULT")]).Data);
    }

    [TestMethod]
    public void TEST_Corpus_016_assign_to_output_two_lines()
    {
        var s = @"
        R1 = 'alpha'
        R2 = 'beta'
end";
        var build = Run(s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("alpha", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("R1")]).Data);
        Assert.AreEqual("beta",  ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("R2")]).Data);
    }

    // -----------------------------------------------------------------------
    // arith_new/ — arithmetic
    // -----------------------------------------------------------------------

    [TestMethod]
    public void TEST_Corpus_023_arith_add()
    {
        var s = @"
        RESULT = 1 + 2
end";
        var build = Run(s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual(3L, ((IntegerVar)build.Execute!.IdentifierTable[build.FoldCase("RESULT")]).Data);
    }

    [TestMethod]
    public void TEST_Corpus_024_arith_subtract()
    {
        var s = @"
        RESULT = 10 - 3
end";
        var build = Run(s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual(7L, ((IntegerVar)build.Execute!.IdentifierTable[build.FoldCase("RESULT")]).Data);
    }

    [TestMethod]
    public void TEST_Corpus_025_arith_multiply()
    {
        var s = @"
        RESULT = 6 * 7
end";
        var build = Run(s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual(42L, ((IntegerVar)build.Execute!.IdentifierTable[build.FoldCase("RESULT")]).Data);
    }

    [TestMethod]
    public void TEST_Corpus_026_arith_divide()
    {
        var s = @"
        RESULT = 10 / 4
end";
        var build = Run(s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual(2L, ((IntegerVar)build.Execute!.IdentifierTable[build.FoldCase("RESULT")]).Data);
    }

    [TestMethod]
    public void TEST_Corpus_027_arith_exponent()
    {
        var s = @"
        RESULT = 2 ** 8
end";
        var build = Run(s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual(256L, ((IntegerVar)build.Execute!.IdentifierTable[build.FoldCase("RESULT")]).Data);
    }

    [TestMethod]
    public void TEST_Corpus_028_arith_unary_minus()
    {
        var s = @"
        RESULT = -5
end";
        var build = Run(s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual(-5L, ((IntegerVar)build.Execute!.IdentifierTable[build.FoldCase("RESULT")]).Data);
    }

    [TestMethod]
    public void TEST_Corpus_029_arith_precedence()
    {
        var s = @"
        RESULT = 2 + 3 * 4
end";
        var build = Run(s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual(14L, ((IntegerVar)build.Execute!.IdentifierTable[build.FoldCase("RESULT")]).Data);
    }

    [TestMethod]
    public void TEST_Corpus_030_arith_remdr()
    {
        var s = @"
        RESULT = REMDR(10, 3)
end";
        var build = Run(s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual(1L, ((IntegerVar)build.Execute!.IdentifierTable[build.FoldCase("RESULT")]).Data);
    }

    // -----------------------------------------------------------------------
    // concat/ — concatenation
    // -----------------------------------------------------------------------

    [TestMethod]
    public void TEST_Corpus_017_concat_two_strings()
    {
        var s = @"
        RESULT = 'hello' ' world'
end";
        var build = Run(s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("hello world", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("RESULT")]).Data);
    }

    [TestMethod]
    public void TEST_Corpus_018_concat_three_strings()
    {
        var s = @"
        RESULT = 'a' 'b' 'c'
end";
        var build = Run(s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("abc", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("RESULT")]).Data);
    }

    [TestMethod]
    public void TEST_Corpus_019_concat_var_string()
    {
        var s = @"
        X = 'hello'
        RESULT = X ' world'
end";
        var build = Run(s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("hello world", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("RESULT")]).Data);
    }

    [TestMethod]
    public void TEST_Corpus_020_concat_integer_string()
    {
        var s = @"
        RESULT = 42 ' items'
end";
        var build = Run(s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("42 items", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("RESULT")]).Data);
    }

    [TestMethod]
    public void TEST_Corpus_021_concat_in_assignment()
    {
        var s = @"
        X = 'foo' 'bar'
        RESULT = X
end";
        var build = Run(s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("foobar", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("RESULT")]).Data);
    }

    [TestMethod]
    public void TEST_Corpus_022_concat_multipart()
    {
        var s = @"
        RESULT = 'a' 'b' 'c' 'd'
end";
        var build = Run(s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("abcd", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("RESULT")]).Data);
    }

    // -----------------------------------------------------------------------
    // control_new/ — goto, branching
    // -----------------------------------------------------------------------

    [TestMethod]
    public void TEST_Corpus_031_goto_unconditional()
    {
        var s = @"
        R1 = 'before'
        :(DONE)
        R1 = 'skipped'
DONE
end";
        var build = Run(s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("before", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("R1")]).Data);
    }

    [TestMethod]
    public void TEST_Corpus_032_goto_loop_count()
    {
        var s = @"
        N = 0
LOOP    N = N + 1
        GT(N, 5)                                                    :S(DONE)
        :(LOOP)
DONE    RESULT = N
end";
        var build = Run(s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual(6L, ((IntegerVar)build.Execute!.IdentifierTable[build.FoldCase("RESULT")]).Data);
    }

    [TestMethod]
    public void TEST_Corpus_033_goto_success()
    {
        var s = @"
        X = 'hello world'
        X 'hello'                                                   :S(FOUND)F(NOTFOUND)
FOUND   RESULT = 'found'
        :(END)
NOTFOUND RESULT = 'not found'
end";
        var build = Run(s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("found", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("RESULT")]).Data);
    }

    [TestMethod]
    public void TEST_Corpus_034_goto_failure()
    {
        var s = @"
        X = 'hello world'
        X 'goodbye'                                                 :S(FOUND)F(NOTFOUND)
FOUND   RESULT = 'found'
        :(END)
NOTFOUND RESULT = 'not found'
end";
        var build = Run(s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("not found", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("RESULT")]).Data);
    }

    [TestMethod]
    public void TEST_Corpus_035_goto_both_branches()
    {
        var s = @"
        X = 'abc'
        X 'abc'                                                     :S(YES)
        RESULT = 'no'
        :(END)
YES     RESULT = 'yes'
end";
        var build = Run(s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("yes", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("RESULT")]).Data);
    }

    [TestMethod]
    public void TEST_Corpus_036_goto_skip_to_end()
    {
        var s = @"
        R1 = 'one'
        :(END)
        R1 = 'two'
end";
        var build = Run(s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("one", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("R1")]).Data);
    }

    [TestMethod]
    public void TEST_Corpus_037_goto_nested_labels()
    {
        var s = @"
        :(A)
        SKIP = 'skipped'
A       R1 = 'a'
        :(B)
        SKIP = 'skipped'
B       R2 = 'b'
end";
        var build = Run(s);
        Assert.AreEqual(0, build.ErrorCodeHistory.Count);
        Assert.AreEqual("a", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("R1")]).Data);
        Assert.AreEqual("b", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("R2")]).Data);
        Assert.AreEqual("", ((StringVar)build.Execute!.IdentifierTable[build.FoldCase("SKIP")]).Data);
    }
}
