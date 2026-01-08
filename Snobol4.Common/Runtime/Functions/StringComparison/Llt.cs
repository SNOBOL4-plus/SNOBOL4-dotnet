namespace Snobol4.Common;

public partial class Executive
{
    //"llt first argument is not a string" /* 130 */,
    //"llt second argument is not a string" /* 131 */,

    internal void LexicalLessThan(List<Var> arguments)
    {
        var v0 = arguments[0];
        var v1 = arguments[1];

        if (!v0.Convert(VarType.STRING, out _, out var left, this))
        {
            LogRuntimeException(130);
            return;
        }

        if (!v1.Convert(VarType.STRING, out _, out var right, this))
        {
            LogRuntimeException(131);
            return;
        }

        if (string.Compare((string)left, (string)right, StringComparison.CurrentCulture) >= 0)
        {
            NonExceptionFailure();
            return;
        }

        PredicateSuccess();
    }
}