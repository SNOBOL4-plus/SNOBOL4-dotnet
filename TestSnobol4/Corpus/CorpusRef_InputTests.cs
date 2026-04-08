using Test.TestLexer;

// ReSharper disable StringLiteralTypo

namespace Test.Corpus;

/// <summary>
/// Corpus tests requiring stdin INPUT — uses RunWithInput() with embedded input.
/// Covers: arith/fileinfo, arith/triplet, control/expr_eval,
///         strings/word1, word2, word3, word4, wordcount, strings/cross.
///
/// Note: strings/cross tests @N cursor capture (M-NET-P35-FIX bug).
/// </summary>
[DoNotParallelize]
[TestClass]
public class CorpusRef_InputTests
{
    [TestMethod]
    public void TEST_Corpus_arith_fileinfo()
    {
        var s = @"
         &TRIM    =  1
NEXTL    CHARS    =  CHARS + SIZE(INPUT)                    :F(DONE)
         LINES    =  LINES + 1                              :(NEXTL)
DONE     OUTPUT   =  CHARS ' characters, ' LINES ' lines read'
END";
        var input = "hello world\nfoo bar baz";
        Assert.AreEqual("22 characters, 2 lines read", SetupTests.RunWithInput(s, input));
    }

    [TestMethod]
    public void TEST_Corpus_arith_triplet()
    {
        var s = @"
         &TRIM    =  1
         N        =  0
LOOP     S        =  INPUT                                  :F(END)
         OUTPUT   =  DUPL(' ', (80 - SIZE(S)) / 2) S
         N        =  REMDR(N + 1, 3)
         OUTPUT   =  EQ(N, 0)                               :(LOOP)
END";
        var input = "alpha\nbeta\ngamma\ndelta\nepsilon\nepsilon";
        var expected =
            "                                     alpha\n" +
            "                                      beta\n" +
            "                                     gamma\n" +
            "\n" +
            "                                     delta\n" +
            "                                    epsilon\n" +
            "                                    epsilon";
        Assert.AreEqual(expected, SetupTests.RunWithInput(s, input));
    }

    [TestMethod, Ignore("M-NET-EVAL-COMPLETE: recursive expr evaluator uses NRETURN + *func() side-effects + EVAL — fix pending")]
    public void TEST_Corpus_control_expr_eval()
    {
        var s = @"
         DEFINE('Push(x)')
         stk      =  TABLE()                       :(PushEnd)
Push     stk[0]   =  stk[0] + 1
         Push     =  .stk[stk[0]]
         $Push    =  x                             :(NRETURN)
PushEnd
         DEFINE('Pop()')                           :(PopEnd)
Pop      Pop      =  stk[stk[0]]
         stk[0]   =  stk[0] - 1                    :(RETURN)
PopEnd
         DEFINE('Unary()arg,op')                   :(UnaryEnd)
Unary    arg      =  Pop()
         op       =  Pop()
         Push()   =  EVAL(op arg)
         Unary    =  .dummy                        :(NRETURN)
UnaryEnd
         DEFINE('Binary()op,left,right')           :(BinaryEnd)
Binary   right    =  Pop()
         op       =  Pop()
         left     =  Pop()
         Push()   =  EVAL(left ' ' op ' ' right)
         Binary   =  .dummy                        :(NRETURN)
BinaryEnd

         integer  =  SPAN('0123456789')
         exponent =  ANY('eEdD') (ANY('+-') | epsilon) integer
         real     =  integer '.' (integer | epsilon) (exponent | epsilon)
+                 |  integer exponent

         addop    =  ANY('+-') . *Push()
         mulop    =  ANY('*/') . *Push()
         constant =  (real | integer) . *Push()

         primary  =  constant | '(' *expr ')'

         factor   =  addop *factor . *Unary()
+                 |  *primary

         term     =  *factor mulop *term . *Binary()
+                 |  *factor

         expr     =  *term addop *expr . *Binary()
+                 |  *term

         &TRIM    =  1
loop     line     =  INPUT                         :F(END)
         line     POS(0) expr RPOS(0)              :F(error)
         OUTPUT   =  Pop()                         :(loop)
error    OUTPUT   = 'Bad input, try again'         :(loop)
END";
        var input = "1+2*3\n(1+2)*3\n2.5e1+0.5\n-3+10\n4*5+6";
        Assert.AreEqual("7\n9\n25.5\n7\n26", SetupTests.RunWithInput(s, input));
    }

    [TestMethod]
    public void TEST_Corpus_strings_word1()
    {
        var s = @"
      PAT      =  "" the "" ARB . OUTPUT ("" of "" | "" a "")
LOOP  LINE     =  INPUT                            :F(END)
      LINE     ?  PAT                              :(LOOP)
END";
        var input = "She saw the cat of the hat sitting near the dog a bone.\nNothing interesting on this line.\nI know the house of cards and the tower a bridge.";
        Assert.AreEqual("cat\nhouse", SetupTests.RunWithInput(s, input));
    }

