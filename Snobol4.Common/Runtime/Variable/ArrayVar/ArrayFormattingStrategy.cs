namespace Snobol4.Common;

/// <summary>
/// Formatting strategy for array variables.
/// Provides string representations for different contexts.
/// </summary>
public class ArrayFormattingStrategy : IFormattingStrategy
{
    private const string _arrayTypeName = "array";
    private const string _anonymousSymbol = "<anonymous>";

    public string ToString(Var self)
    {
        // Simple representation for general use
        return _arrayTypeName;
    }

    public string DumpString(Var self)
    {
        // Detailed representation showing prototype
        var arraySelf = (ArrayVar)self;
        return $"{_arrayTypeName}({arraySelf.Prototype})";
    }

    public string DebugString(Var self)
    {
        // Comprehensive debug information
        var arraySelf = (ArrayVar)self;
        var symbol = string.IsNullOrEmpty(arraySelf.Symbol) ? _anonymousSymbol : arraySelf.Symbol;
        
        return $"ARRAY Symbol: {symbol}  Prototype: {arraySelf.Prototype}  " +
               $"Dimensions: {arraySelf.Dimensions}  TotalSize: {arraySelf.TotalSize}  " +
               $"Fill: {arraySelf.Fill.DumpString()}  " +
               $"Elements: {arraySelf.Data.Count}  Succeeded: {arraySelf.Succeeded}";
    }
}