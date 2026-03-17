using Snobol4.Common;

namespace NoconvDotNetLibrary;

/// <summary>
/// IExternalLibrary fixture for net-ext-noconv Step 5 tests.
///
/// Registers two functions that receive SNOBOL4 ARRAY/TABLE/DATA objects
/// as live Var references (via the IExternalLibrary fast path, which bypasses
/// coercion and delivers the original Var directly) and use the Executive
/// traversal API to inspect them.
///
///   Traverser(arr)       — sums integer elements of an ArrayVar; returns INTEGER
///   TableInspector(tbl)  — counts key-value pairs in a TableVar; returns INTEGER
///   DataFieldCount(pdv)  — counts fields in a ProgramDefinedDataVar; returns INTEGER
/// </summary>
public sealed class NoconvLib : IExternalLibrary
{
    private Executive _exec = null!;

    public void Init(Executive executive)
    {
        _exec = executive;
        Register(executive, "Traverser",      1, args => Traverser(args));
        Register(executive, "TableInspector", 1, args => TableInspector(args));
        Register(executive, "DataFieldCount", 1, args => DataFieldCount(args));
    }

    private static void Register(Executive executive, string name, int arity, FunctionTableEntry.FunctionHandler fn)
    {
        var key = executive.Parent.FoldCase(name);
        executive.FunctionTable[key] = new FunctionTableEntry(executive, key, fn, arity, false);
    }

    private void Traverser(List<Var> args)
    {
        if (args.Count < 1 || args[0] is not ArrayVar arr)
        {
            _exec.NonExceptionFailure();
            return;
        }

        long sum = 0;
        _exec.TraverseArray(arr, (_, v) =>
        {
            if (v.Convert(Executive.VarType.INTEGER, out Var _, out object iv, _exec))
                sum += (long)(iv);
        });

        _exec.SystemStack.Push(new IntegerVar(sum));
        _exec.Failure = false;
    }

    private void TableInspector(List<Var> args)
    {
        if (args.Count < 1 || args[0] is not TableVar tbl)
        {
            _exec.NonExceptionFailure();
            return;
        }

        long count = 0;
        _exec.TraverseTable(tbl, (_, _) => count++);

        _exec.SystemStack.Push(new IntegerVar(count));
        _exec.Failure = false;
    }

    private void DataFieldCount(List<Var> args)
    {
        if (args.Count < 1 || args[0] is not ProgramDefinedDataVar pdv)
        {
            _exec.NonExceptionFailure();
            return;
        }

        var fields = _exec.GetDataFields(pdv);
        _exec.SystemStack.Push(new IntegerVar(fields.Count));
        _exec.Failure = false;
    }
}
