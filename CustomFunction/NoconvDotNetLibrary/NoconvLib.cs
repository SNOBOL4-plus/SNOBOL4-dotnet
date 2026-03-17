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

        executive.FunctionTable["traverser"] = new FunctionTableEntry(
            executive, "traverser",
            args => Traverser(args),
            1, false);

        executive.FunctionTable["tableinspector"] = new FunctionTableEntry(
            executive, "tableinspector",
            args => TableInspector(args),
            1, false);

        executive.FunctionTable["datafieldcount"] = new FunctionTableEntry(
            executive, "datafieldcount",
            args => DataFieldCount(args),
            1, false);
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
            if (v.Convert(VarType.INTEGER, out _, out var iv, _exec))
                sum += (long)iv;
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
