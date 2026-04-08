using Test.TestLexer;

// ReSharper disable StringLiteralTypo

namespace Test.Corpus;

/// <summary>
/// Array/TABLE/DATA tests — new coverage only (091-096 already in SimpleOutput_FunctionsData.cs).
/// New: default values, missing keys, integer keys, two types, linked list, 2D array, PROTOTYPE.
/// </summary>
[DoNotParallelize]
[TestClass]
public class CorpusRef_Data
{
    // ── Additional array/table/data building blocks ──────────────────────────

    [TestMethod]
    public void TEST_Corpus_data_array_default_value()
    {
        // Unset array elements return empty string
        var s = @"
        A = ARRAY(3)
        A<2> = 'set'
        OUTPUT = IDENT(A<1>) 'empty'
        OUTPUT = A<2>
END";
        Assert.AreEqual("empty\nset", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_data_table_missing_key()
    {
        // Unset table key returns empty string
        var s = @"
        T = TABLE()
        T<'x'> = 'found'
        OUTPUT = IDENT(T<'missing'>) 'empty'
        OUTPUT = T<'x'>
END";
        Assert.AreEqual("empty\nfound", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_data_table_integer_key()
    {
        // TABLE keys can be integers
        var s = @"
        T = TABLE()
        T<1> = 'one'
        T<2> = 'two'
        T<3> = 'three'
        OUTPUT = T<1>
        OUTPUT = T<2>
        OUTPUT = T<3>
END";
        Assert.AreEqual("one\ntwo\nthree", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_data_two_types()
    {
        // Two DATA types coexist; DATATYPE distinguishes them
        var s = @"
        DATA('cat(name,lives)')
        DATA('dog(name,tricks)')
        C = cat('Felix', 9)
        D = dog('Rex', 3)
        OUTPUT = REPLACE(DATATYPE(C), &LCASE, &UCASE)
        OUTPUT = REPLACE(DATATYPE(D), &LCASE, &UCASE)
        OUTPUT = name(C)
        OUTPUT = name(D)
END";
        Assert.AreEqual("CAT\nDOG\nFelix\nRex", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_data_linked_list()
    {
        // Classic linked list via DATA
        var s = @"
        DATA('node(val,nxt)')
        HEAD =
        HEAD = node(3, HEAD)
        HEAD = node(2, HEAD)
        HEAD = node(1, HEAD)
        P = HEAD
LOOP    IDENT(P)                                   :S(END)
        OUTPUT = val(P)
        P = nxt(P)                                 :(LOOP)
END";
        Assert.AreEqual("1\n2\n3", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_data_array_2d()
    {
        // 2D array: ARRAY('3,3')
        var s = @"
        A = ARRAY('3,3')
        A<1,1> = 'a'
        A<2,2> = 'b'
        A<3,3> = 'c'
        OUTPUT = A<1,1>
        OUTPUT = A<2,2>
        OUTPUT = A<3,3>
END";
        Assert.AreEqual("a\nb\nc", SetupTests.RunWithInput(s));
    }

    [TestMethod]
    public void TEST_Corpus_data_array_size_prototype()
    {
        // PROTOTYPE returns array dimensions as string
        var s = @"
        A = ARRAY(5)
        OUTPUT = PROTOTYPE(A)
END";
        Assert.AreEqual("5", SetupTests.RunWithInput(s));
    }
}
