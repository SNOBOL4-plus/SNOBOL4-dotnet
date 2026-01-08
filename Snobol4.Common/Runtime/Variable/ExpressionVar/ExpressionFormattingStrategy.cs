namespace Snobol4.Common;

/// <summary>
/// Formatting strategy for expression variables
/// </summary>
public class ExpressionFormattingStrategy : IFormattingStrategy
{
    public string ToString(Var self)
    {
        return "expression";
    }

    public string DumpString(Var self)
    {
        return "<expression>";
    }

    public string DebugString(Var self)
    {
        var expressionSelf = (ExpressionVar)self;
        var symbol = expressionSelf.Symbol == "" ? "<no name>" : expressionSelf.Symbol;
        var delegateName = expressionSelf.FunctionName.Method.Name;
        return $"EXPRESSION Symbol: {symbol}  Delegate: {delegateName}  Succeeded: {expressionSelf.Succeeded}";
    }
}