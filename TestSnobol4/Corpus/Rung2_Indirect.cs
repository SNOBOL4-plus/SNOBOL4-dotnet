using Snobol4.Common;
using Test.TestLexer;

// ReSharper disable StringLiteralTypo

namespace Test.Corpus;

/// <summary>
/// Corpus rung 2 — indirect variable reference via $ operator.
/// Programs use PASS/FAIL output convention; we assert last output line starts with PASS.
/// </summary>
[DoNotParallelize]
[TestClass]
public class Rung2_Indirect
{
    /// <summary>
    /// Runs a script and returns PASS/FAIL lines written to OUTPUT (Console.Error in DOTNET).
    /// Uses a lock to be safe under the assembly-level parallel test runner.
    /// </summary>
    private static readonly object s_consoleLock = new();

    private static List<string> RunGetOutput(string script)
    {
        var lines = new List<string>();
        lock (s_consoleLock)
        {
            var old = Console.Error;
            using var ms = new System.IO.MemoryStream();
            using var sw = new System.IO.StreamWriter(ms) { AutoFlush = true };
            Console.SetError(sw);
            try { SetupTests.SetupScript("-b", script); }
            finally { Console.SetError(old); }
            ms.Position = 0;
            using var sr = new System.IO.StreamReader(ms);
            foreach (var line in sr.ReadToEnd().Split('\n'))
            {
                var t = line.Trim();
                if (t.StartsWith("PASS") || t.StartsWith("FAIL")) lines.Add(t);
            }
        }
        return lines;
    }

    [TestMethod]
    public void TEST_Corpus_210_indirect_ref()
    {
        var s = @"
        bal = 'the real bal'
        differ($'bal', bal)                   :f(e001)
        output = 'FAIL 210/001: $string lookup'        :(end)
e001
        differ($.bal, bal)                   :f(e002)
        output = 'FAIL 210/002: $.var lookup'          :(end)
e002
        output = 'PASS 210_indirect_ref (2/2)'
end";
        var lines = RunGetOutput(s);
        Assert.IsTrue(lines.Count > 0);
        Assert.IsTrue(lines[^1].StartsWith("PASS"), $"Expected PASS, got: {lines[^1]}");
    }

    [TestMethod]
    public void TEST_Corpus_211_indirect_assign()
    {
        var s = @"
        $'qq' = 'x'
        differ(qq, 'x')                   :f(e001)
        output = 'FAIL 211/001: indirect assign sets named var' :(end)
e001
        differ($'_no_such_var_')                   :f(e002)
        output = 'FAIL 211/002: undefined indirect is null'     :(end)
e002
        output = 'PASS 211_indirect_assign (2/2)'
end";
        var lines = RunGetOutput(s);
        Assert.IsTrue(lines.Count > 0);
        Assert.IsTrue(lines[^1].StartsWith("PASS"), $"Expected PASS, got: {lines[^1]}");
    }

    [TestMethod]
    public void TEST_Corpus_212_indirect_array()
    {
        var s = @"
        a = array(3)
        a<2> = 'x'
        differ($.a<2>, 'x')                   :f(e001)
        output = 'FAIL 212/001: $.var<index> indirect array' :(end)
e001
        output = 'PASS 212_indirect_array (1/1)'
end";
        var lines = RunGetOutput(s);
        Assert.IsTrue(lines.Count > 0);
        Assert.IsTrue(lines[^1].StartsWith("PASS"), $"Expected PASS, got: {lines[^1]}");
    }
}
