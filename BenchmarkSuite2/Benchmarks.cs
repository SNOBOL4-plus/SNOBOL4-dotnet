namespace Snobol4.Benchmarks2;

/// <summary>
/// SNOBOL4 benchmark scripts. Each constant is a complete SNOBOL4 program
/// used by Program.cs to measure performance via BenchmarkHelper.Measure().
///
/// Five workload profiles:
///   Roman          — recursive functions, heavy identifier/function dispatch
///   ArithLoop      — pure interpreter dispatch, tight counter loop (5000 iters)
///   StringPattern  — realistic CSV parsing via BREAK pattern (1000 iters)
///   Fibonacci      — deep recursion, FIB(20) ~21k recursive calls
///   StringManip    — REPLACE/SIZE/SUBSTR string operations (2000 iters)
/// </summary>
public static class Scripts
{
    // -------------------------------------------------------------------------
    // Roman — recursive function, heavy identifier lookup and function dispatch.
    // ROMAN('1776') recurses 4 levels, exercising DEFINE, RPOS, LEN, BREAK,
    // REPLACE, label-table gotos. Most sensitive to Roslyn compile overhead
    // and function dispatch cost.
    // -------------------------------------------------------------------------
    public const string Roman = @"
    DEFINE('ROMAN(N)T')                 :(ROMAN_END)
ROMAN   N   RPOS(1)  LEN(1) . T  =         :F(RETURN)
    '0,1I,2II,3III,4IV,5V,6VI,7VII,8VIII,9IX,'
+   T   BREAK(',') . T                  :F(FRETURN)
    ROMAN = REPLACE(ROMAN(N), 'IVXLCDM', 'XLCDM**') T
+                                       :S(RETURN)F(FRETURN)
ROMAN_END
    R1 = ROMAN('1776')
    R2 = ROMAN('9')
    R3 = ROMAN('400')
    R4 = ROMAN('2026')
end";

    // -------------------------------------------------------------------------
    // ArithLoop — tight counter loop, pure interpreter dispatch overhead.
    // Increments N from 0 to 5000 with no I/O or pattern matching.
    // Most sensitive to per-statement overhead, arithmetic operators, goto.
    // -------------------------------------------------------------------------
    public const string ArithLoop = @"
    &TRIM = 1
    N = 0
LOOP    N = N + 1
    N = LT(N, 5000) N          :S(LOOP)
    RESULT = N
end";

    // -------------------------------------------------------------------------
    // StringPattern — realistic CSV parsing workload.
    // Parses a 10-token comma-separated string 1000 times using BREAK pattern.
    // Most sensitive to pattern matching, string concatenation, allocation.
    // -------------------------------------------------------------------------
    public const string StringPattern = @"
    &TRIM = 1
    PAT = BREAK(',') . WORD ','
    ITER = 0
OUTER   ITER = LT(ITER, 1000) ITER + 1      :F(DONE)
    S = 'alpha,beta,gamma,delta,epsilon,zeta,eta,theta,iota,kappa,'
    RESULT = ''
INNER   S PAT = ''                          :F(OUTER)
    RESULT = RESULT WORD               :(INNER)
DONE    FINAL = RESULT
end";

    // -------------------------------------------------------------------------
    // Fibonacci — deep recursion stress test.
    // FIB(20) = 6765, requires ~21,891 recursive SNOBOL4 function calls.
    // Most sensitive to function call/return overhead and stack management.
    // -------------------------------------------------------------------------
    public const string Fibonacci = @"
    DEFINE('FIB(N)')                        :(FIB_END)
FIB     FIB = LT(N,2) N                    :S(RETURN)
    FIB = FIB(N - 1) + FIB(N - 2)     :(RETURN)
FIB_END
    RESULT = FIB(20)
end";

    // -------------------------------------------------------------------------
    // StringManip — string operation throughput.
    // Runs REPLACE, SIZE, and SUBSTR on a sentence string 2000 times.
    // Most sensitive to string function dispatch and string allocation.
    // -------------------------------------------------------------------------
    public const string StringManip = @"
    &TRIM = 1
    ITER = 0
LOOP    ITER = LT(ITER, 2000) ITER + 1      :F(DONE)
    S = 'The quick brown fox jumps over the lazy dog'
    S = REPLACE(S, 'aeiou', '*****')
    N = SIZE(S)
    S2 = SUBSTR(S, 1, 10) SUBSTR(S, 11, 10)
DONE    RESULT = N
end";
}
