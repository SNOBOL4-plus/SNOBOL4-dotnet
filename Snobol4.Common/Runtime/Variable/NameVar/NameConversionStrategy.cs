namespace Snobol4.Common;

/// <summary>
/// Conversion strategy for name variables
/// Names can convert by dereferencing or converting the pointer string
/// </summary>
public class NameConversionStrategy : IConversionStrategy
{
    public bool TryConvert(Var self, Executive.VarType targetType, out Var varOut, out object valueOut, Executive exec)
    {
        var nameSelf = (NameVar)self;
        varOut = StringVar.Null();
        valueOut = "";

        switch (targetType)
        {
            case Executive.VarType.STRING:
                if (nameSelf.Pointer == "")
                    return false;

                varOut = new StringVar(nameSelf.Pointer)
                {
                    Collection = nameSelf.Collection,
                    Key = nameSelf.Key
                };
                valueOut = nameSelf.Pointer;
                return true;

            case Executive.VarType.INTEGER:
                var stringVarInteger = new StringVar(nameSelf.Pointer);
                if (!stringVarInteger.Convert(Executive.VarType.INTEGER, out var dataInteger, out valueOut, exec))
                    return false;
                varOut = dataInteger;
                return true;

            case Executive.VarType.REAL:
                var stringVarReal = new StringVar(nameSelf.Pointer);
                if (!stringVarReal.Convert(Executive.VarType.REAL, out var dataReal, out valueOut, exec))
                    return false;
                varOut = dataReal;
                return true;

            case Executive.VarType.PATTERN:
                var patternOut = new PatternVar(new LiteralPattern(nameSelf.Pointer));
                valueOut = patternOut.Data;
                return true;

            case Executive.VarType.NAME:
                varOut = nameSelf;
                valueOut = nameSelf.Pointer;
                return true;

            case Executive.VarType.EXPRESSION:
                var argExpression = exec.IdentifierTable[nameSelf.Pointer];

                if (argExpression is not StringVar stringVarExpression)
                    return false;

                if (stringVarExpression.Symbol == "")
                    return false;

                var previousCaseFolding = exec.Parent.CaseFolding;
                exec.Parent.CaseFolding = ((IntegerVar)exec.IdentifierTable["&case"]).Data != 0;
                exec.Parent.CodeMode = true;
                exec.Parent.Code = new SourceCode(exec.Parent);
                exec.Parent.Code.ReadCodeInString($" A = *({stringVarExpression.Data.Trim()})", exec.Parent.FilesToCompile[^1]);
                exec.Parent.BuildEval();
                exec.Parent.CaseFolding = previousCaseFolding;
                exec.Parent.CodeMode = false;
                varOut = new ExpressionVar(exec.StarFunctionList[^1]);
                valueOut = ((ExpressionVar)varOut).FunctionName;
                return true;

            case Executive.VarType.CODE:
                var argCode = exec.IdentifierTable[nameSelf.Pointer];

                if (argCode is not StringVar stringVarCode)
                    return false;

                if (stringVarCode.Symbol == "")
                    return false;

                CodeVar code = new()
                {
                    StatementNumber = exec.Statements.Count,
                    Data = stringVarCode.Symbol
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
        return "name";
    }

    public object GetTableKey(Var self)
    {
        var nameSelf = (NameVar)self;

        // Use the pointer string as the key
        return nameSelf.Pointer;
    }
}