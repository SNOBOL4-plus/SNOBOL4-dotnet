using Snobol4.Common;
using Test.TestLexer;

namespace Test.TestParser;

[TestClass]
public partial class TestParser
{
    public string[] Cases1 =
    [
        " r = a ? b = c",
        " r = a & b ? c",
        " r = a | b & c",
        " r = a   b | c",
        " r = a @ b   c",
        " r = a + b @ c",
        " r = a - b + c",
        " r = a # b - c",
        " r = a / b # c",
        " r = a * b / c",
        " r = a % b * c",
        " r = a ^ b % c",
        " r = a $ b ^ c",
        " r = a . b $ c",
        " r = a ~ b . c"
    ];

    public string[] Cases2 =
    [
        " r = a = b ? c",
        " r = a ? b & c",
        " r = a & b | c",
        " r = a | b   c",
        " r = a   b @ c",
        " r = a @ b + c",
        " r = a - b # c",
        " r = a # b / c",
        " r = a / b * c",
        " r = a * b % c",
        " r = a % b ^ c",
        " r = a ^ b $ c",
        " r = a . b ~ c"
    ];

    [TestMethod]
    public void TEST_Precedence_001()
    {
        foreach (var s in Cases1)
        {
            var directives = "-b -n";
            var build = SetupTests.SetupScript(directives, s + ";end");
            Assert.AreEqual(Token.Type.IDENTIFIER, build.Code.SourceLines[0].ParseBody[1].TokenType);
            Assert.AreEqual(Token.Type.IDENTIFIER, build.Code.SourceLines[0].ParseBody[2].TokenType);
            Assert.AreEqual(Token.Type.IDENTIFIER, build.Code.SourceLines[0].ParseBody[4].TokenType);
        }
    }

    [TestMethod]
    public void TEST_Precedence_002()
    {
        foreach (var s in Cases2)
        {
            var directives = "-b -n";
            var build = SetupTests.SetupScript(directives, s + ";end");
            Assert.AreEqual(Token.Type.IDENTIFIER, build.Code.SourceLines[0].ParseBody[1].TokenType);
            Assert.AreEqual(Token.Type.IDENTIFIER, build.Code.SourceLines[0].ParseBody[2].TokenType);
            Assert.AreEqual(Token.Type.IDENTIFIER, build.Code.SourceLines[0].ParseBody[3].TokenType);
        }
    }



    [TestMethod]
    public void TEST_Precedence_003()
    {
        var directives = "-b -n";
        var build = SetupTests.SetupScript(directives, " r = a ? (-b(c,d) + e) * f;end");
        Assert.AreEqual(Token.Type.IDENTIFIER, build.Code.SourceLines[0].ParseBody[0].TokenType);
        Assert.AreEqual(Token.Type.IDENTIFIER, build.Code.SourceLines[0].ParseBody[1].TokenType);
        Assert.AreEqual(Token.Type.IDENTIFIER_FUNCTION, build.Code.SourceLines[0].ParseBody[2].TokenType);
        Assert.AreEqual(Token.Type.IDENTIFIER, build.Code.SourceLines[0].ParseBody[3].TokenType);
        Assert.AreEqual(Token.Type.IDENTIFIER, build.Code.SourceLines[0].ParseBody[4].TokenType);
        Assert.AreEqual(Token.Type.R_PAREN_FUNCTION, build.Code.SourceLines[0].ParseBody[5].TokenType);
        Assert.AreEqual(Token.Type.UNARY_OPERATOR, build.Code.SourceLines[0].ParseBody[6].TokenType);
        Assert.AreEqual(Token.Type.IDENTIFIER, build.Code.SourceLines[0].ParseBody[7].TokenType);
        Assert.AreEqual(Token.Type.BINARY_PLUS, build.Code.SourceLines[0].ParseBody[8].TokenType);
        Assert.AreEqual(Token.Type.IDENTIFIER, build.Code.SourceLines[0].ParseBody[9].TokenType);
        Assert.AreEqual(Token.Type.BINARY_STAR, build.Code.SourceLines[0].ParseBody[10].TokenType);
        Assert.AreEqual(Token.Type.BINARY_QUESTION, build.Code.SourceLines[0].ParseBody[11].TokenType);
        Assert.AreEqual(Token.Type.BINARY_EQUAL, build.Code.SourceLines[0].ParseBody[12].TokenType);
    }
}