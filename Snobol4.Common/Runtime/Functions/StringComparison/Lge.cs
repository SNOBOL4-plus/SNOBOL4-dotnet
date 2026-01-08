namespace Snobol4.Common;

public partial class Executive
{
    //"lge first argument is not a string" /* 124 */,
    //"lge second argument is not a string" /* 125 */,

    internal void LexicalGreaterThanOrEqual(List<Var> arguments)
    {
        var v0 = arguments[0];
        var v1 = arguments[1];

        if (!v0.Convert(VarType.STRING, out _, out var left, this))
        {
            LogRuntimeException(124);
            return;
        }

        if (!v1.Convert(VarType.STRING, out _, out var right, this))
        {
            LogRuntimeException(125);
            return;
        }

        if (string.Compare((string)left, (string)right, StringComparison.CurrentCulture) < 0)
        {
            NonExceptionFailure();
            return;
        }

        PredicateSuccess();
    }
}