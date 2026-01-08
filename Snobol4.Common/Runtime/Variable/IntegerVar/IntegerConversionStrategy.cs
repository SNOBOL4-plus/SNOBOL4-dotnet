using System.Globalization;

namespace Snobol4.Common;

/// <summary>
/// Conversion strategy for integer variables
/// </summary>
public class IntegerConversionStrategy : IConversionStrategy
{
    public bool TryConvert(Var self, Executive.VarType targetType, out Var varOut, out object valueOut, Executive exec)
    {
        var intSelf = (IntegerVar)self;
        varOut = new IntegerVar(0);
        valueOut = "";

        switch (targetType)
        {
            case Executive.VarType.STRING:
                varOut = new StringVar(intSelf.Data.ToString(CultureInfo.CurrentCulture));
                valueOut = ((StringVar)varOut).Data;
                return true;

            case Executive.VarType.INTEGER:
                varOut = intSelf;
                valueOut = intSelf.Data;
                return true;

            case Executive.VarType.REAL:
                varOut = new RealVar(intSelf.Data);
                valueOut = ((RealVar)varOut).Data;
                return true;

            case Executive.VarType.PATTERN:
                varOut = new PatternVar(new LiteralPattern(intSelf.Data.ToString(CultureInfo.CurrentCulture)));
                valueOut = ((PatternVar)varOut).Data;
                return true;

            case Executive.VarType.NAME:
                var stringData = intSelf.Data.ToString();
                if (stringData == "")
                    return false;
                varOut = new NameVar(stringData, intSelf.Key, intSelf.Collection);
                valueOut = intSelf.Data;
                return true;

            case Executive.VarType.EXPRESSION:
                var previousCaseFolding = exec.Parent.CaseFolding;
                exec.Parent.CaseFolding = ((IntegerVar)exec.IdentifierTable["&case"]).Data != 0;
                exec.Parent.CodeMode = true;
                exec.Parent.Code = new SourceCode(exec.Parent);
                exec.Parent.Code.ReadCodeInString($" A = *({intSelf.Data.ToString().Trim()})", exec.Parent.FilesToCompile[^1]);
                exec.Parent.BuildEval();
                exec.Parent.CaseFolding = previousCaseFolding;
                exec.Parent.CodeMode = false;
                varOut = new ExpressionVar(exec.StarFunctionList[^1]);
                valueOut = ((ExpressionVar)varOut).FunctionName;
                return true;

            case Executive.VarType.ARRAY:
            case Executive.VarType.TABLE:
            case Executive.VarType.CODE:
            default:
                return false;
        }
    }

    public string GetDataType(Var self)
    {
        return "integer";
    }

    public object GetTableKey(Var self)
    {
        var intSelf = (IntegerVar)self;
        return intSelf.Data;
    }
}