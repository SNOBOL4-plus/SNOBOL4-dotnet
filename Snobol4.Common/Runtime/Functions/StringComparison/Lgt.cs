namespace Snobol4.Common;

//"lgt first argument is not a string" /* 126 */,
//"lgt second argument is not a string" /* 127 */,

public partial class Executive
{
    internal void LexicalGreaterThan(List<Var> arguments)
    {
        var v0 = arguments[0];
        var v1 = arguments[1];

        if (!v0.Convert(VarType.STRING, out _, out var left, this))
        {
            LogRuntimeException(126);
            return;
        }

        if (!v1.Convert(VarType.STRING, out _, out var right, this))
        {
            LogRuntimeException(127);
            return;
        }

        if (string.Compare((string)left, (string)right, StringComparison.CurrentCulture) <= 0)
        {
            NonExceptionFailure();
            return;
        }

        PredicateSuccess();
    }
}