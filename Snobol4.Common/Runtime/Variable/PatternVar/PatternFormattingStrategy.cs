namespace Snobol4.Common;

/// <summary>
/// Formatting strategy for pattern variables
/// </summary>
public class PatternFormattingStrategy : IFormattingStrategy
{
    public string ToString(Var self)
    {
        return "pattern";
    }

    public string DumpString(Var self)
    {
        var patternSelf = (PatternVar)self;

        // Try to provide more detail about the pattern type
        var patternType = patternSelf.Data switch
        {
            LiteralPattern lit => $"'{lit.Literal}'",
            ConcatenatePattern => "concat",
            AlternatePattern => "alternate",
            ArbPattern => "arb",
            ArbNoPattern => "arbno",
            BalPattern => "bal",
            _ => "pattern"
        };

        return $"<{patternType}>";
    }

    public string DebugString(Var self)
    {
        var patternSelf = (PatternVar)self;
        var symbol = patternSelf.Symbol == "" ? "<no name>" : patternSelf.Symbol;
        var patternType = patternSelf.Data.GetType().Name;
        return $"PATTERN Symbol: {symbol}  Type: {patternType}  Succeeded: {patternSelf.Succeeded}";
    }
}