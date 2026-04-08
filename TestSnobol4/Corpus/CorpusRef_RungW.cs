using Test.TestLexer;

// ReSharper disable StringLiteralTypo

namespace Test.Corpus;

/// <summary>
/// Corpus rungW01–W07 — Byrd Box pattern coverage rungs.
/// All use PASS/FAIL OUTPUT convention; compared against .ref oracle.
/// </summary>
[DoNotParallelize]
[TestClass]
public class CorpusRef_RungW
{
    // ── W01: Literal pattern ─────────────────────────────────────────────

    [TestMethod]
    public void TEST_Corpus_W01_pat_lit_basic()
    {
        var s = @"
        subject = 'hello world'
        subject 'world'                    :f(e001)
        output = 'PASS W01/001: literal match succeeded'   :(end)
e001    output = 'FAIL W01/001: literal match should succeed'
end";
        Assert.AreEqual("PASS W01/001: literal match succeeded", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_W01_pat_lit_fail()
    {
        var s = @"
        subject = 'hello world'
        subject 'xyz'                      :s(e001)
        output = 'PASS W01/002: literal mismatch failed correctly'   :(end)
e001    output = 'FAIL W01/002: literal mismatch should fail'
end";
        Assert.AreEqual("PASS W01/002: literal mismatch failed correctly", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_W01_pat_lit_anchor()
    {
        var s = @"
        subject = 'hello world'
        subject 'world'                    :f(e001)
        output = 'PASS W01/003: unanchored substring match'  :(end)
e001    output = 'FAIL W01/003: unanchored substring match should succeed'
end";
        Assert.AreEqual("PASS W01/003: unanchored substring match", SetupTests.RunWithInput(s));
    }

    // ── W02: Sequential pattern ──────────────────────────────────────────

    [TestMethod]
    public void TEST_Corpus_W02_seq_basic()
    {
        var s = @"
        subject = 'foobar'
        pat = 'foo' 'bar'
        subject pat              :f(e001)
        OUTPUT = 'PASS W02/001: seq match succeeded'         :(END)
e001    OUTPUT = 'FAIL W02/001: seq match should succeed'
END";
        Assert.AreEqual("PASS W02/001: seq match succeeded", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_W02_seq_nested()
    {
        var s = @"
        subject = 'xabc'
        pat = 'a' 'b' 'c'
        subject pat              :f(e001)
        OUTPUT = 'PASS W02/002: nested seq match succeeded'  :(END)
e001    OUTPUT = 'FAIL W02/002: nested seq match should succeed'
END";
        Assert.AreEqual("PASS W02/002: nested seq match succeeded", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_W02_seq_fail_propagate()
    {
        var s = @"
        subject = 'foobar'
        pat = 'foo' 'xyz'
        subject pat              :s(e001)
        OUTPUT = 'PASS W02/003: seq rightly failed'          :(END)
e001    OUTPUT = 'FAIL W02/003: seq should have failed'
END";
        Assert.AreEqual("PASS W02/003: seq rightly failed", SetupTests.RunWithInput(s));
    }

    // ── W03: Alternation ─────────────────────────────────────────────────

    [TestMethod]
    public void TEST_Corpus_W03_alt_basic()
    {
        var s = @"
        subject = 'foobar'
        subject ('foo' | 'baz')  :f(e001)
        OUTPUT = 'PASS W03/001: alt first branch matched'    :(END)
e001    OUTPUT = 'FAIL W03/001: alt first branch should match'
END";
        Assert.AreEqual("PASS W03/001: alt first branch matched", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_W03_alt_second()
    {
        var s = @"
        subject = 'foobar'
        pat = 'baz' | 'bar'
        subject pat              :f(e001)
        OUTPUT = 'PASS W03/002: alt second branch matched'   :(END)
e001    OUTPUT = 'FAIL W03/002: alt second branch should match'
END";
        Assert.AreEqual("PASS W03/002: alt second branch matched", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_W03_alt_both_fail()
    {
        var s = @"
        subject = 'foobar'
        subject ('baz' | 'qux')  :s(e001)
        OUTPUT = 'PASS W03/003: alt both fail as expected'   :(END)
e001    OUTPUT = 'FAIL W03/003: alt should have failed but succeeded'
END";
        Assert.AreEqual("PASS W03/003: alt both fail as expected", SetupTests.RunWithInput(s));
    }

    // ── W04: ARBNO ───────────────────────────────────────────────────────

    [TestMethod]
    public void TEST_Corpus_W04_arbno_basic()
    {
        var s = @"
        subject = 'ababX'
        subject (ARBNO('ab') 'X')  :f(e001)
        OUTPUT = 'PASS W04/001: arbno matched'   :(END)
e001    OUTPUT = 'FAIL W04/001: arbno should match'
END";
        Assert.AreEqual("PASS W04/001: arbno matched", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_W04_arbno_zero()
    {
        var s = @"
        subject = 'hello'
        subject (ARBNO('xyz') 'hello')  :f(e001)
        OUTPUT = 'PASS W04/002: arbno zero match'   :(END)
e001    OUTPUT = 'FAIL W04/002: arbno zero should succeed'
END";
        Assert.AreEqual("PASS W04/002: arbno zero match", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_W04_arbno_backtrack()
    {
        var s = @"
        subject = 'xxx'
        subject (ARBNO('ab') 'xxx')  :f(e001)
        OUTPUT = 'PASS W04/003: arbno zero then literal'   :(END)
e001    OUTPUT = 'FAIL W04/003: should have matched'
END";
        Assert.AreEqual("PASS W04/003: arbno zero then literal", SetupTests.RunWithInput(s));
    }

    // ── W05: Primitive character patterns ────────────────────────────────

    [TestMethod]
    public void TEST_Corpus_W05_any()
    {
        var s = @"
        subject = 'hello'
        subject ANY('aeiou')  :f(e001)
        OUTPUT = 'PASS W05/001: any matched'   :(END)
e001    OUTPUT = 'FAIL W05/001: any should match'
END";
        Assert.AreEqual("PASS W05/001: any matched", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_W05_notany()
    {
        var s = @"
        subject = 'hello'
        subject NOTANY('aeiou')  :f(e001)
        OUTPUT = 'PASS W05/002: notany matched'   :(END)
e001    OUTPUT = 'FAIL W05/002: notany should match'
END";
        Assert.AreEqual("PASS W05/002: notany matched", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_W05_span()
    {
        var s = @"
        subject = '42abc'
        subject SPAN('0123456789')  :f(e001)
        OUTPUT = 'PASS W05/003: span matched'   :(END)
e001    OUTPUT = 'FAIL W05/003: span should match'
END";
        Assert.AreEqual("PASS W05/003: span matched", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_W05_break()
    {
        var s = @"
        subject = 'key:value'
        subject BREAK(':')  :f(e001)
        OUTPUT = 'PASS W05/004: break matched'   :(END)
e001    OUTPUT = 'FAIL W05/004: break should match'
END";
        Assert.AreEqual("PASS W05/004: break matched", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_W05_breakx()
    {
        var s = @"
        subject = 'key:value'
        subject BREAKX(':')  :f(e001)
        OUTPUT = 'PASS W05/005: breakx matched'   :(END)
e001    OUTPUT = 'FAIL W05/005: breakx should match'
END";
        Assert.AreEqual("PASS W05/005: breakx matched", SetupTests.RunWithInput(s));
    }

    // ── W06: Position/length primitives ──────────────────────────────────

    [TestMethod]
    public void TEST_Corpus_W06_len()
    {
        var s = @"
        subject = 'ABCDE'
        subject LEN(3) 'DE'  :f(e001)
        OUTPUT = 'PASS W06/001: LEN(3) then DE matched'   :(t002)
e001    OUTPUT = 'FAIL W06/001: LEN(3) then DE should match'
t002    subject = 'ABCDE'
        subject LEN(0) 'ABCDE'  :f(e002)
        OUTPUT = 'PASS W06/002: LEN(0) then full string matched'  :(t003)
e002    OUTPUT = 'FAIL W06/002: LEN(0) then full string should match'
t003    subject = 'AB'
        subject LEN(5)  :s(e003)
        OUTPUT = 'PASS W06/003: LEN(5) on 2-char string correctly fails'  :(END)
e003    OUTPUT = 'FAIL W06/003: LEN(5) on short string should fail'
END";
        Assert.AreEqual(
            "PASS W06/001: LEN(3) then DE matched\nPASS W06/002: LEN(0) then full string matched\nPASS W06/003: LEN(5) on 2-char string correctly fails",
            SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_W06_pos()
    {
        var s = @"
        subject = 'HELLO'
        subject POS(0) 'H'  :f(e001)
        OUTPUT = 'PASS W06/001: POS(0) matched'   :(t002)
e001    OUTPUT = 'FAIL W06/001: POS(0) should match'
t002    subject = 'HELLO'
        subject 'EL' POS(3)  :f(e002)
        OUTPUT = 'PASS W06/002: POS(3) after EL matched'  :(t003)
e002    OUTPUT = 'FAIL W06/002: POS(3) after EL should match'
t003    subject = 'HELLO'
        subject POS(2) 'H'  :s(e003)
        OUTPUT = 'PASS W06/003: POS(2) H correctly fails'  :(END)
e003    OUTPUT = 'FAIL W06/003: POS(2) H should fail'
END";
        Assert.AreEqual(
            "PASS W06/001: POS(0) matched\nPASS W06/002: POS(3) after EL matched\nPASS W06/003: POS(2) H correctly fails",
            SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_W06_rpos()
    {
        var s = @"
        subject = 'HELLO'
        subject 'HELLO' RPOS(0)  :f(e001)
        OUTPUT = 'PASS W06/001: RPOS(0) at end matched'   :(t002)
e001    OUTPUT = 'FAIL W06/001: RPOS(0) at end should match'
t002    subject = 'HELLO'
        subject 'HEL' RPOS(2)  :f(e002)
        OUTPUT = 'PASS W06/002: RPOS(2) after HEL matched'  :(t003)
e002    OUTPUT = 'FAIL W06/002: RPOS(2) after HEL should match'
t003    subject = 'HELLO'
        subject RPOS(1) 'LO'  :s(e003)
        OUTPUT = 'PASS W06/003: RPOS(1) LO correctly fails'  :(END)
e003    OUTPUT = 'FAIL W06/003: RPOS(1) LO should fail'
END";
        Assert.AreEqual(
            "PASS W06/001: RPOS(0) at end matched\nPASS W06/002: RPOS(2) after HEL matched\nPASS W06/003: RPOS(1) LO correctly fails",
            SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_W06_tab()
    {
        var s = @"
        subject = 'ABCDE'
        subject TAB(3) 'DE'  :f(e001)
        OUTPUT = 'PASS W06/001: TAB(3) then DE matched'   :(t002)
e001    OUTPUT = 'FAIL W06/001: TAB(3) then DE should match'
t002    subject = 'ABCDE'
        subject 'AB' TAB(2) 'CDE'  :f(e002)
        OUTPUT = 'PASS W06/002: AB TAB(2) CDE matched'  :(t003)
e002    OUTPUT = 'FAIL W06/002: AB TAB(2) CDE should match'
t003    subject = 'ABCDE'
        subject 'ABC' TAB(1)  :s(e003)
        OUTPUT = 'PASS W06/003: TAB(1) behind cursor correctly fails'  :(END)
e003    OUTPUT = 'FAIL W06/003: TAB(1) behind cursor should fail'
END";
        Assert.AreEqual(
            "PASS W06/001: TAB(3) then DE matched\nPASS W06/002: AB TAB(2) CDE matched\nPASS W06/003: TAB(1) behind cursor correctly fails",
            SetupTests.RunWithInput(s));
    }

    // ── W07: Captures ────────────────────────────────────────────────────

    [TestMethod]
    public void TEST_Corpus_W07_capt_imm()
    {
        var s = @"
        subject = 'FOOBAR'
        subject ('FOO' $ cap1)  :f(e001)
        OUTPUT = 'PASS W07/001: imm cap=' cap1   :(t002)
e001    OUTPUT = 'FAIL W07/001: imm capture should match'
t002    subject = 'FOOBAR'
        subject ('BAR' $ cap2)  :f(e002)
        OUTPUT = 'PASS W07/002: imm cap BAR=' cap2   :(END)
e002    OUTPUT = 'FAIL W07/002: BAR capture should match'
END";
        Assert.AreEqual("PASS W07/001: imm cap=FOO\nPASS W07/002: imm cap BAR=BAR", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_W07_capt_cond()
    {
        var s = @"
        subject = 'HELLO WORLD'
        subject ('HELLO' . cap1)  :f(e001)
        OUTPUT = 'PASS W07/001: cond cap=' cap1   :(t002)
e001    OUTPUT = 'FAIL W07/001: cond capture should match'
t002    subject = 'ABC'
        subject ('XYZ' . cap2)  :s(e002)
        OUTPUT = 'PASS W07/002: cond cap correctly fails'  :(END)
e002    OUTPUT = 'FAIL W07/002: failed match should not succeed'
END";
        Assert.AreEqual("PASS W07/001: cond cap=HELLO\nPASS W07/002: cond cap correctly fails", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_W07_capt_chain()
    {
        var s = @"
        subject = 'ABCDEF'
        subject (('ABC' . c1) ('DEF' . c2))  :f(e001)
        OUTPUT = 'PASS W07/001: c1=' c1 ' c2=' c2   :(END)
e001    OUTPUT = 'FAIL W07/001: chained captures should match'
END";
        Assert.AreEqual("PASS W07/001: c1=ABC c2=DEF", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_W07_capt_fail()
    {
        var s = @"
        subject = 'HELLO'
        subject ('WORLD' . cap1)  :s(e001)
        OUTPUT = 'PASS W07/001: no match, cap1 unchanged'  :(END)
e001    OUTPUT = 'FAIL W07/001: failed pattern should not branch success'
END";
        Assert.AreEqual("PASS W07/001: no match, cap1 unchanged", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_W07_capt_cur()
    {
        // @N cursor capture — this is the M-NET-P35-FIX bug target.
        // Expected: PASS W07/001: cursor pos=3  and  PASS W07/002: cursor after match=5
        var s = @"
        subject = 'ABCDE'
        subject ('ABC' @pos1)  :f(e001)
        OUTPUT = 'PASS W07/001: cursor pos=' pos1   :(t002)
e001    OUTPUT = 'FAIL W07/001: cursor capture should match'
t002    subject = 'ABCDE'
        subject ('ABCDE' @pos2)  :f(e002)
        OUTPUT = 'PASS W07/002: cursor after match=' pos2   :(END)
e002    OUTPUT = 'FAIL W07/002: cursor after match should match'
END";
        Assert.AreEqual("PASS W07/001: cursor pos=3\nPASS W07/002: cursor after match=5", SetupTests.RunWithInput(s));
    }
}
