namespace Snobol4.Common;

// VALUE(name) — returns the value of the variable named by the string argument.
// Equivalent to $'name' (unary indirection on a string).
// When called with a ProgramDefinedDataVar, dispatches to field access
// (handles the case where a user-defined type has a field named "value").
// Fails (error 239) if argument cannot be used as a variable name.

public partial class Executive
{
    internal void Value(List<Var> arguments)
    {
        // If argument is a user-defined data type, treat as field accessor
        if (arguments[0] is ProgramDefinedDataVar pdv)
        {
            // Route to GetProgramDefinedDataField with the same args
            GetProgramDefinedDataField(arguments);
            return;
        }

        string symbol;

        switch (arguments[0])
        {
            case StringVar sv:
                symbol = Parent.FoldCase(sv.Data);
                break;

            case IntegerVar iv:
                symbol = Parent.FoldCase(iv.Data.ToString());
                break;

            case RealVar rv:
                symbol = Parent.FoldCase(rv.Data.ToString(System.Globalization.CultureInfo.InvariantCulture));
                break;

            case NameVar nv:
                symbol = Parent.FoldCase(
                    string.IsNullOrEmpty(nv.Pointer) ? nv.Collection!.Symbol : nv.Pointer);
                break;

            default:
                LogRuntimeException(239);
                return;
        }

        if (string.IsNullOrEmpty(symbol))
        {
            LogRuntimeException(239);
            return;
        }

        SystemStack.Push(IdentifierTable[symbol]);
    }
}
