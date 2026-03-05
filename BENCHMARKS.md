# SNOBOL4.NET Performance Benchmarks

This document records execution performance of SNOBOL4.NET at each significant
milestone in the `feature/threaded-execution` development branch.

Benchmarks are run using a Stopwatch harness (20 reps, 3 warmup runs) in Release
mode. Each benchmark run includes the full pipeline: parse → compile → execute.

---

## Environment

| Property | Value |
|---|---|
| OS | Linux (Ubuntu 24.04 LTS) |
| CPU | Intel Xeon Platinum 8581C @ 2.10GHz (KVM hypervisor, 2 cores) |
| .NET | 10.0 |
| Branch | `feature/threaded-execution` vs `master` |

---

## Benchmark Programs

### Roman — Recursive functions, heavy identifier lookup
Converts integers to Roman numerals via a recursive SNOBOL4 `DEFINE` function.
Each call executes ~4 statements and recurses once per digit.
Most sensitive to: function dispatch, identifier lookup, label-table goto resolution.

### ArithLoop_5000 — Pure dispatch overhead
Increments a counter 5000 times with no I/O or pattern matching.
Most sensitive to: per-statement dispatch, arithmetic operators, conditional goto.

### StringPattern_1000 — Realistic string/pattern workload
Parses a 10-token CSV string 1000 times using `BREAK` pattern matching.
Most sensitive to: pattern matching, string concatenation, conditional gotos.

### Fibonacci_20 — Deep recursion
Computes Fibonacci(20) recursively (~21,891 recursive calls).
Most sensitive to: function call/return overhead, stack management.

### StringManip_2000 — String operations
Performs `REPLACE`, `SIZE`, and `SUBSTR` operations 2000 times.
Most sensitive to: string function dispatch, string allocation.

---

## Results: master vs feature/threaded-execution

> Recorded: 2026-03-05  
> master commit: `1776030` (Roslyn C# code generation path)  
> feature commit: `46a9205` (Threaded execution, Roslyn fully removed)  
> Method: Stopwatch, 20 reps, 3 warmup runs, Release build

| Benchmark | master (Roslyn) | feature (Threaded) | Speedup | Alloc master | Alloc feature | Alloc saved |
|---|---|---|---|---|---|---|
| `Roman_1776` | 124.2 ms | 7.8 ms | **15.9x** | 1,537 KB | 435 KB | **3.5x less** |
| `ArithLoop_5000` | 160.8 ms | 122.5 ms | **1.3x** | 10,620 KB | 8,311 KB | 1.3x less |
| `StringPattern_1000` | 416.7 ms | 331.2 ms | **1.3x** | 34,310 KB | 30,555 KB | 1.1x less |
| `Fibonacci_20` | 593.4 ms | 510.6 ms | **1.2x** | 50,931 KB | 45,375 KB | 1.1x less |
| `StringManip_2000` | 35.8 ms | 2.4 ms | **14.9x** | 1,164 KB | 236 KB | **4.9x less** |

### Analysis

**Where threaded execution wins big (10–16x):**
Roman and StringManip are short-running programs dominated by Roslyn compile
overhead (~25–100 ms per run in master). With Roslyn fully removed, these
programs now spend nearly all their time in actual execution rather than
C# compilation. This is the primary win of the threaded architecture.

**Where gains are modest (1.2–1.3x):**
ArithLoop, StringPattern, and Fibonacci run long enough that Roslyn overhead
is a smaller fraction of total time. The remaining gap reflects the overhead
of interpreter dispatch vs. direct C# execution. These benchmarks allocate
heavily due to string operations and pattern matching — areas not yet optimized.

**Allocation improvements:**
Memory allocation is reduced across all benchmarks. Roman and StringManip
show the biggest reductions (3.5x and 4.9x) because Roslyn itself allocates
substantial memory compiling syntax trees. Long-running benchmarks show
smaller allocation improvements since string/pattern operations dominate.

---

## Reference: CSNOBOL4 (Phil Budne) statement throughput

From the build-time timing benchmark on this machine:
- **6,741,133 statements/second** (145.7M statements in 21.6 seconds)
- Nanoseconds per statement: **148 ns**

SNOBOL4.NET on ArithLoop_5000 (5000 statements, 122.5ms):
- **~40,800 statements/second** — approximately **165x slower than CSNOBOL4**

---

## Phase 1 Baseline — C# Code Generation (master)

> Architecture: SNOBOL4 → C# text → Roslyn → ExecuteLoop with string dictionary dispatch

The master branch generates C# source code from SNOBOL4 at runtime, compiles it
with Roslyn, loads the resulting DLL, and executes it. Roslyn compile time
(25–100ms per program) dominates all short-to-medium length programs.

---

## Phase 7 — Threaded Execution (feature/threaded-execution)

> Architecture: SNOBOL4 → Instruction[] → ThreadedExecuteLoop

The feature branch replaces Roslyn code generation entirely with a threaded
interpreter. A `ThreadedCodeCompiler` transforms the parsed AST into a flat
`Instruction[]` array, and `ThreadedExecuteLoop` dispatches opcodes in a tight
`switch` loop. No C# is generated, no Roslyn DLL is compiled or loaded.

### What was done
- Phase 1–4: Build threaded execution infrastructure (opcodes, compiler, loop)
- Phase 5: Replace main program execution with threaded path
- Phase 6: Replace star functions (deferred expressions) with threaded path
- Phase 7: Remove Roslyn entirely from the live execution path

### Path to further improvement

The remaining performance gap vs CSNOBOL4 (~165x) is in the interpreter loop
itself. The primary bottlenecks (in order of expected impact):

1. **Identifier lookup** — `PushVar` currently does `IdentifierTable[symbol]`
   (string dictionary lookup) on every variable access. Since slot indices are
   known at compile time, this could become a direct `vars[slot]` array access.

2. **Function dispatch** — `Operator("__+")` does a string dictionary lookup
   on every arithmetic/comparison operation. Pre-resolving to delegates at
   compile time would eliminate this.

3. **String allocation** — Pattern matching and string operations allocate
   new objects on every call. Interning or pooling common strings would reduce GC pressure.
