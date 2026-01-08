using System.Globalization;

namespace Snobol4.Common;

/// <summary>
/// Conversion strategy for real (floating-point) variables
/// </summary>
public class RealConversionStrategy : IConversionStrategy
{
    public bool TryConvert(Var self, Executive.VarType targetType, out Var varOut, out object valueOut, Executive exec)
    {
        var realSelf = (RealVar)self;
        varOut = StringVar.Null();
        valueOut = "";

        switch (targetType)
        {
            case Executive.VarType.STRING:
                valueOut = realSelf.Data.ToString(CultureInfo.CurrentCulture);
                varOut = new StringVar((string)valueOut);
                return true;

            case Executive.VarType.INTEGER:
                // Round towards zero (truncate)
                var rounded = Math.Round(realSelf.Data, MidpointRounding.ToZero);

                try
                {
                    checked
                    {
                        var intValue = (long)rounded;
                        varOut = new IntegerVar(intValue);
                        valueOut = intValue;
                        return true;
                    }
                }
                catch (OverflowException)
                {
                    return false;
                }

            case Executive.VarType.REAL:
                varOut = realSelf;
                valueOut = realSelf.Data;
                return true;

            case Executive.VarType.PATTERN:
                valueOut = realSelf.Data.ToString(CultureInfo.CurrentCulture);
                varOut = new PatternVar(new LiteralPattern((string)valueOut));
                return true;

            case Executive.VarType.NAME:
                var stringData = realSelf.Data.ToString(CultureInfo.InvariantCulture);
                if (stringData == "")
                    return false;
                varOut = new NameVar(stringData, realSelf.Key, realSelf.Collection);
                valueOut = realSelf.Data;
                return true;

            case Executive.VarType.EXPRESSION:
                var previousCaseFolding = exec.Parent.CaseFolding;
                exec.Parent.CaseFolding = ((IntegerVar)exec.IdentifierTable["&case"]).Data != 0;
                exec.Parent.CodeMode = true;
                exec.Parent.Code = new SourceCode(exec.Parent);
                exec.Parent.Code.ReadCodeInString($" A = *({realSelf.Data.ToString(CultureInfo.InvariantCulture).Trim()})", exec.Parent.FilesToCompile[^1]);
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
        return "real";
    }

    public object GetTableKey(Var self)
    {
        var realSelf = (RealVar)self;
        return realSelf.Data;
    }
}