namespace Snobol4.Common;

/// <summary>
/// Formatting strategy for table variables
/// </summary>
public class TableFormattingStrategy : IFormattingStrategy
{
    public string ToString(Var self)
    {
        return "table";
    }

    public string DumpString(Var self)
    {
        var tableSelf = (TableVar)self;
        return $"table({tableSelf.Data.Count})";
    }

    public string DebugString(Var self)
    {
        var tableSelf = (TableVar)self;
        var symbol = tableSelf.Symbol == "" ? "<no name>" : tableSelf.Symbol;
        return $"TABLE Symbol: {symbol}  Count: {tableSelf.Data.Count}  Succeeded: {tableSelf.Succeeded}";
    }
}