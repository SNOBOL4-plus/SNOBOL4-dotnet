using Test.TestLexer;

// ReSharper disable StringLiteralTypo

namespace Test.Corpus;

/// <summary>
/// Tests for corpus/lib/string.sno functions inlined (no -INCLUDE).
/// Functions: pad_left, pad_right, ltrim, rtrim, trimws, repeat,
///            contains, startswith, endswith, index.
/// </summary>
[DoNotParallelize]
[TestClass]
public class CorpusRef_LibString
{
    private const string StringLib = @"
               DEFINE('pad_left(s,n,c)')                               :(pad_left_end)
pad_left       c              =   IDENT(c) ' '
               pad_left       =   GE(SIZE(s),n) s                 :S(RETURN)
               pad_left       =   DUPL(c, n - SIZE(s)) s           :(RETURN)
pad_left_end

               DEFINE('pad_right(s,n,c)')                              :(pad_right_end)
pad_right      c              =   IDENT(c) ' '
               pad_right      =   GE(SIZE(s),n) s                 :S(RETURN)
               pad_right      =   s DUPL(c, n - SIZE(s))           :(RETURN)
pad_right_end

               DEFINE('ltrim(s)ws,r')                              :(ltrim_end)
ltrim          ws             =   ' ' CHAR(9) CHAR(10) CHAR(13)
               s POS(0) (SPAN(ws) | '') REM . r =
               ltrim          =   r                                :(RETURN)
ltrim_end

               DEFINE('rtrim(s)ws,i,ch')                           :(rtrim_end)
rtrim          ws             =   ' ' CHAR(9) CHAR(10) CHAR(13)
               i              =   SIZE(s)
rtrim0         LE(i, 0)                                            :S(rtrim1)
               ch             =   SUBSTR(s, i, 1)
               ch ANY(ws)                                          :F(rtrim1)
               i              =   i - 1                            :(rtrim0)
rtrim1         rtrim          =   SUBSTR(s, 1, i)                  :(RETURN)
rtrim_end

               DEFINE('trimws(s)')                                  :(trimws_end)
trimws         trimws         =   ltrim(rtrim(s))                  :(RETURN)
trimws_end

               DEFINE('repeat(s,n)')                               :(repeat_end)
repeat         repeat         =   DUPL(s, n)                       :(RETURN)
repeat_end

               DEFINE('contains(s,t)')                             :(contains_end)
contains       s BREAK(t) t                                        :S(RETURN)F(FRETURN)
contains_end

               DEFINE('startswith(s,t)')                           :(startswith_end)
startswith     s POS(0) t                                          :S(RETURN)F(FRETURN)
startswith_end

               DEFINE('endswith(s,t)')                             :(endswith_end)
endswith       s t RPOS(0)                                         :S(RETURN)F(FRETURN)
endswith_end

               DEFINE('index(s,t)ix')                              :(index_end)
index          index          =   0
               ix             =   s
               ix BREAK(t) . ix                                    :F(RETURN)
               index          =   SIZE(ix) + 1                     :(RETURN)
index_end
";

