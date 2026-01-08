namespace Snobol4.Common;

public partial class Executive
{
    //"leq first argument is not a string" /* 122 */,
    //"leq second argument is not a string" /* 123 */,

    internal void LexicalEqual(List<Var> arguments)
    {
        var v0 = arguments[0];
        var v1 = arguments[1];

        if (!v0.Convert(VarType.STRING, out _, out var left, this))
        {
            LogRuntimeException(122);
            return;
        }

        if (!v1.Convert(VarType.STRING, out _, out var right, this))
        {
            LogRuntimeException(123);
            return;
        }

        if (string.Compare((string)left, (string)right, StringComparison.CurrentCulture) != 0)
        {
            NonExceptionFailure();
            return;
        }

        PredicateSuccess();
    }
}