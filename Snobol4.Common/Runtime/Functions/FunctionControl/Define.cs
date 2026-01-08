using System.Diagnostics;

namespace Snobol4.Common;

public partial class Executive
{
    internal Stack<string> ProgramDefinedFunctionStack = [];

    internal void CreateProgramDefinedFunction(List<Var> arguments)
    {
        if (arguments.Count == 0)
        {
            LogRuntimeException(82);
            return;
        }

        if (!arguments[0].Convert(VarType.STRING, out _, out var value, this))
        {   
            LogRuntimeException(81);
            return;
        }

        var prototype = (string)value;

        Debug.Assert(prototype != null, nameof(prototype) + " != null");
        if (prototype.Trim().Length == 0)
        {
            LogRuntimeException(82); // Not a valid prototype
            return;
        }

        var match = CompiledRegex.FunctionPrototypePattern().Match(prototype);
        if (!match.Success)
        {
            LogRuntimeException(85); // Not a valid prototype
            return;
        }

        var functionName = match.Groups[1].Value;
        var locals = new List<string>(match.Groups[2].Value.Split(','));

        // Allow functions with exactly one null argument (e.g., 'shift()')
        if (locals is [{ Length: 0 }])
            locals.Clear();

        var argumentCount = locals.Count; // The first set of locals is the arguments
        var parameters = match.Groups[3].Value.Split(',');
        if (parameters.Length > 0 && parameters[0].Length > 0)
            locals.AddRange(parameters);
        locals.Add(functionName);

        if (locals.Any(str => str.Length == 0 || !CompiledRegex.FunctionPrototypeIdentifierPattern().Match(str).Success))
        {
            LogRuntimeException(85);
            return;
        }

        var newEntry = new FunctionTableEntry(functionName, ExecuteProgramDefinedFunction, argumentCount, locals, prototype);
        FunctionTable[functionName] = newEntry;
        PredicateSuccess();
    }

    internal void ExecuteProgramDefinedFunction(List<Var> arguments)
    {
        var functionName = ((StringVar)arguments[^1]).Data;
        ProgramDefinedFunctionStack.Push(functionName);
        var entry = FunctionTable[functionName];
        List<Var> saveVars = [];

        for (var i = 0; i < entry.Locals.Count; ++i)
        {
            // Save the current value of local variables
            var symbol = entry.Locals[i];

            // If the identifier does not exist, make it now
            if (!IdentifierTable.ContainsKey(symbol))
                IdentifierTable[symbol] = StringVar.Null();

            var v = IdentifierTable[symbol];
            saveVars.Add(v);

            // Set local variables to the new values
            if (i < entry.ArgumentCount)
            {
                arguments[i].Symbol = symbol;
                IdentifierTable[arguments[i].Symbol] = arguments[i];
            }
            else
            {
                var sVar = StringVar.Null(entry.Locals[i]);
                IdentifierTable[sVar.Symbol] = sVar;
            }
        }

        // Save value of local variables on the stack
        entry.StateStack.Push(saveVars);

        ((IntegerVar)IdentifierTable["&fnclevel"]).Data++;

        if (((IntegerVar)IdentifierTable["&ftrace"]).Data > 0)
            FunctionTraceEntry(arguments, functionName);


        ExecuteLoop(Labels[functionName]);
        var returnVar = IdentifierTable[functionName];
        SystemStack.Push(returnVar);

        ((IntegerVar)IdentifierTable["&fnclevel"]).Data--;

        if (((IntegerVar)IdentifierTable["&ftrace"]).Data > 0)
            FunctionTraceExit(functionName);

        // Restore local variables
        saveVars = entry.StateStack.Pop();
        for (var i = 0; i < entry.Locals.Count; ++i)
            IdentifierTable[saveVars[i].Symbol] = saveVars[i];
    }

    private void FunctionTraceEntry(List<Var> arguments, string functionName)
    {
        var line = ((IntegerVar)IdentifierTable["&line"]).Data + 1;
        var linePad = line.ToString().PadRight(8, '*');
        var r = (int)((IntegerVar)IdentifierTable["&fnclevel"]).Data;
        var args = "";

        for (var i = 1; i < arguments.Count; ++i)
        {
            args += (i > 1 ? ", " : "") + arguments[i - 1];
        }

        Console.Error.WriteLine($@"****{linePad} {new string('i', r - 1)} {functionName}({args})");
        ((IntegerVar)IdentifierTable["&ftrace"]).Data--;
    }

    private void FunctionTraceExit(string functionName)
    {
        var line = ((IntegerVar)IdentifierTable["&line"]).Data + 1;
        var linePad = line.ToString().PadRight(8, '*');
        var r = (int)((IntegerVar)IdentifierTable["&fnclevel"]).Data;
        Console.Error.WriteLine($@"****{linePad} {new string('i', r)} return {functionName} = {SystemStack.Peek()}");
        ((IntegerVar)IdentifierTable["&ftrace"]).Data--;
    }

}