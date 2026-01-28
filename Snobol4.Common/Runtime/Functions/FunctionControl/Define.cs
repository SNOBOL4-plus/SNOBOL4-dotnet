using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Snobol4.Common;

public class UserFunctionDefinition
{
    internal string Prototype;
    internal List<string> Locals;
    internal string EntryLabel;

    internal UserFunctionDefinition(string prototype, List<string> locals, string entryLabel)
    {
        Prototype = prototype;
        Locals = locals;
        EntryLabel = entryLabel;
    }
}
public partial class Executive
{
    internal Dictionary<string, UserFunctionDefinition> UserFunctionDefinitions = new();

    internal void CreateProgramDefinedFunction(List<Var> arguments)
    {

        // define argument cannot be null
        if (arguments.Count == 0)
        {
            LogRuntimeException(82);
            return;
        }

        // define argument must be a string
        if (!arguments[0].Convert(VarType.STRING, out _, out var value, this))
        {
            LogRuntimeException(81);
            return;
        }

        // define argument cannot be null
        var prototype = ((string)value).Trim();
        if (prototype == "")
        {
            LogRuntimeException(81);
            return;
        }

        // define argument must not have a null datatype name
        var match = CompiledRegex.FunctionPrototypePattern().Match(prototype);
        if (!match.Success)
        {
            Assert.IsTrue(true, "prototype argument is not valid object in CreateProgramDefinedFunction()");
            LogRuntimeException(1000);
            return;
        }

        // function name cannot be null
        var functionName = match.Groups[1].Value.Trim();
        if (functionName == "")
        {
            LogRuntimeException(84);
            return;
        }

        // function name cannot be an existing function
        if (FunctionTable.ContainsKey(functionName))
        {
            LogRuntimeException(248);
            return;
        }

        // must have a left paren
        var lParen = match.Groups[2].Value;
        if (lParen == "")
        {
            LogRuntimeException(83);
            return;
        }

        // must have a right paren
        var rParen = match.Groups[4].Value;
        if (rParen == "")
        {
            LogRuntimeException(85);
            return;
        }

        // Get entry label
        string entryLabel = "";
        if (arguments[1] is StringVar entryVar && entryVar.Data == "")
        {
            entryLabel = functionName;
        }
        else
        {
            if (!arguments[1].Convert(VarType.NAME, out Var entry, out _, this))
            {
                LogRuntimeException(86);
            }

            entryLabel = ((NameVar)entry).Pointer;
        }

        if (!Labels.ContainsKey(entryLabel))
        {
            LogRuntimeException(86);
        }

        //internal UserFunctionDefinition(string prototype, List<string> locals, string entryLabel)

        // Build table entry
        var parameters = new List<string>(match.Groups[3].Value.Split(','));
        List<string> locals = [];
        locals.AddRange(parameters.Select(t => t.Trim()).Where(parameter => parameter != ""));
        var argumentCount = locals.Count;
        parameters = new List<string>(match.Groups[5].Value.Split(','));
        locals.AddRange(parameters.Select(t => t.Trim()).Where(parameter => parameter != ""));
        locals.Add(functionName);
        var newEntry = new FunctionTableEntry(functionName, ExecuteProgramDefinedFunction, argumentCount, false);
        FunctionTable[functionName] = newEntry;

        // Build user function definition entry
        var userFunctionDefinition = new UserFunctionDefinition(prototype, locals, entryLabel);
        UserFunctionDefinitions[functionName] = userFunctionDefinition;

        PredicateSuccess();
    }

    internal void ExecuteProgramDefinedFunction(List<Var> arguments)
    {
        var functionName = ((StringVar)arguments[^1]).Data;
        ProgramDefinedFunctionStack.Push(functionName);
        var entry = FunctionTable[functionName];
        List<Var> saveVars = [];
        var definition = UserFunctionDefinitions[functionName];

        for (var i = 0; i < definition.Locals.Count; ++i)
        {
            // Save the current value of local variables
            var symbol = definition.Locals[i];

            // If the identifier does not exist, make it now
            if (!IdentifierTable.ContainsKey(symbol))
            {
                IdentifierTable[symbol] = StringVar.Null();
            }

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
                var sVar = StringVar.Null(definition.Locals[i]);
                IdentifierTable[sVar.Symbol] = sVar;
            }
        }

        // Save value of local variables on the stack
        entry.StateStack.Push(saveVars);

        // Update keywords
        ((IntegerVar)IdentifierTable["&fnclevel"]).Data++;

        if (((IntegerVar)IdentifierTable["&ftrace"]).Data > 0)
        {
            FunctionTraceEntry(arguments, functionName);
        }

        // Run function by transferring to the entry label
        ExecuteLoop(Labels[definition.EntryLabel]);

        var returnVar = IdentifierTable[functionName];
        SystemStack.Push(returnVar);
        ((IntegerVar)IdentifierTable["&fnclevel"]).Data--;

        // Post-processing
        if (((IntegerVar)IdentifierTable["&ftrace"]).Data > 0)
        {
            FunctionTraceExit(functionName);
        }

        // Restore local variables
        saveVars = entry.StateStack.Pop();

        for (var i = 0; i < definition.Locals.Count; ++i)
        {
            IdentifierTable[saveVars[i].Symbol] = saveVars[i];
        }
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