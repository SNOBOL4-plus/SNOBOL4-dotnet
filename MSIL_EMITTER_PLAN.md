# BuilderEmitMsil — Implementation Plan
## Branch: `feature/msil-emitter`  (branched from `feature/post-threaded-dev` at d608197)

---

## Context for the Implementing Claude

### Repo
- **Remote:** `https://LCherryholmes:REDACTED_TOKEN@github.com/jcooper0/Snobol4.Net.git`
- **Working branch:** `feature/msil-emitter`
- **Base commit:** `d608197` — "Step 8: Add PluginLoadContext + 25 LOAD/UNLOAD tests — 1413 passing"
- **Baseline test count:** 1413 passed, 0 failed (run `dotnet test TestSnobol4/TestSnobol4.csproj -c Release`)
- **git config:** `user.email=lcherryh@yahoo.com`, `user.name=LCherryholmes`
- **dotnet:** 10.0.103 — use `export PATH=$PATH:/usr/local/dotnet` in bash

### Solution layout
```
Snobol4.Net/
  Snobol4.Common/          ← main library, all work happens here
    Builder/
      Builder.cs           ← compile pipeline (BuildMain, BuildCode, BuildEval, BuildForTest)
      BuilderResolve.cs    ← ResolveSlots() — populates VariableSlots, FunctionSlots, Constants
      BuilderEmitMsil.cs   ← DOES NOT EXIST YET — Jeffrey's draft uploaded, needs fixes
      ThreadedCodeCompiler.cs ← emits Instruction[] from token lists
      Instruction.cs       ← OpCode enum + Instruction struct
      Token.cs             ← Token.Type enum + Token class
      ConstantPool.cs      ← interned Var pool
      FunctionSlot.cs      ← per-call-site slot (symbol + argCount)
      VariableSlot.cs      ← per-variable slot (symbol)
    Runtime/Execution/
      ThreadedExecuteLoop.cs ← main dispatch loop (switch on OpCode)
      ExecutionCache.cs    ← VarSlotArray, OperatorHandlers, OperatorFast()
      StatementControl.cs  ← RunExpressionThread()
      Executive.cs         ← partial class root, _reusableArgList
      MsilHelpers.cs       ← DOES NOT EXIST YET — Step 1 creates this
    Runtime/Functions/
      Execution/Function.cs ← Function(int argCount) — model for CallFuncBySlot
  TestSnobol4/
    ThreadedCompilerTests.cs ← existing threaded compiler tests (model for new tests)
    MsilEmitterTests.cs    ← DOES NOT EXIST YET — Step 5 creates this
```

---

## What This Feature Does

`ThreadedExecuteLoop` currently dispatches every opcode through a `while`/`switch` loop.
For a tight loop executing 10,000 iterations, the switch is evaluated ~10,000 × (opcodes
per statement) times. This accounts for **21.3% of all trace time** in benchmarks.

`BuilderEmitMsil.cs` eliminates this by JIT-compiling each statement's expression-level
token list into a `DynamicMethod` delegate (`Action<Executive>`) at program load time.
The delegate is cached in `MsilCache` keyed by `List<Token>` reference. At runtime, one
`CallMsil` opcode invokes the cached delegate, replacing the individual `PushVar`,
`PushConst`, `CallFunc`, and operator opcodes with a straight-line native call sequence.

**Control flow is not emitted** — `Init`, `Finalize`, `Jump`, `JumpOnSuccess`,
`JumpOnFailure`, `Halt`, `GotoIndirect` all stay in `ThreadedExecuteLoop`. Only the
expression body between `Init` and `Finalize` is JIT-compiled.

---

## Jeffrey's Draft

Jeffrey uploaded `BuilderEmitMsil.cs` (the draft). The architecture is correct.
Three categories of bugs prevent it from compiling and running:

### Bug 1 — Wrong Token.Type names (15 compile errors)

The draft uses bare names; the actual `Token.Type` enum uses `BINARY_` prefixes,
unary ops share one enum value, indexing and assignment have different names:

