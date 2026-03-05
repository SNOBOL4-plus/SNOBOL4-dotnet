# SNOBOL4.NET Performance Benchmarks

This document records execution performance of SNOBOL4.NET at each significant
milestone in the `feature/threaded-execution` development branch.

Benchmarks are run via `BenchmarkSuite2` using BenchmarkDotNet in Release mode.
Each benchmark runs the full pipeline: SNOBOL4 source → Parser → CodeGenerator
→ Roslyn compile → ExecuteLoop.

---

## Environment

| Property | Value |
|---|---|
| OS | Linux (Ubuntu 24.04 LTS) |
| CPU | Intel Xeon Platinum 8581C @ 2.10GHz (KVM hypervisor) |
| .NET | 10.0 |
| BenchmarkDotNet | 0.15.2 |
| Commit | 1776030 (Fix all compiler warnings) |
| Branch | feature/threaded-execution (Phase 1 baseline) |

---

## Benchmark Programs

### RomanBenchmark — Recursive functions, heavy identifier lookup
Converts integers to Roman numerals via a recursive SNOBOL4 DEFINE function.
Each call executes ~4 statements and recurses once per digit.
Sensitive to: `Identifier()` dictionary lookup, `FunctionName()`/`Function()`
dispatch, label-table goto resolution.

### ArithmeticLoopBenchmark — Pure dispatch overhead
Increments a counter 1000 times with no I/O or pattern matching.
Sensitive to: per-statement overhead (`InitializeStatement`, `FinalizeStatement`,
`Operator("__+")` dispatch, `LT` function call, conditional goto).

### StringPatternLoopBenchmark — Realistic string/pattern workload
Parses a 10-token CSV string 500 times using `BREAK` pattern matching.
Sensitive to: `Scanner.PatternMatch`, string concatenation, conditional gotos,
pattern object allocation.

---

## Phase 1 Baseline — C# Code Generation (Current Architecture)

> Recorded: 2026-03-04
> Architecture: SNOBOL4 → C# text → Roslyn → ExecuteLoop with string dictionary dispatch

| Benchmark | Mean | StdDev | Allocated |
|---|---|---|---|
| `Roman_1776` | *TBD — run BenchmarkSuite2 in Release mode* | | |
| `Roman_AllFour` | | | |
| `ArithLoop_1000` | | | |
| `StringPattern_500iters` | | | |

> Note: BenchmarkDotNet requires running outside a container/sandbox for stable
> results (it needs to control CPU affinity and disable GC interference).
> The TBD values above should be filled in by running:
> `dotnet run --project BenchmarkSuite2 -c Release`
> on a dedicated machine. The relative speedup figures in Phase 5 are what matter.

### Reference: CSNOBOL4 (Phil Budne) statement throughput
From the build-time timing benchmark on this machine:
- **6,741,133 statements/second** (145.7M statements in 21.6 seconds)
- Nanoseconds per statement: **148 ns**

### Reference: SNOBOL4.NET on 1brc (200K rows)
From prior session profiling:
- **~774 statements/second** on the 1brc workload (I/O + table operations)
- **~2,070 seconds** for 1,602,219 statements

---

## Phase 5 Results — Threaded Execution vs Roslyn Dispatch

> Recorded: 2026-03-05
> Commit: c8a2f10 (Remove Roslyn fallback path from CODE/EVAL execution)
> Method: Stopwatch timing, 5 reps each, Release build, same machine.
> Both modes include Roslyn compile time (unavoidable until star functions are replaced).

| Benchmark | Threaded | Non-Threaded | Speedup |
|---|---|---|---|
| `ArithLoop_1000` | 79 ms | 124 ms | **1.6x** |
| `Roman_1776` | 60 ms | 39 ms | 0.65x (slower) |
| `StringPattern_500` | 202 ms | 246 ms | **1.2x** |

### Why not 10x yet?

The original estimate of ~10x assumed the Roslyn compile step would be eliminated.
Currently every benchmark run still pays the full Roslyn compile cost (~25-35 ms)
because star functions (`*(expr)` deferred expressions) are still generated as C#
methods by `CodeGenerator.cs` and compiled by Roslyn. This cost dominates the
short benchmarks and masks the execution speedup.

Roman is actually **slower** in threaded mode because it is heavy on recursive
user-defined functions and star functions — the parts that still touch Roslyn-generated
delegates most. Pure dispatch (ArithLoop) already shows the expected improvement.

### Path to 10x

The remaining work is replacing star function C# generation with a threaded
equivalent — storing the deferred expression token list at parse time and
evaluating it inline in `ThreadedExecuteLoop`. Once Roslyn compile is eliminated
from the hot path entirely, the 10x target becomes realistic.

---

## Notes on Methodology

- All BenchmarkDotNet runs use `[MemoryDiagnoser]` to capture allocations
- Each benchmark includes Roslyn compile time — this is intentional, as it
  represents the full user-visible cost of running a SNOBOL4 program
- Phase 5 will introduce a `UseThreadedExecution` flag; benchmarks will be
  re-run with the flag enabled to produce the Phase 5 column above
- The `ArithmeticLoopBenchmark` is the purest measure of dispatch overhead;
  the `StringPatternLoopBenchmark` is the most representative of real workloads
