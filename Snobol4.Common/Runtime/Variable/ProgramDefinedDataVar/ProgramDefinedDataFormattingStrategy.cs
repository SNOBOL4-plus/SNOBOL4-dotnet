namespace Snobol4.Common;

/// <summary>
/// Formatting strategy for program-defined data variables
/// </summary>
public class ProgramDefinedDataFormattingStrategy : IFormattingStrategy
{
    public string ToString(Var self)
    {
        var dataSelf = (ProgramDefinedDataVar)self;
        return dataSelf.UserDefinedDataName;
    }

    public string DumpString(Var self)
    {
        var dataSelf = (ProgramDefinedDataVar)self;

        // Show type and field count
        return $"<{dataSelf.UserDefinedDataName}:{dataSelf.ProgramDefinedData.Count}>";
    }

    public string DebugString(Var self)
    {
        var dataSelf = (ProgramDefinedDataVar)self;
        var symbol = dataSelf.Symbol == "" ? "<no name>" : dataSelf.Symbol;

        // Show field names
        var fields = string.Join(", ", dataSelf.ProgramDefinedData.Keys);

        return $"DATA Symbol: {symbol}  Type: {dataSelf.UserDefinedDataName}  Fields: [{fields}]  Succeeded: {dataSelf.Succeeded}";
    }
}