    [TestMethod]
    public void TEST_Corpus_lib_string_pad_left()
    {
        var s = StringLib + @"
        OUTPUT = pad_left('hi', 6, '*')
        OUTPUT = pad_left('hi', 6)
        OUTPUT = pad_left('toolong', 4, '*')
END";
        Assert.AreEqual("****hi"+ Environment.NewLine + "    hi"+ Environment.NewLine + "toolong", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_lib_string_pad_right()
    {
        var s = StringLib + @"
        OUTPUT = pad_right('hi', 6, '*')
        OUTPUT = pad_right('hi', 6)
        OUTPUT = pad_right('toolong', 4, '*')
END";
        Assert.AreEqual("hi****"+ Environment.NewLine + "hi    "+ Environment.NewLine + "toolong", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_lib_string_ltrim()
    {
        var s = StringLib + @"
        OUTPUT = ltrim('   hello')
        OUTPUT = ltrim('hello')
END";
        Assert.AreEqual("hello"+ Environment.NewLine + "hello", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_lib_string_rtrim()
    {
        var s = StringLib + @"
        OUTPUT = rtrim('hello   ')
        OUTPUT = rtrim('hello')
END";
        Assert.AreEqual("hello"+ Environment.NewLine + "hello", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_lib_string_trimws()
    {
        var s = StringLib + @"
        OUTPUT = trimws('  hello  ')
        OUTPUT = trimws('hello')
END";
        Assert.AreEqual("hello"+ Environment.NewLine + "hello", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_lib_string_repeat()
    {
        var s = StringLib + @"
        OUTPUT = repeat('hi', 3)
        OUTPUT = repeat('ab', 0)
        OUTPUT = SIZE(repeat('x', 5))
END";
        Assert.AreEqual("hihihi"+ Environment.NewLine + ""+ Environment.NewLine + "5", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_lib_string_contains()
    {
        var s = StringLib + @"
        contains('foobar', 'oba')                  :F(c1fail)
        OUTPUT = 'contains ok'
        :(c1)
c1fail  OUTPUT = 'FAIL: contains'
c1
        contains('foobar', 'xyz')                  :S(c2fail)
        OUTPUT = 'not found ok'
        :(END)
c2fail  OUTPUT = 'FAIL: should not contain'
END";
        Assert.AreEqual("contains ok"+ Environment.NewLine + "not found ok", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_lib_string_startswith_endswith()
    {
        var s = StringLib + @"
        startswith('foobar', 'foo')                :F(sw1fail)
        OUTPUT = 'startswith ok'
        :(sw1)
sw1fail OUTPUT = 'FAIL: startswith'
sw1
        endswith('foobar', 'bar')                  :F(ew1fail)
        OUTPUT = 'endswith ok'
        :(ew1)
ew1fail OUTPUT = 'FAIL: endswith'
ew1
        startswith('foobar', 'bar')                :S(sw2fail)
        OUTPUT = 'no startswith ok'
        :(sw2)
sw2fail OUTPUT = 'FAIL: startswith matched wrong'
sw2
END";
        Assert.AreEqual("startswith ok"+ Environment.NewLine + "endswith ok"+ Environment.NewLine + "no startswith ok", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_lib_string_index()
    {
        var s = StringLib + @"
        OUTPUT = index('foobar', 'oba')
        OUTPUT = index('foobar', 'xyz')
        OUTPUT = index('hello', 'h')
END";
        // Note: index uses BREAK(t) which is character-set based, not substring.
        // BREAK('oba') in 'foobar' stops at first char in {o,b,a} = 'o' at pos 2 → returns 2.
        Assert.AreEqual("2"+ Environment.NewLine + "0"+ Environment.NewLine + "1", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_lib_string_all()
    {
        // Full oracle match from test_string.ref
        var s = StringLib + @"
        &TRIM = 1
        OUTPUT = pad_left('hi', 6, '*')
        OUTPUT = pad_right('hi', 6, '*')
        OUTPUT = ltrim('   hello')
        OUTPUT = rtrim('hello   ')
        OUTPUT = trimws('  hello  ')
        OUTPUT = repeat('hi', 3)
        contains('foobar', 'oba')                  :F(bad_c1)
        OUTPUT = 'contains ok'
        :(c1)
bad_c1  OUTPUT = 'FAIL: contains'
c1
        startswith('foobar', 'foo')                :F(bad_sw1)
        OUTPUT = 'startswith ok'
        :(sw1)
bad_sw1 OUTPUT = 'FAIL: startswith'
sw1
        endswith('foobar', 'bar')                  :F(bad_ew1)
        OUTPUT = 'endswith ok'
        :(ew1)
bad_ew1 OUTPUT = 'FAIL: endswith'
ew1
        startswith('foobar', 'bar')                :S(bad_sw2)
        OUTPUT = 'no startswith ok'
        :(sw2)
bad_sw2 OUTPUT = 'FAIL: startswith matched wrong'
sw2
        OUTPUT = index('foobar', 'oba')
        OUTPUT = index('foobar', 'xyz')
END";
        Assert.AreEqual(
            "****hi"+ Environment.NewLine + "hi****"+ Environment.NewLine + "hello"+ Environment.NewLine + "hello"+ Environment.NewLine + "hello"+ Environment.NewLine + "hihihi"+ Environment.NewLine + "" +
            "contains ok"+ Environment.NewLine + "startswith ok"+ Environment.NewLine + "endswith ok"+ Environment.NewLine + "no startswith ok"+ Environment.NewLine + "2"+ Environment.NewLine + "0",
            SetupTests.RunWithInput(s));
    }
}
