using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Snobol4.Common;

namespace Snobol4.Benchmarks2;

/// <summary>
/// Runs a SNOBOL4 script through the full Builder pipeline and returns the
/// resulting Builder (which holds the Executive and IdentifierTable).
/// </summary>
public static class BenchmarkHelper
{
    public static Builder RunScript(string script)
    {
        Builder builder = new();
        builder.ParseCommandLine(new[] { "-b", Path.Combine(Path.GetTempPath(), "bench.sno") });
        builder.Code.ReadTestScript(new MemoryStream(Encoding.UTF8.GetBytes(script)));
        builder.BuildMain();
        return builder;
    }

    /// <summary>
    /// Run a benchmark: warmup runs, then timed reps.
    /// Returns (mean ms, stddev ms, mean bytes allocated per run).
    /// </summary>
    public static (double mean, double stddev, long allocBytes) Measure(
        string script, int reps = 20, int warmup = 3)
    {
        for (int i = 0; i < warmup; i++) RunScript(script);

        var times = new List<long>(reps);
        long totalAlloc = 0;

        for (int i = 0; i < reps; i++)
        {
            long before = GC.GetTotalAllocatedBytes(precise: false);
            var sw = Stopwatch.StartNew();
            RunScript(script);
            sw.Stop();
            long after = GC.GetTotalAllocatedBytes(precise: false);
            times.Add(sw.ElapsedMilliseconds);
            totalAlloc += after - before;
        }

        double mean   = times.Average();
        double stddev = Math.Sqrt(times.Average(t => (t - mean) * (t - mean)));
        return (mean, stddev, totalAlloc / reps);
    }
}
