namespace Snobol4.Common;

/// <summary>
/// Formatting strategy for code variables
/// </summary>
public class CodeFormattingStrategy : IFormattingStrategy
{
    public string ToString(Var self)
    {
        return "code";
    }

    public string DumpString(Var self)
    {
        var codeSelf = (CodeVar)self;
        return $"<code@{codeSelf.StatementNumber}>";
    }

    public string DebugString(Var self)
    {
        var codeSelf = (CodeVar)self;
        var symbol = codeSelf.Symbol == "" ? "<no name>" : codeSelf.Symbol;
        var preview = codeSelf.Data.Length > 40
            ? codeSelf.Data[..40] + "..."
            : codeSelf.Data;
        return $"CODE Symbol: {symbol}  StatementNumber: {codeSelf.StatementNumber}  Source: '{preview}'  Succeeded: {codeSelf.Succeeded}";
    }
}