using Test.TestLexer;

// ReSharper disable StringLiteralTypo

namespace Test.Corpus;

/// <summary>
/// Tests for corpus/lib/stack.sno functions inlined (no -INCLUDE).
/// Functions: stack_init, stack_push, stack_pop, stack_peek, stack_top, stack_depth.
/// </summary>
[DoNotParallelize]
[TestClass]
public class CorpusRef_LibStack
{
    private const string StackLib = @"
               DATA('slink(snext,sval)')

               DEFINE('stack_init()')                              :(stack_init_end)
stack_init     stk            =                                    :(RETURN)
stack_init_end

               DEFINE('stack_push(x)')                            :(stack_push_end)
stack_push     stk            =   slink(stk, x)
               stack_push     =   .sval(stk)                      :(NRETURN)
stack_push_end

               DEFINE('stack_pop(var)')                           :(stack_pop_end)
stack_pop      DIFFER(stk)                                        :F(FRETURN)
               IDENT(var)                                         :F(stack_pop1)
               stack_pop      =   sval(stk)
               stk            =   snext(stk)                      :(RETURN)
stack_pop1     $var           =   sval(stk)
               stk            =   snext(stk)
               stack_pop      =   .dummy                          :(NRETURN)
stack_pop_end

               DEFINE('stack_peek()')                             :(stack_peek_end)
stack_peek     DIFFER(stk)                                        :F(FRETURN)
               stack_peek     =   sval(stk)                       :(RETURN)
stack_peek_end

               DEFINE('stack_top()')                              :(stack_top_end)
stack_top      DIFFER(stk)                                        :F(FRETURN)
               stack_top      =   .sval(stk)                      :(NRETURN)
stack_top_end

               DEFINE('stack_depth()sd')                          :(stack_depth_end)
stack_depth    stack_depth    =   0
               sd             =   stk
stk_dep0       DIFFER(sd)                                         :F(RETURN)
               stack_depth    =   stack_depth + 1
               sd             =   snext(sd)                       :(stk_dep0)
stack_depth_end
";

    [TestMethod]
    public void TEST_Corpus_lib_stack_push_pop_basic()
    {
        var s = StackLib + @"
        &TRIM = 1
        stack_init()
        stack_push('a')
        stack_push('b')
        stack_push('c')
        OUTPUT = stack_depth()
        OUTPUT = stack_pop()
        OUTPUT = stack_pop()
        OUTPUT = stack_depth()
        OUTPUT = stack_pop()
        OUTPUT = stack_depth()
END";
        Assert.AreEqual("3"+ Environment.NewLine + "c"+ Environment.NewLine + "b"+ Environment.NewLine + "1"+ Environment.NewLine + "a"+ Environment.NewLine + "0", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_lib_stack_empty_freturn()
    {
        // Popping empty stack should FRETURN
        var s = StackLib + @"
        &TRIM = 1
        stack_init()
        stack_pop()                                :S(bad)
        OUTPUT = 'empty ok'
        :(END)
bad     OUTPUT = 'FAIL: empty pop should FRETURN'
END";
        Assert.AreEqual("empty ok", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_lib_stack_peek_no_pop()
    {
        // peek returns top without removing it
        var s = StackLib + @"
        &TRIM = 1
        stack_init()
        stack_push('x')
        OUTPUT = stack_peek()
        OUTPUT = stack_depth()
        OUTPUT = stack_pop()
END";
        Assert.AreEqual("x"+ Environment.NewLine + "1"+ Environment.NewLine + "x", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_lib_stack_pop_into_var()
    {
        // stack_pop('varname') stores result in named variable
        var s = StackLib + @"
        &TRIM = 1
        stack_init()
        stack_push(42)
        stack_push(99)
        stack_pop('myvar')
        OUTPUT = myvar
END";
        Assert.AreEqual("99", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_lib_stack_from_pattern()
    {
        // Push values captured from a pattern match
        var s = StackLib + @"
        &TRIM = 1
        stack_init()
        subject = 'hello world'
        subject BREAK(' ') . w1 ' ' REM . w2 =    :F(bad)
        stack_push(w1)
        stack_push(w2)
        OUTPUT = stack_pop()
        OUTPUT = stack_pop()
        :(END)
bad     OUTPUT = 'FAIL: pattern match failed'
END";
        Assert.AreEqual("world"+ Environment.NewLine + "hello", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_lib_stack_integers()
    {
        // Stack works with integers
        var s = StackLib + @"
        &TRIM = 1
        stack_init()
        stack_push(10)
        stack_push(20)
        stack_push(30)
        OUTPUT = stack_pop() + stack_pop() + stack_pop()
END";
        Assert.AreEqual("60", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_lib_stack_all()
    {
        // Full oracle match from test_stack.ref
        var s = StackLib + @"
        &TRIM = 1
        stack_init()
        stack_push('a')
        stack_push('b')
        stack_push('c')
        OUTPUT = stack_depth()
        OUTPUT = stack_pop()
        OUTPUT = stack_pop()
        OUTPUT = stack_depth()
        OUTPUT = stack_pop()
        OUTPUT = stack_depth()
        stack_pop()                                :S(bad_s1)
        OUTPUT = 'empty ok'
        :(s1)
bad_s1  OUTPUT = 'FAIL: empty pop should FRETURN'
s1
        stack_init()
        stack_push('x')
        OUTPUT = stack_peek()
        OUTPUT = stack_depth()
        OUTPUT = stack_pop()
        stack_init()
        stack_push(42)
        stack_push(99)
        stack_pop('myvar')
        OUTPUT = myvar
        stack_init()
        subject = 'hello world'
        subject BREAK(' ') . w1 ' ' REM . w2 =    :F(bad_s2)
        stack_push(w1)
        stack_push(w2)
        OUTPUT = stack_pop()
        OUTPUT = stack_pop()
        :(END)
bad_s2  OUTPUT = 'FAIL: pattern match failed'
END";
        Assert.AreEqual("3"+ Environment.NewLine + "c"+ Environment.NewLine + "b"+ Environment.NewLine + "1"+ Environment.NewLine + "a"+ Environment.NewLine + "0"+ Environment.NewLine + "empty ok"+ Environment.NewLine + "x"+ Environment.NewLine + "1"+ Environment.NewLine + "x"+ Environment.NewLine + "99"+ Environment.NewLine + "world"+ Environment.NewLine + "hello",
            SetupTests.RunWithInput(s));
    }
}
