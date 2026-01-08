namespace Snobol4.Common;

public partial class Executive
{
    internal void Ampersand(List<Var> arguments)
    {
        var v = arguments[0];

        // Unary & operation requires a named variable
        if (v.Symbol == "")
        {
            LogRuntimeException(212);
            return;
        }

        // &operator must be existing keyword
        var newSymbol = "&" + v.Symbol;

        if (!IdentifierTable.TryGetValue(newSymbol, out var keywordVar))
        {
            LogRuntimeException(251);
            return;
        }

        SystemStack.Push(keywordVar);
    }
}