using Test.TestLexer;

// ReSharper disable StringLiteralTypo

namespace Test.Corpus;

[DoNotParallelize]
[TestClass]
public class Rung9_TypesPredicates
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
    public void TEST_Corpus_910_convert()
    {
        var s = @"
        differ(convert('12', 'integer'), 12)                   :f(e001)
        output = 'FAIL 910/001: string->integer'        :(end)
e001
        differ(convert(2.5, 'integer'), 2)                   :f(e002)
        output = 'FAIL 910/002: real->integer truncation' :(end)
e002
        differ(convert(2, 'real'), 2.0)                   :f(e003)
        output = 'FAIL 910/003: integer->real'          :(end)
e003
        differ(convert('.2', 'real'), 0.2)                   :f(e004)
        output = 'FAIL 910/004: string->real'           :(end)
e004
        output = 'PASS 910_convert (4/4)'
end";
        var lines = RunGetOutput(s);
        Assert.IsTrue(lines.Count > 0);
        Assert.IsTrue(lines[^1].StartsWith("PASS"), $"Expected PASS, got: {lines[^1]}");
    }

    [TestMethod]
    public void TEST_Corpus_911_datatype()
    {
        // DOTNET DATATYPE returns lowercase ('string', 'integer', 'real')
        // Adjust corpus program to use lowercase comparisons
        var s = @"
        differ(datatype('hello'), 'string')                   :f(e001)
        output = 'FAIL 911/001: string literal datatype' :(end)
e001
        differ(datatype(12), 'integer')                   :f(e002)
        output = 'FAIL 911/002: integer datatype'       :(end)
e002
        differ(datatype(1.33), 'real')                   :f(e003)
        output = 'FAIL 911/003: real datatype'          :(end)
e003
        differ(datatype(''), 'string')                   :f(e004)
        output = 'FAIL 911/004: null is string'         :(end)
e004
        output = 'PASS 911_datatype (4/4)'
end";
        var lines = RunGetOutput(s);
        Assert.IsTrue(lines.Count > 0);
        Assert.IsTrue(lines[^1].StartsWith("PASS"), $"Expected PASS, got: {lines[^1]}");
    }

    [TestMethod]
    public void TEST_Corpus_912_num_pred()
    {
        var s = @"
        lt(5, 4)                   :f(e001)
        output = 'FAIL 912/001: lt(5,4) should fail'   :(end)
e001    lt(4, 4)                   :f(e002)
        output = 'FAIL 912/002: lt(4,4) should fail'   :(end)
e002    lt(4, 5)                   :s(e003)
        output = 'FAIL 912/003: lt(4,5) should succeed' :(end)
e003
        le(5, 2)                   :f(e004)
        output = 'FAIL 912/004: le(5,2) should fail'   :(end)
e004    le(4, 4)                   :s(e005)
        output = 'FAIL 912/005: le(4,4) should succeed' :(end)
e005    le(4, 10)                  :s(e006)
        output = 'FAIL 912/006: le(4,10) should succeed' :(end)
e006
        eq(4, 5)                   :f(e007)
        output = 'FAIL 912/007: eq(4,5) should fail'   :(end)
e007    eq(5, 5)                   :s(e008)
        output = 'FAIL 912/008: eq(5,5) should succeed' :(end)
e008
        ne(4, 4)                   :f(e009)
        output = 'FAIL 912/009: ne(4,4) should fail'   :(end)
e009    ne(4, 6)                   :s(e010)
        output = 'FAIL 912/010: ne(4,6) should succeed' :(end)
e010
        gt(4, 6)                   :f(e011)
        output = 'FAIL 912/011: gt(4,6) should fail'   :(end)
e011    gt(4, 4)                   :f(e012)
        output = 'FAIL 912/012: gt(4,4) should fail'   :(end)
e012    gt(5, 2)                   :s(e013)
        output = 'FAIL 912/013: gt(5,2) should succeed' :(end)
e013
        ge(5, 7)                   :f(e014)
        output = 'FAIL 912/014: ge(5,7) should fail'   :(end)
e014    ge(4, 4)                   :s(e015)
        output = 'FAIL 912/015: ge(4,4) should succeed' :(end)
e015    ge(7, 5)                   :s(e016)
        output = 'FAIL 912/016: ge(7,5) should succeed' :(end)
e016
        ne(4, 5 - 1)               :f(e017)
        output = 'FAIL 912/017: ne(4,5-1) should fail (both=4)' :(end)
e017
        ne('12', 12)                   :f(e018)
        output = 'FAIL 912/018: ne(string,int) type mismatch' :(end)
e018
        output = 'PASS 912_num_pred (18/18)'
end";
        var lines = RunGetOutput(s);
        Assert.IsTrue(lines.Count > 0);
        Assert.IsTrue(lines[^1].StartsWith("PASS"), $"Expected PASS, got: {lines[^1]}");
    }

    [TestMethod]
    public void TEST_Corpus_913_integer_pred()
    {
        var s = @"
        integer('abc')             :f(e001)
        output = 'FAIL 913/001: integer(string) should fail' :(end)
e001
        integer(12)                :s(e002)
        output = 'FAIL 913/002: integer(12) should succeed'  :(end)
e002
        integer('12')              :s(e003)
        output = 'FAIL 913/003: integer(numeric-string) should succeed' :(end)
e003
        output = 'PASS 913_integer_pred (3/3)'
end";
        var lines = RunGetOutput(s);
        Assert.IsTrue(lines.Count > 0);
        Assert.IsTrue(lines[^1].StartsWith("PASS"), $"Expected PASS, got: {lines[^1]}");
    }

    [TestMethod]
    public void TEST_Corpus_914_lgt()
    {
        var s = @"
        lgt('abc', 'xyz')          :f(e001)
        output = 'FAIL 914/001: lgt(abc,xyz) should fail'   :(end)
e001
        lgt('abc', 'abc')          :f(e002)
        output = 'FAIL 914/002: lgt(abc,abc) should fail'   :(end)
e002
        lgt('xyz', 'abc')          :s(e003)
        output = 'FAIL 914/003: lgt(xyz,abc) should succeed' :(end)
e003
        lgt('', 'abc')             :f(e004)
        output = 'FAIL 914/004: lgt(null,abc) should fail'  :(end)
e004
        lgt('abc', '')             :s(e005)
        output = 'FAIL 914/005: lgt(abc,null) should succeed' :(end)
e005
        output = 'PASS 914_lgt (5/5)'
end";
        var lines = RunGetOutput(s);
        Assert.IsTrue(lines.Count > 0);
        Assert.IsTrue(lines[^1].StartsWith("PASS"), $"Expected PASS, got: {lines[^1]}");
    }
}
