namespace Snobol4.Common;

/// <summary>
/// Conversion strategy for array variables
/// </summary>
public class ArrayConversionStrategy : IConversionStrategy
{
    public bool TryConvert(Var self, Executive.VarType targetType, out Var varOut, out object valueOut, Executive exec)
    {
        var arraySelf = (ArrayVar)self;
        varOut = StringVar.Null();
        valueOut = "";

        switch (targetType)
        {
            case Executive.VarType.ARRAY:
                varOut = arraySelf;
                valueOut = arraySelf;
                return true;

            case Executive.VarType.TABLE:
                // Array must have 2 dimensions
                if (arraySelf.Dimensions != 2)
                    return false;

                // Array must have two columns
                if (arraySelf.UpperBounds[0] - arraySelf.LowerBounds[0] != 1)
                    return false;

                TableVar convertedTable = new(arraySelf.Fill);

                for (var i = 0; i < arraySelf.Data.Count; i += 2)
                    convertedTable.Data[arraySelf.Data[i].GetTableKey()] = arraySelf.Data[i + 1];

                valueOut = convertedTable.Data;
                varOut = convertedTable;
                return true;

            case Executive.VarType.STRING:
            case Executive.VarType.INTEGER:
            case Executive.VarType.REAL:
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
        return "array";
    }

    public object GetTableKey(Var self)
    {
        // Arrays use their unique ID as table key
        return self.UniqueId;
    }
}