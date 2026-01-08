namespace Snobol4.Common;

/// <summary>
/// Conversion strategy for table variables
/// </summary>
public class TableConversionStrategy : IConversionStrategy
{
    public bool TryConvert(Var self, Executive.VarType targetType, out Var varOut, out object valueOut, Executive exec)
    {
        var tableSelf = (TableVar)self;
        varOut = StringVar.Null();
        valueOut = "";

        switch (targetType)
        {
            case Executive.VarType.ARRAY:
                // Convert table to 2D array with keys and values
                if (tableSelf.Data.Count == 0)
                    return false;

                ArrayVar convertedArray = new();
                convertedArray.ConfigurePrototype($"{tableSelf.Data.Count},2", tableSelf.Fill);

                var i = 0;
                var j = 0;
                foreach (var item in tableSelf.Data)
                {
                    // Convert key to appropriate Var type
                    convertedArray.Data[i++] = item.Key switch
                    {
                        long l => new IntegerVar(l),
                        double d => new RealVar(d),
                        string s => new StringVar(s),
                        _ => tableSelf.Collection == null || tableSelf.Key == null
                            ? tableSelf
                            : ((TableVar)tableSelf.Collection).Data[tableSelf.Key]
                    };

                    // Add value
                    convertedArray.Data[i++] = tableSelf.Data.Values.ElementAt(j++);
                }

                varOut = convertedArray;
                valueOut = tableSelf.Data;
                return true;

            case Executive.VarType.TABLE:
                varOut = tableSelf;
                valueOut = tableSelf.Data;
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
        return "table";
    }

    public object GetTableKey(Var self)
    {
        // Tables use their unique ID as table key
        return self.UniqueId;
    }
}