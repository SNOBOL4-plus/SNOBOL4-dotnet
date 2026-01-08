using Snobol4.Common;
using Test.TestLexer;

namespace Test.TestParser;

public partial class TestParser
{
    public string[] Right =
    [
        " a = b = c = d",
       " a | b | c | d",
       " a b c d",
       " a ^ b ^ c ^ d",
       " a @ b @ c @ d",
       " a ~ b ~ c ~ d"
    ];

    public string[] Left =
    [
        " a ? b ? c ? d",
       " a + b + c + d",
       " a - b - c - d",
       " a / b / c / d",
       " a * b * c * d",
       " a $ b $ c $ d",
       " a . b . c . d",
       " a & b & c & d",
       " a # b # c # d",
       " a % b % c % d"
    ];

    [TestMethod]
    public void TEST_Associativity_001()
    {
        foreach (var s in Right)
        {
            var directives = "-b -n";
            var build = SetupTests.SetupScript(directives, s + ";end");
            Assert.AreEqual(Token.Type.IDENTIFIER, build.Code.SourceLines[0].ParseBody[0].TokenType);
            Assert.AreEqual(Token.Type.IDENTIFIER, build.Code.SourceLines[0].ParseBody[1].TokenType);
            Assert.AreEqual(Token.Type.IDENTIFIER, build.Code.SourceLines[0].ParseBody[2].TokenType);
            Assert.AreEqual(Token.Type.IDENTIFIER, build.Code.SourceLines[0].ParseBody[3].TokenType);
        }
    }

    [TestMethod]
    public void TEST_Associativity_002()
    {
        foreach (var s in Left)
        {
            var directives = "-b -n";
            var build = SetupTests.SetupScript(directives, s + ";end");
            Assert.AreEqual(Token.Type.IDENTIFIER, build.Code.SourceLines[0].ParseBody[0].TokenType);
            Assert.AreEqual(Token.Type.IDENTIFIER, build.Code.SourceLines[0].ParseBody[1].TokenType);
            Assert.AreEqual(Token.Type.IDENTIFIER, build.Code.SourceLines[0].ParseBody[3].TokenType);
            Assert.AreEqual(Token.Type.IDENTIFIER, build.Code.SourceLines[0].ParseBody[5].TokenType);
        }
    }
}