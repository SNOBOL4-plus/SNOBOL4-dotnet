using Test.TestLexer;

// ReSharper disable StringLiteralTypo

namespace Test.Corpus;

[DoNotParallelize]
[TestClass]
public class Rung8_StringOps
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
    public void TEST_Corpus_810_replace()
    {
        var s = @"
        differ(replace('axxbyyy', 'xy', '01'), 'a00b111')                   :f(e001)
        output = 'FAIL 810/001: xy->01 mapping'             :(end)
e001
        a = replace(&alphabet, 'xy', 'ab')
        differ(replace('axy', &alphabet, a), 'aab')                   :f(e002)
        output = 'FAIL 810/002: alphabet translation'       :(end)
e002
        differ(replace('hello', 'aeiou', 'aeiou'), 'hello')                   :f(e003)
        output = 'FAIL 810/003: identity replace'           :(end)
e003
        output = 'PASS 810_replace (3/3)'
end";
        var lines = RunGetOutput(s);
        Assert.IsTrue(lines.Count > 0);
        Assert.IsTrue(lines[^1].StartsWith("PASS"), $"Expected PASS, got: {lines[^1]}");
    }

    [TestMethod]
    public void TEST_Corpus_811_size()
    {
        var s = @"
        differ(size('abc'), 3)                   :f(e001)
        output = 'FAIL 811/001: size of 3-char string'  :(end)
e001
        differ(size(12), 2)                   :f(e002)
        output = 'FAIL 811/002: size of integer 12 = 2 digits' :(end)
e002
        differ(size(''), 0)                   :f(e003)
        output = 'FAIL 811/003: size of null = 0'       :(end)
e003
        output = 'PASS 811_size (3/3)'
end";
        var lines = RunGetOutput(s);
        Assert.IsTrue(lines.Count > 0);
        Assert.IsTrue(lines[^1].StartsWith("PASS"), $"Expected PASS, got: {lines[^1]}");
    }

    [TestMethod]
    public void TEST_Corpus_812_dupl()
    {
        var s = @"
        differ(dupl('abc', 2), 'abcabc')                   :f(e001)
        output = 'FAIL 812/001: dupl string x2'         :(end)
e001
        differ(dupl('', 10), '')                   :f(e002)
        output = 'FAIL 812/002: dupl null is null'      :(end)
e002
        differ(dupl('abcdefg', 0), '')                   :f(e003)
        output = 'FAIL 812/003: dupl x0 is null'        :(end)
e003
        differ(dupl(1, 10), '1111111111')                   :f(e004)
        output = 'FAIL 812/004: dupl integer coerce'    :(end)
e004
        output = 'PASS 812_dupl (4/4)'
end";
        var lines = RunGetOutput(s);
        Assert.IsTrue(lines.Count > 0);
        Assert.IsTrue(lines[^1].StartsWith("PASS"), $"Expected PASS, got: {lines[^1]}");
    }
}
