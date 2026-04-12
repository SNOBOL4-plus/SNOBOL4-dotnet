using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Snobol4.Common;

//"field function argument is wrong datatype" /* 41 */
//"data argument is not a string" /* 75 */
//"data argument is null" /* 76 */
//"data argument is missing a left paren" /* 77 */
//"data argument has null datatype name" /* 78 */
//"data argument is missing a right paren" /* 79 */
//"data argument has null field name" /* 80 */
//"prototype argument is not valid object" /* 164 */,
//"attempted redefinition of system function" /* 248 */


public class UserDataDefinition
{
    internal string Prototype;
    internal List<string> FieldNames;

    internal UserDataDefinition(string prototype, List<string> fieldNames)
    {
        Prototype = prototype;
        FieldNames = fieldNames;
    }
}

public partial class Executive
{
    internal Dictionary<string, UserDataDefinition> UserDataDefinitions = new();

    internal void ProgramDefinedData(List<Var> arguments)
    {
        // data argument cannot be null
        if (arguments.Count == 0)
        {
            LogRuntimeException(76);
            return;
        }

        // data argument must be a string
        if (!arguments[0].Convert(VarType.STRING, out _, out var value, this))
        {
            LogRuntimeException(75);
            return;
        }

        // data argument cannot be null
        var prototype = Parent.FoldCase((string)value).Trim();
        if (prototype == "")
        {
            LogRuntimeException(76);
            return;
        }

        // data argument must not have a null datatype name
        var match = CompiledRegex.ProgramDefinedDataPrototypePattern().Match(prototype);
        if (!match.Success)
        {
            Assert.IsTrue(true, "prototype argument is not valid object in Data()");
            LogRuntimeException(164);
            return;
        }

        // datatype name cannot be null
        var dataName = match.Groups[1].Value.Trim();
        if (dataName == "")
        {
            LogRuntimeException(78);
            return;
        }

        // datatype name cannot be an existing user-defined function
        // (protected builtins may be shadowed by DATA type names, per SNOBOL4 spec)
        //if (FunctionTable.ContainsKey(dataName))
        var existingDataType = FunctionTable[dataName];
        if (existingDataType is not null && !existingDataType.IsProtected)
        {
            LogRuntimeException(248);
            return;
        }

        // must have a left paren
        var lParen = match.Groups[2].Value;
        if (lParen == "")
        {
            LogRuntimeException(77);
            return;
        }

        // must have a right paren
        var rParen = match.Groups[4].Value;
        if (rParen == "")
        {
            LogRuntimeException(79);
            return;
        }

        // TODO Does this ignore consecutive commas like DEFINE
        var fields = new List<string>(match.Groups[3].Value.Split(','));
        for (var i = 0; i < fields.Count; i++)
        {
            fields[i] = Parent.FoldCase(fields[i].Trim());

            // field name cannot be null
            if (fields[i] == "")
            {
                LogRuntimeException(80);
                return;
            }

            // field name cannot be an existing user-defined function
            // (protected builtins may be shadowed by DATA field accessors, per SNOBOL4 spec)
            //if (FunctionTable.ContainsKey(fields[i]))
            var existingField = FunctionTable[fields[i]];
            if (existingField is not null && !existingField.IsProtected)
            {
                // Allow redefinition of DATA field accessors (polymorphic dispatch by type)
                // Only block redefinition of user-defined functions (DEFINE'd)
                if (existingField.Handler != GetProgramDefinedDataField)
                {
                    LogRuntimeException(248);
                    return;
                }
                // else: existing field accessor will be overwritten below — that's correct
            }
        }


        FunctionTable[dataName] = new FunctionTableEntry(this, dataName, CreateProgramDefinedDataInstance, fields.Count, false);
        var userDataDefinition = new UserDataDefinition(prototype, fields);
        UserDataDefinitions[dataName] = userDataDefinition;

        foreach (var fieldName in fields)
        {
            var existingEntry = FunctionTable[fieldName];
            if (existingEntry is null || !existingEntry.IsProtected)
            {
                // Only register if no protected builtin owns this name.
                // Protected builtins (e.g. VALUE) keep their slot; GetProgramDefinedDataField
                // dispatches by argument type at runtime so field access still works.
                FunctionTable[fieldName] = new FunctionTableEntry(this, fieldName, GetProgramDefinedDataField, 1, false);
            }
        }

        PredicateSuccess();
    }

    internal void CreateProgramDefinedDataInstance(List<Var> arguments)
    {
        var dataName = ((StringVar)arguments[^1]).Data;
        var dataDefinition = UserDataDefinitions[dataName];
        arguments = arguments[..^1];
        var userDefinedDataVar = new ProgramDefinedDataVar(dataName, dataDefinition.Prototype, dataDefinition.FieldNames);
        var fieldsCount = dataDefinition.FieldNames.Count;
        var fieldValues = userDefinedDataVar.FieldValues.Data;

        for (var i = 0; i < fieldsCount; i++)
        {
            // Strip array/field lvalue bookkeeping (Key/Collection) before storing.
            // AssignReplace stamps destination Key/Collection onto cloned values so
            // they serve as lvalues; storing that into a DATA field causes an infinite
            // dereference cycle in GetProgramDefinedDataField. NameVars are preserved.
            var fv = arguments[i];
            if (fv is not NameVar && fv.Collection != null)
            {
                fv = fv.Clone();
                fv.Key        = null;
                fv.Collection = null;
            }
            fieldValues[i] = fv;
        }

        SystemStack.Push(userDefinedDataVar);
    }

    internal void GetProgramDefinedDataField(List<Var> arguments)
    {
        var fieldName = ((StringVar)arguments[1]).Data;
        // Dereference one level of NAME indirection (e.g. nd = Top() returns a NameVar
        // pointing to a DATA field; resolve it before casting to ProgramDefinedDataVar).
        Var arg0 = arguments[0] is NameVar nv0 ? nv0.Dereference(this) : arguments[0];
        var programDefinedDataVar = (ProgramDefinedDataVar)arg0;
        object index = (long)programDefinedDataVar.Definition.FieldNames.IndexOf(fieldName);
        var v = programDefinedDataVar.FieldValues.Data[(int)(long)index];
        v.Key = index;
        v.Collection = programDefinedDataVar.FieldValues;
        SystemStack.Push(v);
    }
}