| Draft uses | Actual `Token.Type` |
|------------|---------------------|
| `PLUS` | `BINARY_PLUS` |
| `MINUS` | `BINARY_MINUS` |
| `STAR` | `BINARY_STAR` |
| `SLASH` | `BINARY_SLASH` |
| `CARET` | `BINARY_CARET` |
| `BLANK` | `BINARY_CONCAT` |
| `BAR` | `BINARY_PIPE` |
| `PERIOD` | `BINARY_PERIOD` |
| `DOLLAR` | `BINARY_DOLLAR` |
| `QUESTION` | `BINARY_QUESTION` |
| `AT` | `BINARY_AT` |
| `AMPERSAND` | `BINARY_AMPERSAND` |
| `PERCENT` | `BINARY_PERCENT` |
| `HASH` | `BINARY_HASH` |
| `TILDE` | `BINARY_TILDE` |
| `EQUALS` | `BINARY_EQUAL` |
| `L_BRACKET` | `R_SQUARE` (and `R_ANGLE`) |
| `UNARY_MINUS` … `UNARY_SLASH` (11 separate cases) | `UNARY_OPERATOR` — one case, dispatch on `t.MatchedString` |

The `UNARY_OPERATOR` dispatch pattern (from `ThreadedCodeCompiler.cs`) is:
```csharp
case Token.Type.UNARY_OPERATOR:
    // t.MatchedString is "-", "+", "$", "&", ".", "~", "?", "@", "%", "#", "/"
    // or a user-defined opsyn character
    var opCode = t.MatchedString switch {
        "-" => OpCode.OpUnaryMinus,
        "+" => OpCode.OpUnaryPlus,
        "$" => OpCode.OpIndirection,
        "&" => OpCode.OpKeyword,
        "." => OpCode.OpName,
        "~" => OpCode.OpNegation,
        "?" => OpCode.OpInterrogation,
        "@" => OpCode.OpUnaryAt,
        "%" => OpCode.OpUnaryPercent,
        "#" => OpCode.OpUnaryHash,
        "/" => OpCode.OpUnarySlash,
        _   => OpCode.OpUnaryOpsyn   // user-defined opsyn operator
    };
    if (opCode != OpCode.OpUnaryOpsyn)
        EmitOperator(il, opCode, 1);
    else
    {
        // opsyn-defined unary: constant pool holds "_X" key
        var poolIdx = Constants.GetOrAddString("_" + t.MatchedString);
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldc_I4, poolIdx);
        il.Emit(OpCodes.Ldc_I4, 1);
        il.Emit(OpCodes.Call, _operatorFastOpsyn);  // needs separate MethodInfo
    }
    break;
```

### Bug 2 — Three missing Executive methods (3 compile errors)

The draft calls methods that don't exist. They must be added in `MsilHelpers.cs`:

```csharp
// PushVarBySlot — mirrors the PushVar case in ThreadedExecuteLoop
internal void PushVarBySlot(int slotIdx)
{
    if (slotIdx >= VarSlotArray.Length) ExpandVarSlotArray();
    SystemStack.Push(VarSlotArray[slotIdx]);
}

// PushConstByIndex — mirrors the PushConst case in ThreadedExecuteLoop
internal void PushConstByIndex(int poolIdx)
{
    SystemStack.Push(Parent.Constants.Pool[poolIdx].Clone());
}

// CallFuncBySlot — mirrors Function() but resolves by slot index, not popped name
// The function name StringVar is NOT on the stack (unlike the current Function() path).
// The slot holds the canonical name; look it up in FunctionTable, extract args, dispatch.
internal void CallFuncBySlot(int slotIdx, int argCount)
{
    if (Failure) return;
    var slot = Parent.FunctionSlots[slotIdx];
    var entry = FunctionTable[slot.Symbol];
    if (entry == null) { LogRuntimeException(22); return; }
    _reusableArgList.Clear();
    if (SystemStack.ExtractArguments(argCount, _reusableArgList, this)) return;
    for (var i = _reusableArgList.Count; i < entry.ArgumentCount; i++)
        _reusableArgList.Add(StringVar.Null());
    InputArguments(_reusableArgList);
    _reusableArgList.Add(new StringVar(slot.Symbol));  // name as last arg
    entry.Handler(_reusableArgList);
}

// PushExprByIndex — mirrors the PushExpr case in ThreadedExecuteLoop
internal void PushExprByIndex(int exprIdx)
{
    Constant(StarFunctionList[exprIdx]);
}
```

