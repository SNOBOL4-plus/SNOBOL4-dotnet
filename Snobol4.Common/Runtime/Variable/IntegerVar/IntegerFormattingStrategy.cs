using System.Globalization;

namespace Snobol4.Common;

/// <summary>
/// Formatting strategy for integer variables
/// </summary>
public class IntegerFormattingStrategy : IFormattingStrategy
{
    public string ToString(Var self)
    {
        var intSelf = (IntegerVar)self;
        return intSelf.Data.ToString(CultureInfo.CurrentCulture);
    }

    public string DumpString(Var self)
    {
        var intSelf = (IntegerVar)self;
        return $"{intSelf.Data}";
    }

    public string DebugString(Var self)
    {
        var intSelf = (IntegerVar)self;
        var symbol = intSelf.Symbol == "" ? "<no name>" : intSelf.Symbol;
        return $"INTEGER Symbol: {symbol}  Data: {intSelf.Data}  Succeeded: {intSelf.Succeeded}";
    }
}