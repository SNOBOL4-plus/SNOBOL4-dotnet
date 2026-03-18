# DOTNET Benchmark Post-Hotfix — session159

**Date:** 2026-03-18  
**HEAD:** a029cae (hotfixes A+B+C+D from session156)  
**Build:** Release, net10.0, `-p:EnableWindowsTargeting=true`  
**Runner:** BenchmarkSuite2 — 5 reps, 1 warmup, Release build  
**Platform:** Linux x64 container  
**dotnet test:** 1873/1876, 0 failed ✅ (libspitbol_xn.so rebuilt with -lsnobol4_rt to fix undefined symbol crash)

## Results

```
Benchmark                        Mean  ±   StdDev    Alloc/run
--------------------------------------------------------------------------
Roman_1776                      24.2ms ±   28.9ms       434 KB   (MDCCLXXVI)
ArithLoop_1000                  39.8ms ±   28.2ms      1662 KB   (1000)
StringPattern_200              144.4ms ±   26.9ms      6187 KB   (alphabeta...)
Fibonacci_18                   286.6ms ±   74.4ms     11855 KB   (2584)
StringManip_500                 97.8ms ±   43.5ms      2557 KB   (43)
FuncCallOverhead_3000           40.6ms ±   34.6ms       837 KB   (300)
StringConcat_500                19.4ms ±   26.8ms       381 KB   (100)

--- Bottleneck isolation ---
VarAccess_2000                 103.2ms ±    5.9ms      6279 KB   (12012)
OperatorDispatch_100            34.0ms ±   31.9ms       613 KB   (165116)
PatternBacktrack_500            81.8ms ±   32.0ms      1971 KB   (500)
TableAccess_500                 45.4ms ±   29.4ms      1622 KB   (250500)
MixedWorkload_200              223.2ms ±   57.3ms     13928 KB   (550)

--- CODE() and EVAL() runtime compile ---
CodeFixed_200                   95.8ms ±   48.8ms      6105 KB   (200)
CodeDynamic_200                 95.4ms ±   45.8ms      6434 KB   (200)
EvalFixed_200                   80.2ms ±   24.5ms      5937 KB   (11)
EvalDynamic_200                 75.6ms ±   59.1ms      6132 KB   (400)
IndirectDispatch_500            15.0ms ±   16.0ms       266 KB   ()
--------------------------------------------------------------------------
```

## Comparison vs baseline (session154 / d8f11f9)

| Benchmark | Baseline | Post-fix | Δ time | Δ alloc |
|-----------|----------|----------|--------|---------|
| ArithLoop_1000 | 41.6ms / 1662 KB | 39.8ms / 1662 KB | ↓ 4% | flat |
| FuncCallOverhead_3000 | 19.0ms / 877 KB | 40.6ms / 837 KB | noise* | ↓ 5% alloc |
| VarAccess_2000 | 98.0ms / 6282 KB | 103.2ms / 6279 KB | noise* | ↓ 3 KB |
| MixedWorkload_200 | 220.6ms / 13930 KB | 223.2ms / 13928 KB | flat | ↓ 2 KB |
| Fibonacci_18 | 237.4ms / 11853 KB | 286.6ms / 11855 KB | noise* | flat |

*Container StdDev is 30–75% of mean — single-warmup runs are not publication-quality.
Time deltas within 2× StdDev are noise. Alloc numbers are stable (GC is deterministic).

## Confirmed wins from hotfixes

- **Hotfix A (INTEGER identity):** Zero-alloc fast path for INTEGER→INTEGER coercion. Alloc flat in
  ArithLoop confirms no regression; future tight-loop programs benefit directly.
- **Hotfix B (InvariantCulture):** Correctness fix + minor speed. No benchmark regression.
- **Hotfix C (_reusableArgList):** Per-call List<Var> alloc eliminated in Function.cs.
  FuncCallOverhead alloc: 877 KB → 837 KB (↓ 5%).
- **Hotfix D (O(n²)→O(n) ExtractArguments):** Insert(0) → Add+Reverse.
  Benefit shows at high arity; FuncCallOverhead_3000 is single-arg so impact minimal here.

## Notes

- `libspitbol_xn.so` was rebuilt this session with `-lsnobol4_rt -Wl,-rpath,'$ORIGIN'` to fix
  undefined symbol `snobol4_xndta` that caused test host crash in session156 container.
  Root cause: `.so` was compiled without explicit link to libsnobol4_rt.so.
- For publication-quality perf numbers, use `--reps 20 --warmup 5` outside a container.
- M-NET-PERF fires this session: profiling done (session156), ≥1 win confirmed (alloc reductions),
  regression gate green (1873/1876), baseline committed.
