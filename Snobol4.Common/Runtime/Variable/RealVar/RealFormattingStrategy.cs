using System.Globalization;

namespace Snobol4.Common;

/// <summary>
/// Formatting strategy for real (floating-point) variables
/// </summary>
public class RealFormattingStrategy : IFormattingStrategy
{
    public string ToString(Var self)
    {
        var realSelf = (RealVar)self;
        return realSelf.Data.ToString(CultureInfo.CurrentCulture);
    }

    public string DumpString(Var self)
    {
        var realSelf = (RealVar)self;
        return $"{realSelf.Data}";
    }

    public string DebugString(Var self)
    {
        var realSelf = (RealVar)self;
        var symbol = realSelf.Symbol == "" ? "<no name>" : realSelf.Symbol;
        return $"REAL Symbol: {symbol}  Data: {realSelf.Data}  Succeeded: {realSelf.Succeeded}";
    }
}