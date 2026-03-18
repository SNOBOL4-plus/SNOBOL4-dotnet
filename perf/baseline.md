# DOTNET Benchmark Baseline — session154

**Date:** 2026-03-17  
**HEAD:** d8f11f9  
**Build:** Release, net10.0, `-p:EnableWindowsTargeting=true`  
**Runner:** BenchmarkSuite2 — 5 reps, 1 warmup, full Builder pipeline (lex→parse→emit→compile→execute)  
**Platform:** Linux x64 container  

## Results

```
Benchmark                        Mean  ±   StdDev    Alloc/run    Result
--------------------------------------------------------------------------
Roman_1776                      23.4ms ±   22.2ms       438 KB   (MDCCLXXVI)
ArithLoop_1000                  41.6ms ±   24.5ms      1662 KB   (1000)
StringPattern_200               92.0ms ±   33.6ms      5699 KB   (alphabeta...)
Fibonacci_18                   237.4ms ±   32.5ms     11853 KB   (2584)
StringManip_500                 44.2ms ±   25.1ms      2707 KB   (43)
FuncCallOverhead_3000           19.0ms ±   18.4ms       877 KB   (300)
StringConcat_500                18.8ms ±   25.6ms       394 KB   (100)

--- Bottleneck isolation ---
VarAccess_2000                  98.0ms ±   33.2ms      6282 KB   (12012)
OperatorDispatch_100            21.0ms ±   27.0ms       611 KB   (165116)
PatternBacktrack_500            59.0ms ±   35.6ms      1971 KB   (500)
TableAccess_500                 33.6ms ±   21.9ms      1623 KB   (250500)
MixedWorkload_200              220.6ms ±   20.5ms     13930 KB   (550)

--- CODE() and EVAL() runtime compile ---
CodeFixed_200                   65.8ms ±   25.0ms      6125 KB   (200)
CodeDynamic_200                 89.4ms ±   54.9ms      6432 KB   (200)
EvalFixed_200                   54.6ms ±   30.2ms      5935 KB   (11)
EvalDynamic_200                 39.4ms ±   14.5ms      6134 KB   (400)
IndirectDispatch_500             5.0ms ±    0.0ms       264 KB   ()  ← IndirectDispatch error 22 (known gap)
```

## Notes

- StdDev is high (JIT warmup + container noise) — single warmup run is insufficient for stable numbers.
  Use `--reps 20 --warmup 5` or BenchmarkDotNet for publication-quality numbers.
- `IndirectDispatch_500`: error 22 on `$FN(X)` — indirect dispatch via EVAL not fully wired; result empty.
- `Fibonacci_18` is the heaviest: 237ms / 11 MB alloc — deep recursion stress, most sensitive to call overhead.
- `VarAccess_2000`: 98ms / 6 MB — highest alloc per iteration ratio; likely `Var.Convert` boxing.
- `MixedWorkload_200`: 220ms / 14 MB — combined overhead; good regression canary.

## Hotfix candidates (per net-perf-analysis plan)

1. `Var.Convert` fast-path (VarAccess/MixedWorkload alloc)
2. FunctionTable fold+lookup (Fibonacci call overhead)
3. SystemStack List pressure (MixedWorkload alloc)
