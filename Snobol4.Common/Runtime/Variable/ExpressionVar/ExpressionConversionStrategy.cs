namespace Snobol4.Common;

/// <summary>
/// Conversion strategy for expression variables
/// </summary>
public class ExpressionConversionStrategy : IConversionStrategy
{
    public bool TryConvert(Var self, Executive.VarType targetType, out Var varOut, out object valueOut, Executive exec)
    {
        var expressionSelf = (ExpressionVar)self;
        varOut = StringVar.Null();
        valueOut = "";

        switch (targetType)
        {
            case Executive.VarType.EXPRESSION:
                varOut = expressionSelf;
                valueOut = expressionSelf.FunctionName;
                return true;

            case Executive.VarType.STRING:
            case Executive.VarType.INTEGER:
            case Executive.VarType.REAL:
            case Executive.VarType.ARRAY:
            case Executive.VarType.TABLE:
            case Executive.VarType.PATTERN:
            case Executive.VarType.NAME:
            case Executive.VarType.CODE:
            default:
                return false;
        }
    }

    public string GetDataType(Var self)
    {
        return "expression";
    }

    public object GetTableKey(Var self)
    {
        // Expressions use their unique ID as table key
        return self.UniqueId;
    }
}