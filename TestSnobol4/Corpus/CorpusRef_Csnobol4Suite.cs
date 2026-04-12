using Test.TestLexer;

// ReSharper disable StringLiteralTypo

namespace Test.Corpus;

/// <summary>
/// Budne csnobol4-suite — 116 programs from corpus/programs/csnobol4-suite/.
/// Each test reads .sno source and .ref oracle from disk (CORPUS_ROOT env or /home/claude/corpus).
/// Excluded (8): bench, breakline, genc, k, ndbm, sleep, time, line2.
/// </summary>
[DoNotParallelize]
[TestClass]
public class CorpusRef_Csnobol4Suite
{
    private static readonly string CorpusRoot =
        Environment.GetEnvironmentVariable("CORPUS_ROOT") ?? "/home/claude/corpus";
    private static readonly string Suite =
        Path.Combine(CorpusRoot, "programs", "csnobol4-suite");

    private void RunBudne(string name, string? input = null)
    {
        var sno = Path.Combine(Suite, name + ".sno");
        var refPath = Path.Combine(Suite, name + ".ref");
        if (!File.Exists(sno) || !File.Exists(refPath))
            Assert.Inconclusive($"Corpus file not found: {sno}");
        var src = File.ReadAllText(sno);
        var expected = File.ReadAllText(refPath).TrimEnd();
        var actual = SetupTests.RunWithInput(src, input);
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TEST_Budne_100func()
    {
        RunBudne("100func");
    }

    [TestMethod]
    public void TEST_Budne_8bit()
    {
        RunBudne("8bit");
    }

    [TestMethod]
    public void TEST_Budne_8bit2()
    {
        RunBudne("8bit2");
    }

    [TestMethod]
    public void TEST_Budne_a()
    {
        RunBudne("a");
    }

    [TestMethod]
    public void TEST_Budne_alis()
    {
        RunBudne("alis");
    }

    [TestMethod]
    public void TEST_Budne_alph()
    {
        RunBudne("alph");
    }

    [TestMethod]
    public void TEST_Budne_alt1()
    {
        RunBudne("alt1");
    }

    [TestMethod]
    public void TEST_Budne_alt2()
    {
        RunBudne("alt2");
    }

    [TestMethod]
    public void TEST_Budne_any()
    {
        RunBudne("any");
    }

    [TestMethod]
    public void TEST_Budne_atn()
    {
        RunBudne("atn");
    }

    [TestMethod]
    public void TEST_Budne_bal()
    {
        RunBudne("bal");
    }

    [TestMethod]
    public void TEST_Budne_base()
    {
        RunBudne("base");
    }

    [TestMethod]
    public void TEST_Budne_breakx()
    {
        RunBudne("breakx");
    }

    [TestMethod]
    public void TEST_Budne_case1()
    {
        RunBudne("case1");
    }

    [TestMethod]
    public void TEST_Budne_case2()
    {
        RunBudne("case2");
    }

    [TestMethod]
    public void TEST_Budne_cat()
    {
        RunBudne("cat");
    }

    [TestMethod]
    public void TEST_Budne_char()
    {
        RunBudne("char");
    }

    [TestMethod]
    public void TEST_Budne_collect()
    {
        RunBudne("collect");
    }

    [TestMethod]
    public void TEST_Budne_collect2()
    {
        RunBudne("collect2");
    }

    [TestMethod]
    public void TEST_Budne_comment()
    {
        RunBudne("comment");
    }

    [TestMethod]
    public void TEST_Budne_contin()
    {
        RunBudne("contin");
    }

    [TestMethod]
    public void TEST_Budne_conv2()
    {
        RunBudne("conv2");
    }

    [TestMethod]
    public void TEST_Budne_convert()
    {
        RunBudne("convert");
    }

    [TestMethod]
    public void TEST_Budne_crlf()
    {
        RunBudne("crlf");
    }

    [TestMethod]
    public void TEST_Budne_diag1()
    {
        RunBudne("diag1");
    }

    [TestMethod]
    public void TEST_Budne_diag2()
    {
        RunBudne("diag2");
    }

    [TestMethod]
    public void TEST_Budne_digits()
    {
        RunBudne("digits");
    }

    [TestMethod]
    public void TEST_Budne_dump()
    {
        RunBudne("dump");
    }

    [TestMethod]
    public void TEST_Budne_end()
    {
        RunBudne("end");
    }

    [TestMethod]
    public void TEST_Budne_err()
    {
        RunBudne("err");
    }

    [TestMethod]
    public void TEST_Budne_fact()
    {
        RunBudne("fact");
    }

    [TestMethod]
    public void TEST_Budne_factor()
    {
        RunBudne("factor");
    }

    [TestMethod]
    public void TEST_Budne_file()
    {
        RunBudne("file");
    }

    [TestMethod]
    public void TEST_Budne_float()
    {
        RunBudne("float");
    }

    [TestMethod]
    public void TEST_Budne_float2()
    {
        RunBudne("float2");
    }

    [TestMethod]
    public void TEST_Budne_ftrace()
    {
        RunBudne("ftrace");
    }

    [TestMethod]
    public void TEST_Budne_fun1()
    {
        RunBudne("fun1");
    }

    [TestMethod]
    public void TEST_Budne_fun2()
    {
        RunBudne("fun2");
    }

    [TestMethod]
    public void TEST_Budne_func2()
    {
        RunBudne("func2");
    }

    [TestMethod]
    public void TEST_Budne_function()
    {
        RunBudne("function");
    }

    [TestMethod]
    public void TEST_Budne_hello()
    {
        RunBudne("hello");
    }

    [TestMethod]
    public void TEST_Budne_hide()
    {
        RunBudne("hide");
    }

    [TestMethod]
    public void TEST_Budne_include()
    {
        RunBudne("include");
    }

    [TestMethod]
    public void TEST_Budne_include2()
    {
        RunBudne("include2");
    }

    [TestMethod]
    public void TEST_Budne_include3()
    {
        RunBudne("include3");
    }

    [TestMethod]
    public void TEST_Budne_include4()
    {
        RunBudne("include4");
    }

    [TestMethod]
    public void TEST_Budne_ind()
    {
        RunBudne("ind");
    }

    [TestMethod]
    public void TEST_Budne_intval()
    {
        RunBudne("intval");
    }

    [TestMethod]
    public void TEST_Budne_json1()
    {
        RunBudne("json1");
    }

    [TestMethod]
    public void TEST_Budne_keytrace()
    {
        RunBudne("keytrace");
    }

    [TestMethod]
    public void TEST_Budne_label()
    {
        RunBudne("label");
    }

    [TestMethod]
    public void TEST_Budne_labelcode()
    {
        RunBudne("labelcode");
    }

    [TestMethod]
    public void TEST_Budne_len()
    {
        RunBudne("len");
    }

    [TestMethod]
    public void TEST_Budne_lexcmp()
    {
        RunBudne("lexcmp");
    }

    [TestMethod]
    public void TEST_Budne_lgt()
    {
        RunBudne("lgt");
    }

    [TestMethod]
    public void TEST_Budne_line()
    {
        RunBudne("line");
    }

    [TestMethod]
    public void TEST_Budne_loaderr()
    {
        RunBudne("loaderr");
    }

    [TestMethod]
    public void TEST_Budne_local()
    {
        RunBudne("local");
    }

    [TestMethod]
    public void TEST_Budne_longline()
    {
        RunBudne("longline");
    }

    [TestMethod]
    public void TEST_Budne_longrec()
    {
        RunBudne("longrec");
    }

    [TestMethod]
    public void TEST_Budne_loop()
    {
        RunBudne("loop");
    }

    [TestMethod]
    public void TEST_Budne_match()
    {
        RunBudne("match");
    }

    [TestMethod]
    public void TEST_Budne_match2()
    {
        RunBudne("match2");
    }

    [TestMethod]
    public void TEST_Budne_match3()
    {
        RunBudne("match3");
    }

    [TestMethod]
    public void TEST_Budne_match4()
    {
        RunBudne("match4");
    }

    [TestMethod]
    public void TEST_Budne_matchloop()
    {
        RunBudne("matchloop");
    }

    [TestMethod]
    public void TEST_Budne_maxint()
    {
        RunBudne("maxint");
    }

    [TestMethod]
    public void TEST_Budne_noexec()
    {
        RunBudne("noexec");
    }

    [TestMethod]
    public void TEST_Budne_nqueens()
    {
        RunBudne("nqueens");
    }

    [TestMethod]
    public void TEST_Budne_openi()
    {
        RunBudne("openi");
    }

    [TestMethod]
    public void TEST_Budne_openo()
    {
        RunBudne("openo");
    }

    [TestMethod]
    public void TEST_Budne_openo2()
    {
        RunBudne("openo2");
    }

    [TestMethod]
    public void TEST_Budne_ops()
    {
        RunBudne("ops");
    }

    [TestMethod]
    public void TEST_Budne_ord()
    {
        RunBudne("ord");
    }

    [TestMethod]
    public void TEST_Budne_pad()
    {
        RunBudne("pad");
    }

    [TestMethod]
    public void TEST_Budne_popen()
    {
        RunBudne("popen");
    }

    [TestMethod]
    public void TEST_Budne_popen2()
    {
        RunBudne("popen2");
    }

    [TestMethod]
    public void TEST_Budne_pow()
    {
        RunBudne("pow");
    }

    [TestMethod]
    public void TEST_Budne_preload1()
    {
        RunBudne("preload1");
    }

    [TestMethod]
    public void TEST_Budne_preload2()
    {
        RunBudne("preload2");
    }

    [TestMethod]
    public void TEST_Budne_preload3()
    {
        RunBudne("preload3");
    }

    [TestMethod]
    public void TEST_Budne_preload4()
    {
        RunBudne("preload4");
    }

    [TestMethod]
    public void TEST_Budne_punch()
    {
        RunBudne("punch");
    }

    [TestMethod]
    public void TEST_Budne_random()
    {
        RunBudne("random");
    }

    [TestMethod]
    public void TEST_Budne_repl()
    {
        RunBudne("repl");
    }

    [TestMethod]
    public void TEST_Budne_reverse()
    {
        RunBudne("reverse");
    }

    [TestMethod]
    public void TEST_Budne_rewind1()
    {
        RunBudne("rewind1");
    }

    [TestMethod]
    public void TEST_Budne_roman()
    {
        RunBudne("roman");
    }

    [TestMethod]
    public void TEST_Budne_scanerr()
    {
        RunBudne("scanerr");
    }

    [TestMethod]
    public void TEST_Budne_setexit()
    {
        RunBudne("setexit");
    }

    [TestMethod]
    public void TEST_Budne_setexit2()
    {
        RunBudne("setexit2");
    }

    [TestMethod]
    public void TEST_Budne_setexit3()
    {
        RunBudne("setexit3");
    }

    [TestMethod]
    public void TEST_Budne_setexit4()
    {
        RunBudne("setexit4");
    }

    [TestMethod]
    public void TEST_Budne_setexit5()
    {
        RunBudne("setexit5");
    }

    [TestMethod]
    public void TEST_Budne_setexit6()
    {
        RunBudne("setexit6");
    }

    [TestMethod]
    public void TEST_Budne_setexit7()
    {
        RunBudne("setexit7");
    }

    [TestMethod]
    public void TEST_Budne_space()
    {
        RunBudne("space");
    }

    [TestMethod]
    public void TEST_Budne_space2()
    {
        RunBudne("space2");
    }

    [TestMethod]
    public void TEST_Budne_spit()
    {
        RunBudne("spit");
    }

    [TestMethod]
    public void TEST_Budne_str()
    {
        RunBudne("str");
    }

    [TestMethod]
    public void TEST_Budne_substr()
    {
        RunBudne("substr");
    }

    [TestMethod]
    public void TEST_Budne_sudoku()
    {
        RunBudne("sudoku");
    }

    [TestMethod]
    public void TEST_Budne_t()
    {
        RunBudne("t");
    }

    [TestMethod]
    public void TEST_Budne_tab()
    {
        RunBudne("tab");
    }

    [TestMethod]
    public void TEST_Budne_trace1()
    {
        RunBudne("trace1");
    }

    [TestMethod]
    public void TEST_Budne_trace2()
    {
        RunBudne("trace2");
    }

    [TestMethod]
    public void TEST_Budne_trfunc()
    {
        RunBudne("trfunc");
    }

    [TestMethod]
    public void TEST_Budne_trim0()
    {
        RunBudne("trim0");
    }

    [TestMethod]
    public void TEST_Budne_trim1()
    {
        RunBudne("trim1");
    }

    [TestMethod]
    public void TEST_Budne_uneval()
    {
        RunBudne("uneval");
    }

    [TestMethod]
    public void TEST_Budne_uneval2()
    {
        RunBudne("uneval2");
    }

    [TestMethod]
    public void TEST_Budne_unsc()
    {
        RunBudne("unsc");
    }

    [TestMethod]
    public void TEST_Budne_update()
    {
        RunBudne("update");
    }

    [TestMethod]
    public void TEST_Budne_vdiffer()
    {
        RunBudne("vdiffer");
    }

    [TestMethod]
    public void TEST_Budne_words()
    {
        RunBudne("words");
    }

    [TestMethod]
    public void TEST_Budne_words1()
    {
        RunBudne("words1");
    }

}
