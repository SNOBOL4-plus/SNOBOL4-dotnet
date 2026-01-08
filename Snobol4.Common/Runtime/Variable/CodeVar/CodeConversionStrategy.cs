namespace Snobol4.Common;

/// <summary>
/// Conversion strategy for code variables
/// </summary>
public class CodeConversionStrategy : IConversionStrategy
{
    public bool TryConvert(Var self, Executive.VarType targetType, out Var varOut, out object valueOut, Executive exec)
    {
        var codeSelf = (CodeVar)self;
        varOut = StringVar.Null();
        valueOut = "";

        switch (targetType)
        {
            case Executive.VarType.CODE:
                varOut = codeSelf;
                valueOut = codeSelf.Data;
                //valueOut = ((CodeVar)varOut).Data;
                //valueOut = codeSelf.StatementNumber;
                return true;

            case Executive.VarType.STRING:
            case Executive.VarType.INTEGER:
            case Executive.VarType.REAL:
            case Executive.VarType.ARRAY:
            case Executive.VarType.TABLE:
            case Executive.VarType.PATTERN:
            case Executive.VarType.NAME:
            case Executive.VarType.EXPRESSION:
            default:
                return false;
        }
    }

    public string GetDataType(Var self)
    {
        return "code";
    }

    public object GetTableKey(Var self)
    {
        // Code uses its unique ID as table key
        return self.UniqueId;
    }
}