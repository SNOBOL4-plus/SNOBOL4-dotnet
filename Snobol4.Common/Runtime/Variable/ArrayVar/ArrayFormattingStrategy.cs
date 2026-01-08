namespace Snobol4.Common;

/// <summary>
/// Formatting strategy for array variables
/// </summary>
public class ArrayFormattingStrategy : IFormattingStrategy
{
    public string ToString(Var self)
    {
        return "array";
    }

    public string DumpString(Var self)
    {
        var arraySelf = (ArrayVar)self;
        return $"array({arraySelf.Prototype})";
    }

    public string DebugString(Var self)
    {
        var arraySelf = (ArrayVar)self;
        var symbol = arraySelf.Symbol == "" ? "<no name>" : arraySelf.Symbol;
        return $"ARRAY Symbol: {symbol}  Prototype: {arraySelf.Prototype}  Dimensions: {arraySelf.Dimensions}  Size: {arraySelf.Data.Count}  Succeeded: {arraySelf.Succeeded}";
    }
}