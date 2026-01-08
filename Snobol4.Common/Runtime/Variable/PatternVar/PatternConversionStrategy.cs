namespace Snobol4.Common;

/// <summary>
/// Conversion strategy for pattern variables
/// </summary>
public class PatternConversionStrategy : IConversionStrategy
{
    public bool TryConvert(Var self, Executive.VarType targetType, out Var varOut, out object valueOut, Executive exec)
    {
        var patternSelf = (PatternVar)self;
        varOut = StringVar.Null();
        valueOut = "";

        switch (targetType)
        {
            case Executive.VarType.PATTERN:
                varOut = patternSelf;
                valueOut = patternSelf.Data;
                return true;

            case Executive.VarType.STRING:
            case Executive.VarType.INTEGER:
            case Executive.VarType.REAL:
            case Executive.VarType.ARRAY:
            case Executive.VarType.TABLE:
            case Executive.VarType.NAME:
            case Executive.VarType.EXPRESSION:
            case Executive.VarType.CODE:
            default:
                return false;
        }
    }

    public string GetDataType(Var self)
    {
        return "pattern";
    }

    public object GetTableKey(Var self)
    {
        // Patterns use their unique ID as table key
        return self.UniqueId;
    }
}