using System.Text;

namespace Snobol4.Common;

//"pattern replacement right operand is not a string" /* 31 */,
//"keyword value assigned is not integer" /* 208 */,
//"keyword in assignment is protected" /* 209 */,
//"keyword value assigned is negative or too large" /* 210 */,
//"value assigned to keyword errtext not a string" /* 211 */,
//"syntax error: value used where name is required" /* 212 */,

public partial class Executive
{
    public void _BinaryEquals()
    {
        using var profile1 = Profiler.Start3("_BinaryEquals", this);

        if (Parent.BuildOptions.TraceStatements)
            Console.Error.WriteLine(@"_BinaryEquals");
        // Do not delete. Used by DLL

        // Get all arguments and check for prior failure
        List<Var> arguments = [];

        if (SystemStack.ExtractArguments(2, arguments, this, 1))
        {
            return;
        }

        if (arguments[0] is SubjectVar)
        {
            ReplaceMatch(arguments);
        }
        else
        {
            Assign(arguments);
        }
    }

    internal void ReplaceMatch(List<Var> arguments)
    {
        var subjectVar = (SubjectVar)arguments[0];

        // Replacement must resolve to a string
        if (!arguments[1].Convert(VarType.STRING, out _, out var subject, this))
        {
            LogRuntimeException(31);
            return;
        }

        // Perform the replacement
        var resultsVar = subjectVar.MatchReplace((string)subject);
        var v = IdentifierTable[resultsVar.Symbol] = resultsVar;
        SystemStack.Push(v);
    }

