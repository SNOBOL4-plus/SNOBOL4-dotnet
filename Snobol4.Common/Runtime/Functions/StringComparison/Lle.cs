namespace Snobol4.Common;

//"lle first argument is not a string" /* 128 */,
//"lle second argument is not a string" /* 129 */,

public partial class Executive
{
    internal void LexicalLessThanOrEqual(List<Var> arguments)
    {
        var v0 = arguments[0];
        var v1 = arguments[1];

        if (!v0.Convert(VarType.STRING, out _, out var left, this))
        {
            LogRuntimeException(128);
            return;
        }

        if (!v1.Convert(VarType.STRING, out _, out var right, this))
        {
            LogRuntimeException(129);
            return;
        }

        if (string.Compare((string)left, (string)right, StringComparison.CurrentCulture) > 0)
        {
            NonExceptionFailure();
            return;
        }

        PredicateSuccess();
    }
}