    [TestMethod]
    public void TEST_Corpus_strings_word2()
    {
        var s = @"
      PAT      =  POS(0) LEN(4) . WHEN
+                 TAB(6) ARB . WHO "" :""
+                 TAB(24) REM . WHAT
LOOP  LINE     =  INPUT                            :F(END)
      LINE     ?  PAT                              :F(LOOP)
      OUTPUT   =  WHO "" invented the "" WHAT "" in "" WHEN  :(LOOP)
END";
        var input = "1769  Watt             : Steam Engine\n1876  Bell             : Telephone\n1903  Wright           : Airplane\n1928  Fleming          : Penicillin";
        var expected =
            "Watt             invented the  Steam Engine in 1769\n" +
            "Bell             invented the  Telephone in 1876\n" +
            "Wright           invented the  Airplane in 1903\n" +
            "Fleming          invented the  Penicillin in 1928";
        Assert.AreEqual(expected, SetupTests.RunWithInput(s, input));
    }

    [TestMethod]
    public void TEST_Corpus_strings_word3()
    {
        var s = @"
      PAT      =  POS(0) BREAK(' ') . WHEN (' ' SPAN(' '))
+                 ARB . WHO (' ' SPAN(' :'))
+                 REM . WHAT
LOOP  LINE     =  INPUT                            :F(END)
      LINE     ?  PAT                              :F(LOOP)
      OUTPUT   =  WHO "" invented the "" WHAT "" in "" WHEN  :(LOOP)
END";
        var input = "1769  Watt : Steam Engine\n1876  Bell : Telephone\n1903  Wright : Airplane\n1928  Fleming : Penicillin";
        var expected =
            "Watt invented the Steam Engine in 1769\n" +
            "Bell invented the Telephone in 1876\n" +
            "Wright invented the Airplane in 1903\n" +
            "Fleming invented the Penicillin in 1928";
        Assert.AreEqual(expected, SetupTests.RunWithInput(s, input));
    }

    [TestMethod]
    public void TEST_Corpus_strings_word4()
    {
        var s = @"
      PAT      =  POS(0) BREAK(' ') . WHEN (' ' SPAN(' '))
+                 BREAKX(' ') . WHO (' ' SPAN(' :'))
+                 REM . WHAT
LOOP  LINE     =  INPUT                            :F(END)
      LINE     ?  PAT                              :F(LOOP)
      OUTPUT   =  WHO "" invented the "" WHAT "" in "" WHEN  :(LOOP)
END";
        var input = "1769  Watt : Steam Engine\n1876  Bell : Telephone\n1903  Wright : Airplane\n1928  Fleming : Penicillin";
        var expected =
            "Watt invented the Steam Engine in 1769\n" +
            "Bell invented the Telephone in 1876\n" +
            "Wright invented the Airplane in 1903\n" +
            "Fleming invented the Penicillin in 1928";
        Assert.AreEqual(expected, SetupTests.RunWithInput(s, input));
    }

    [TestMethod]
    public void TEST_Corpus_strings_wordcount()
    {
        var s = @"
      &TRIM    =  1
      NUMERALS =  '0123456789'
      WORD     =  ""'-"" NUMERALS &UCASE &LCASE
      WPAT     =  BREAK(WORD) SPAN(WORD)
NEXTL LINE     =  INPUT                            :F(DONE)
NEXTW LINE     ?  WPAT =                           :F(NEXTL)
      N        =  N + 1                            :(NEXTW)
DONE  OUTPUT   =  +N ' words'
END";
        var input = "it's a well-known fact that the quick brown fox\njumped over the lazy dog";
        Assert.AreEqual("14 words", SetupTests.RunWithInput(s, input));
    }

    /// <summary>
    /// cross.sno — @N cursor position capture.
    /// This test exercises the M-NET-P35-FIX bug: @NH and @NV capture cursor
    /// positions during pattern match. Currently fails due to VarSlotArray not
    /// being updated for variables first created by @N at runtime.
    /// Tracked: M-NET-P35-FIX.
    /// </summary>
    [TestMethod]
    public void TEST_Corpus_strings_cross()
    {
        var s = @"
      &TRIM    =  1
AGAIN H        =  INPUT                            :F(END)
      V        =  INPUT                            :F(END)
      HC       =  H
NEXTH HC       ?  @NH ANY(V) . CROSS = '*'         :F(AGAIN)
      VC       =  V
NEXTV VC       ?  @NV CROSS = '#'                  :F(NEXTH)
      OUTPUT   =
      PRINTV   =  V
      PRINTV   ?  POS(NV) LEN(1) = '#'
PRINT PRINTV   ?  LEN(1) . C =                     :F(NEXTV)
      OUTPUT   =  DIFFER(C, '#') DUPL(' ', NH) C   :S(PRINT)
      OUTPUT   =  H                                :(PRINT)
END";
        var input = "SNOBOL\nOBJECT";
        var expected =
            "\nSNOBOL\n  B\n  J\n  E\n  C\n  T\n\n   O\nSNOBOL\n   J\n   E\n   C\n   T\n\nSNOBOL\n    B\n    J\n    E\n    C\n    T";
        Assert.AreEqual(expected, SetupTests.RunWithInput(s, input));
    }
}