Note: `CallFuncBySlot` does **not** push or pop the function name `StringVar` from the
stack. In the current `ThreadedCodeCompiler` path, `IDENTIFIER_FUNCTION` emits a
`PushConst` (the name) before the arguments, and `Function()` pops it at the end. In
the MSIL path, the name is never on the stack — it comes from the slot directly.
The draft's `EmitAndCache` already omits the `PushConst` for function names (it pushes
the name to `pendingFunctionNames` instead). This is correct.

### Bug 3 — Missing token coverage (silent wrong results)

| Token | Currently missing | Fix |
|-------|------------------|-----|
| `EXPRESSION` | Star function `*(...)` sub-expressions | Add case: emit call to `PushExprByIndex(exprIdx)` where `exprIdx = int.Parse(t.MatchedString[4..])` (same logic as `ThreadedCodeCompiler`) |
| `UNARY_OPERATOR` with opsyn name | User-defined unary ops | See `OpUnaryOpsyn` subcase above |
| `COMMA_CHOICE` | Choice operator `(A,B)` | See IL branch pattern below |
| `R_PAREN_CHOICE` | Closes choice construct | See IL branch pattern below |

**COMMA_CHOICE / R_PAREN_CHOICE IL pattern:**

The threaded compiler pushes instruction indices and patches them later. In IL we use
`ILGenerator.DefineLabel()` and `MarkLabel()`. Replace `_choiceStack` (instruction
indices) with a `Stack<Label>`:

```csharp
// Replace field:
var choiceLabels = new Stack<Label>();  // local in EmitAndCache

// COMMA_CHOICE:
case Token.Type.COMMA_CHOICE:
{
    var skipLabel = il.DefineLabel();
    choiceLabels.Push(skipLabel);
    // if (!Failure) goto skipLabel  — mirrors JumpOnSuccess
    il.Emit(OpCodes.Ldarg_0);
    il.Emit(OpCodes.Ldfld, _failureField);      // executive.Failure
    il.Emit(OpCodes.Brtrue_S, skipLabel);       // if Failure==false, jump past alternative (wait — see note)
    // Actually: JumpOnSuccess means "jump if NOT failure" i.e. jump if Failure==false
    // ChoiceStart: if (Failure) { SystemStack.Pop(); Failure = false; }
    il.Emit(OpCodes.Ldarg_0);
    il.Emit(OpCodes.Call, _choiceStartMethod);
    break;
}

// R_PAREN_CHOICE:
case Token.Type.R_PAREN_CHOICE:
{
    int levels = (int)t.IntegerValue;
    for (int i = 0; i < levels; i++)
        if (choiceLabels.Count > 0)
            il.MarkLabel(choiceLabels.Pop());
    break;
}

// After loop — patch single-comma choices:
while (choiceLabels.Count > 0)
    il.MarkLabel(choiceLabels.Pop());
```

Add to reflected MethodInfo section:
```csharp
private static readonly FieldInfo _failureField =
    typeof(Executive).GetField("Failure",
        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
    ?? throw new MissingFieldException(nameof(Executive), "Failure");

private static readonly MethodInfo _choiceStartMethod =
    typeof(Executive).GetMethod("ChoiceStart",
        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
        null, [], null)
    ?? throw new MissingMethodException(nameof(Executive), "ChoiceStart");
```

Add `ChoiceStart()` helper to `MsilHelpers.cs`:
```csharp
internal void ChoiceStart()
{
    if (Failure) { SystemStack.Pop(); Failure = false; }
}
```

---

## Implementation Steps

### Step 1 — Add Executive helper methods
**New file:** `Snobol4.Common/Runtime/Execution/MsilHelpers.cs`

