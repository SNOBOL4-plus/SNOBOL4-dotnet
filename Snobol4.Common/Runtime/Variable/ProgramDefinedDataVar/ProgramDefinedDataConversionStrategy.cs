namespace Snobol4.Common;

/// <summary>
/// Conversion strategy for program-defined data variables
/// </summary>
public class ProgramDefinedDataConversionStrategy : IConversionStrategy
{
    public bool TryConvert(Var self, Executive.VarType targetType, out Var varOut, out object valueOut, Executive exec)
    {
        var dataSelf = (ProgramDefinedDataVar)self;
        varOut = StringVar.Null();
        valueOut = "";

        switch (targetType)
        {
            case Executive.VarType.STRING:
                // Convert to type name
                varOut = new StringVar(dataSelf.UserDefinedDataName);
                valueOut = dataSelf.UserDefinedDataName;
                return true;

            case Executive.VarType.INTEGER:
            case Executive.VarType.REAL:
            case Executive.VarType.ARRAY:
            case Executive.VarType.TABLE:
            case Executive.VarType.PATTERN:
            case Executive.VarType.NAME:
            case Executive.VarType.EXPRESSION:
            case Executive.VarType.CODE:
            default:
                return false;
        }
    }

    public string GetDataType(Var self)
    {
        var dataSelf = (ProgramDefinedDataVar)self;
        // Return the user-defined type name, not "data"
        return dataSelf.UserDefinedDataName;
    }

    public object GetTableKey(Var self)
    {
        // User-defined data uses its unique ID as table key
        return self.UniqueId;
    }
}