    internal void Assign(List<Var> arguments)
    {
        while (arguments[0] is ExpressionVar expressionVar1)
        {
            expressionVar1.FunctionName(this);
            arguments[0] = SystemStack.Pop();
        }

        var leftVar = arguments[0];  // Destination Var
        var rightVar = arguments[1]; // Source Var

        // If source has input channels, get input now
        rightVar = InputArgument(rightVar);

        //LeftPattern side must have a symbol, be keyed to a collection, or be a NameVar
        if (leftVar is { Symbol: "", Key: null } and not NameVar)
        {
            LogRuntimeException(leftVar.IsKeyword ? 211 : 212);
            return;
        }

        // Protected natural variables (arb, bal, fail, fence, rem, succeed) — error 42
        if (!leftVar.IsKeyword && leftVar.IsReadOnly)
        {
            LogRuntimeException(42);
            return;
        }

        // Special checks for keywords
        if (leftVar.IsKeyword)
        {
            // LeftPattern side must be writable
            if (leftVar.IsReadOnly)
            {
                LogRuntimeException(209);
                return;
            }


            // If leftVar side is &errtext, rightVar side must be a string
            if (leftVar.Symbol == "&errtext" && leftVar is not StringVar)
            {
                LogRuntimeException(211);
                return;
            }

            // Otherwise, right side must be an integer
            if (rightVar is not IntegerVar integerVar)
            {
                LogRuntimeException(208);
                return;
            }

            if (KeywordTable.TryGetValue(leftVar.Symbol, out var handler))
            {
                handler(rightVar, true);
                // Sync-step monitor fire-point for keyword assignment.
                if (MonitorIpc.Enabled)
                    MonitorIpc.EmitValue(leftVar.Symbol ?? "", rightVar);
                return;
            }
        }

        switch (leftVar.Collection)
        {
            case ArrayVar arrayVar:
            {
                // Dereference NameVar rvalue before storing into the array slot.
                // If rightVar is a NameVar (e.g. returned via NRETURN from Shift/Push),
                // storing it directly and then stamping Key/Collection onto it creates a
                // self-referential cycle: arrayVar.Data[i] → NameVar(Collection=arrayVar, Key=i)
                // → arrayVar.Data[i] → ... indefinitely (ShiftReduce S-10 root cause).
                var rval = rightVar is NameVar rvNv ? rvNv.Dereference(this) : rightVar;
                // Clone rightVar before mutating Key/Collection so the source variable's
                // VarSlotArray slot (if rightVar came from PushVar) is not contaminated.
                var arrVal = rval is ArrayVar or TableVar ? rval : rval.Clone();
                arrVal.Key        = leftVar.Key;
                arrVal.Collection = leftVar.Collection;
                arrayVar.Data[(int)(long)leftVar.Key!] = arrVal;
                SystemStack.Push(arrVal);
                break;
            }

            case TableVar tableVar:
            {
                var rval = rightVar is NameVar rvNv ? rvNv.Dereference(this) : rightVar;
                var tblVal = rval is ArrayVar or TableVar ? rval : rval.Clone();
                tblVal.Key        = leftVar.Key;
                tblVal.Collection = leftVar.Collection;
                tableVar.Data[leftVar.Key!] = tblVal;
                SystemStack.Push(tblVal);
                break;
            }

            default:
                // If the lvalue is a NameVar (e.g. returned via NRETURN), assign through
                // its Pointer — the actual variable being referenced — not leftVar.Symbol.
                var targetSymbol = leftVar is NameVar nameVar ? nameVar.Pointer : leftVar.Symbol;
                var newVar = rightVar is ArrayVar or TableVar ? rightVar : rightVar.Clone();
                newVar.Symbol = targetSymbol;
                // Strip array/table bookkeeping from the stored scalar value.
                // A value read from A<K> carries .Collection=arrayVar and .Key=k so
                // that Assign can write back through it when used as an lvalue.
                // When stored into a plain scalar variable (not a NameVar, which needs
                // its own Key/Collection to track its referent), those fields must be
                // cleared — otherwise a later use of the scalar as an lvalue routes
                // into the array write branch instead of the scalar IdentifierTable path.
                if (newVar is not NameVar && newVar is not ArrayVar && newVar is not TableVar)
                {
                    newVar.Key        = null;
                    newVar.Collection = null;
                }
                newVar.OutputChannel = leftVar.OutputChannel;
                newVar.InputChannel = leftVar.InputChannel;
                IdentifierTable[newVar.Symbol] = newVar;
                SystemStack.Push(newVar);
                break;
        }

        // Sync-step monitor fire-point: emit VALUE record on the just-stored value.
        // Mirrors csn ASGNVV (v311.sil:5938) and spl asign:asg01 (sbl.min:17596).
        // No-op when MonitorIpc is dormant.  See GOAL-NET-BEAUTY-SELF S-2-bridge-2.
        if (MonitorIpc.Enabled)
        {
            var stored = SystemStack.Peek();
            // Lvalue name extraction:
            //   * Scalar              → stored.Symbol (e.g. "S" for S = ...)
            //   * Array element a<i>  → stored.Symbol is empty, but stored.Collection
            //                            points at the ArrayVar; use its Symbol ("a").
            //   * Table slot d<'k'>   → same, use the TableVar's Symbol ("d").
            //   * Truly anonymous     → fall through to MonitorIpc.LvalueNameId's
            //                            <lval> sentinel.
            // S-2-bridge-7-lval (Mon Apr 28 2026) — this gives semantically-meaningful
            // names for aggregate-element stores so the wire identifies WHICH table or
            // array was written.  Both csn and spl bridges use <lval> here today; the
            // dot wire is strictly more informative.  See GOAL-NET-BEAUTY-SELF.
            string lvalueName = stored.Symbol ?? "";
            if (string.IsNullOrEmpty(lvalueName) && stored.Collection?.Symbol is { Length: > 0 } collSym)
            {
                lvalueName = collSym;
            }
            // Suppress VALUE emission when the lvalue is the current function's return
            // variable slot (i.e. "fnName = value" inside the function body).
            // Define.cs ExecuteProgramDefinedFunction already emits VALUE for the
            // return slot after the function exits (mirrors csn DEFF20 / spl retrn).
            // Emitting here too produces a duplicate VALUE before the RETURN event,
            // which diverges from csn/spl wire order.  See GOAL-NET-BEAUTY-SELF S-2-bridge-7-fullscan.
            bool isReturnSlot = ProgramDefinedFunctionStack.TryPeek(out var callerFn)
                                && callerFn == lvalueName;
            if (!isReturnSlot)
                MonitorIpc.EmitValue(lvalueName, stored);
        }

        if (SystemStack.Peek().OutputChannel == "")
        {
            return;
        }

        // Call output function
        var outputVar = SystemStack.Peek();

        switch (outputVar.OutputChannel)
        {
            case "+console-output":
            case "+terminal-output":
                Console.Error.WriteLine(outputVar.ToString());
                break;

            case "+console-output-nnl":
                Console.Error.Write(outputVar.ToString());
                break;
            
            default:
                StreamOutputs[outputVar.OutputChannel].Write(Encoding.UTF8.GetBytes(outputVar + Environment.NewLine));
                break;
        }
    }
}