Add `PushVarBySlot`, `PushConstByIndex`, `CallFuncBySlot`, `PushExprByIndex`,
and `ChoiceStart` as shown above.

Build: `dotnet build Snobol4.Common/Snobol4.Common.csproj -c Release`  
Verify: compiles clean.  
**Commit + push:** `"Step 1: Add MSIL helper methods to Executive (MsilHelpers.cs)"`

---

### Step 2 — Fix and add BuilderEmitMsil.cs
**New file:** `Snobol4.Common/Builder/BuilderEmitMsil.cs`

Start from Jeffrey's draft and apply all fixes from Bug 1, 2, 3 above:
- Fix all 15 `BINARY_` prefix token names
- Replace 11 `UNARY_*` cases with one `UNARY_OPERATOR` case + `MatchedString` dispatch
- Fix `EQUALS` → `BINARY_EQUAL`
- Fix `L_BRACKET` → `R_SQUARE` (also add `R_ANGLE` for table/array indexing)
- Add `EXPRESSION` case
- Add `COMMA_CHOICE` / `R_PAREN_CHOICE` using `ILGenerator` labels
- Add `_pushExprByIndex`, `_choiceStartMethod`, `_failureField` reflected handles
- Scope `MsilCache` and `EmitMsilForAllStatements` as before
- `DynamicMethod` owner stays `typeof(Executive)`, `skipVisibility: true`

Build: `dotnet build Snobol4.Common/Snobol4.Common.csproj -c Release`  
Verify: compiles clean.  
**Commit + push:** `"Step 2: Add BuilderEmitMsil.cs (fixed from Jeffrey's draft)"`

---

### Step 3 — Add OpCode.CallMsil and emit it from ThreadedCodeCompiler
**Modified files:**
- `Snobol4.Common/Builder/Instruction.cs` — add `CallMsil = 90` to `OpCode` enum (operand = delegate index in a new `MsilDelegates` list)
- `Snobol4.Common/Builder/ThreadedCodeCompiler.cs` — after `EmitTokenList(line.ParseBody)`, check if `MsilCache` has a delegate for that token list and emit `CallMsil(delegateIdx)` in its place (skipping the individual opcodes)

**Design:** `Builder` holds `internal List<Action<Executive>> MsilDelegates = new()`.
When `ThreadedCodeCompiler` compiles a token list that has a cached delegate, it emits
`CallMsil(delegateIdx)` instead of the expanded opcodes. A flag or null-check on
`MsilCache` determines which path to take — the threaded path is the fallback.

Build full solution: `dotnet build`  
**Commit + push:** `"Step 3: Add OpCode.CallMsil, emit from ThreadedCodeCompiler"`

---

### Step 4 — Handle CallMsil in ThreadedExecuteLoop
**Modified file:** `Snobol4.Common/Runtime/Execution/ThreadedExecuteLoop.cs`

Add one case to the switch:
```csharp
case OpCode.CallMsil:
    Parent.MsilDelegates[instr.IntOperand](this);
    break;
```

**Wire EmitMsilForAllStatements into pipeline** (`Builder.cs`):
Call `EmitMsilForAllStatements()` immediately after `ResolveSlots()` in all four build
paths (`BuildMain`, `BuildForTest`, `BuildEval`, `BuildCode`). For `BuildCode`
(CODE/EVAL at runtime), also call it after `AppendCompile` so dynamically compiled
statements get delegates too.

Run full test suite: `dotnet test TestSnobol4/TestSnobol4.csproj -c Release`  
**All 1413 must pass.**  
**Commit + push:** `"Step 4: Wire CallMsil into execute loop and compile pipeline — 1413 passing"`

---

### Step 5 — Tests
**New file:** `TestSnobol4/MsilEmitterTests.cs`

