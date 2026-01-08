namespace Snobol4.Common;

/// <summary>
/// Conversion strategy for string variables
/// </summary>
public class StringConversionStrategy : IConversionStrategy
{
    public bool TryConvert(Var self, Executive.VarType targetType, out Var varOut, out object valueOut, Executive exec)
    {
        var stringSelf = (StringVar)self;
        varOut = StringVar.Null();
        valueOut = "";

        switch (targetType)
        {
            case Executive.VarType.STRING:
                varOut = stringSelf;
                valueOut = stringSelf.Data;
                return true;

            case Executive.VarType.INTEGER:
                if (!Var.ToInteger(stringSelf.Data, out var intValue))
                    return false;
                varOut = new IntegerVar(intValue);
                valueOut = intValue;
                return true;

            case Executive.VarType.REAL:
                if (!Var.ToReal(stringSelf.Data, out var realValue))
                    return false;
                varOut = new RealVar(realValue);
                valueOut = realValue;
                return true;

            case Executive.VarType.PATTERN:
                varOut = new PatternVar(new LiteralPattern(stringSelf.Data));
                valueOut = ((PatternVar)varOut).Data;
                return true;

            case Executive.VarType.NAME:
                if (stringSelf.Data == "")
                    return false;
                varOut = new NameVar(stringSelf.Data, stringSelf.Key, stringSelf.Collection);
                valueOut = stringSelf.Data;
                return true;

            case Executive.VarType.EXPRESSION:
                if (stringSelf.Data == "")
                    return false;

                var previousCaseFolding = exec.Parent.CaseFolding;
                exec.Parent.CaseFolding = ((IntegerVar)exec.IdentifierTable["&case"]).Data != 0;
                exec.Parent.CodeMode = true;
                exec.Parent.Code = new SourceCode(exec.Parent);
                exec.Parent.Code.ReadCodeInString($" A = *({stringSelf.Data.Trim()})", exec.Parent.FilesToCompile[^1]);
                exec.Parent.BuildEval();
                exec.Parent.CaseFolding = previousCaseFolding;
                exec.Parent.CodeMode = false;
                varOut = new ExpressionVar(exec.StarFunctionList[^1]);
                valueOut = ((ExpressionVar)varOut).FunctionName;
                return true;

            case Executive.VarType.CODE:
                CodeVar code = new()
                {
                    StatementNumber = exec.Statements.Count,
                    Data = stringSelf.Data
                };

                exec.Parent.Code = new SourceCode(exec.Parent);
                exec.Parent.Code.ReadCodeInString(code.Data, exec.Parent.FilesToCompile[^1]);

                if (!exec.Parent.BuildCode())
                    return false;

                varOut = code;
                valueOut = code.Data;
                return true;

            case Executive.VarType.ARRAY:
            case Executive.VarType.TABLE:
            default:
                return false;
        }
    }

    public string GetDataType(Var self)
    {
        return "string";
    }

    public object GetTableKey(Var self)
    {
        var stringSelf = (StringVar)self;
        return stringSelf.Data;
    }
}