```csharp
// Cache populated
[TestMethod] MsilCache_PopulatedAfterCompile()
    // after SetupScript, Builder.MsilCache.Count > 0

// Arithmetic correctness
[TestMethod] MsilCache_ArithmeticStatement()
    // "        N = 3 + 4\nend" → N == "7"

// Function call
[TestMethod] MsilCache_FunctionCall()
    // "        R = SIZE('hello')\nend" → R == "5"

// Star expression (EXPRESSION token)
[TestMethod] MsilCache_StarExpression()
    // pattern using *(...) executes correctly

// Choice operator
[TestMethod] MsilCache_ChoiceOperator()
    // "(A,B)" selects first successful alternative

// Idempotent double-emit
[TestMethod] MsilCache_IdempotentOnDoubleEmit()
    // calling EmitMsilForAllStatements twice doesn't change count or break results

// Regression — all 1413 still pass
```

Run: `dotnet test TestSnobol4/TestSnobol4.csproj -c Release`  
Verify: 1413 + new tests, 0 failed.  
**Commit + push:** `"Step 5: MsilEmitter tests — all passing"`

---

## Key Relationships to Keep Straight

1. **`IDENTIFIER_FUNCTION` handling differs between paths:**
   - `ThreadedCodeCompiler`: emits `PushConst(namePoolIdx)` before args, then `CallFunc(slotIdx, argCount)`. `Function()` pops the name at the end.
   - `BuilderEmitMsil`: does NOT emit a push for the name. `CallFuncBySlot(slotIdx, argCount)` resolves the name from the slot directly. The `pendingFunctionNames` stack in `EmitAndCache` is just bookkeeping — nothing IL is emitted for `IDENTIFIER_FUNCTION`.

2. **`MsilCache` key is the `List<Token>` reference** (object identity, not content). Each `SourceLine.ParseBody`, `ParseSuccessGoto`, etc. is a distinct list object — this is safe.

3. **`DynamicMethod` owner = `typeof(Executive)`, `skipVisibility: true`** — required so the emitted IL can call `internal` methods on `Executive`. Do not change this.

4. **`OperatorFast` signature is `(OpCode op, int argumentCount)`** — the `OpCode` enum is cast to int in IL: `il.Emit(OpCodes.Ldc_I4, (int)opCode)`. This is what the draft already does correctly.

5. **`funcSlots` in `ThreadedExecuteLoop`** is declared but unused (`var funcSlots = Parent.FunctionSlots`). It was reserved for `CallFuncBySlot`. The `CallMsil` case uses `Parent.MsilDelegates` instead — `funcSlots` can stay or be removed.

6. **`ChoiceStart` as a method vs inline IL:** The threaded loop handles `ChoiceStart` inline (`if (Failure) { SystemStack.Pop(); Failure = false; }`). In MSIL we call a helper rather than emitting the field-access/branch/pop IL sequence, keeping the emitter simpler. Both are correct.

7. **`BuildEval` path:** `BuildEval` uses `CompileStarFunctions` only, not `tc.Compile()`, because it compiles a single expression as a star function. `EmitMsilForAllStatements` should still be called so the expression body gets a delegate.

---

## Files to Create / Modify

| File | Action |
|------|--------|
| `Snobol4.Common/Runtime/Execution/MsilHelpers.cs` | **Create** (Step 1) |
| `Snobol4.Common/Builder/BuilderEmitMsil.cs` | **Create** (Step 2) |
| `Snobol4.Common/Builder/Instruction.cs` | **Modify** — add `CallMsil = 90` to `OpCode` (Step 3) |
| `Snobol4.Common/Builder/ThreadedCodeCompiler.cs` | **Modify** — emit `CallMsil` (Step 3) |
| `Snobol4.Common/Builder/Builder.cs` | **Modify** — call `EmitMsilForAllStatements()`, add `MsilDelegates` list (Step 4) |
| `Snobol4.Common/Runtime/Execution/ThreadedExecuteLoop.cs` | **Modify** — handle `CallMsil` (Step 4) |
| `TestSnobol4/MsilEmitterTests.cs` | **Create** (Step 5) |

---

## Commit Strategy

Commit and push after every step. Small commits, all green. Merge back to
`feature/post-threaded-dev` with `--no-ff` when complete.

```bash
git add <files>
git commit -m "Step N: <description>"
git push
```

Remote URL with credentials:
`https://LCherryholmes:REDACTED_TOKEN@github.com/jcooper0/Snobol4.Net.git`
