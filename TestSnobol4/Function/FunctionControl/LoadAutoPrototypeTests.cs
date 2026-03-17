using Snobol4.Common;
using Test.TestLexer;

namespace Test.FunctionControl;

/// <summary>
/// Tests for net-load-dotnet Steps 2 and 3:
///   Step 2 — Auto-prototype: LOAD('dll', 'Ns.Class') reflects the single
///             public method and registers it by method name.
///   Step 3 — Explicit binding: LOAD('dll', 'Ns.Class::Method') picks a
///             named method from a multi-method class.
/// ReflectLibrary has no IExternalLibrary — all registration is via reflection.
/// </summary>
[TestClass]
public class LoadAutoPrototypeTests
{
    private static Builder Run(string script) =>
        SetupTests.SetupScript("-b", script);

    private static string Str(string name, Builder b) =>
        b.Execute!.IdentifierTable[b.FoldCase(name)].ToString();

    private static string Dll => SetupTests.ReflectLibraryPath;

    // ── Step 2: auto-prototype — single public method discovered ─────────

    [TestMethod]
    public void AutoProto_Doubler_IntegerReturn()
    {
        // Doubler has one method: Double(long) → long
        var b = Run($@"
        load('{Dll}', 'ReflectFunction.Doubler')
        result = Double(21)
end");
        Assert.AreEqual(0, b.ErrorCodeHistory.Count);
        Assert.AreEqual("42", Str("result", b));
    }

    [TestMethod]
    public void AutoProto_Greeter_StaticStringMethod()
    {
        // Greeter has one static method: Greet(string) → string
        var b = Run($@"
        load('{Dll}', 'ReflectFunction.Greeter')
        result = Greet('World')
end");
        Assert.AreEqual(0, b.ErrorCodeHistory.Count);
        Assert.AreEqual("Hello, World!", Str("result", b));
    }

    [TestMethod]
    public void AutoProto_SuccessBranch()
    {
        var b = Run($@"
        load('{Dll}', 'ReflectFunction.Doubler')   :S(OK)F(FAIL)
FAIL    result = 'failed'                           :(END)
OK      result = 'ok'
END
end");
        Assert.AreEqual("ok", Str("result", b));
    }

    [TestMethod]
    public void AutoProto_FailureBranch_ClassNotFound()
    {
        var b = Run($@"
        load('{Dll}', 'ReflectFunction.NoSuchClass')   :S(OK)F(FAIL)
FAIL    result = 'failed'                               :(END)
OK      result = 'ok'
END
end");
        Assert.AreEqual("failed", Str("result", b));
    }

    [TestMethod]
    public void AutoProto_FailureBranch_AmbiguousMultiMethod()
    {
        // Calculator has two public methods — auto-prototype without :: must fail
        var b = Run($@"
        load('{Dll}', 'ReflectFunction.Calculator')   :S(OK)F(FAIL)
FAIL    result = 'failed'                              :(END)
OK      result = 'ok'
END
end");
        Assert.AreEqual("failed", Str("result", b));
    }

    [TestMethod]
    public void AutoProto_Idempotent_DoubleLoad()
    {
        var b = Run($@"
        load('{Dll}', 'ReflectFunction.Doubler')
        load('{Dll}', 'ReflectFunction.Doubler')
        result = Double(7)
end");
        Assert.AreEqual(0, b.ErrorCodeHistory.Count);
        Assert.AreEqual("14", Str("result", b));
    }

    [TestMethod]
    public void AutoProto_UnloadByFname()
    {
        var b = Run($@"
        load('{Dll}', 'ReflectFunction.Doubler')
        r1 = Double(5)
        unload('Double')
        r2 = r1
end");
        Assert.AreEqual(0, b.ErrorCodeHistory.Count);
        Assert.AreEqual("10", Str("r1", b));
    }

    [TestMethod]
    public void AutoProto_Formatter_MixedArgs()
    {
        // Formatter.Format(string, long) → string
        var b = Run($@"
        load('{Dll}', 'ReflectFunction.Formatter')
        result = Format('count', 99)
end");
        Assert.AreEqual(0, b.ErrorCodeHistory.Count);
        Assert.AreEqual("count=99", Str("result", b));
    }

    // ── Step 3: explicit ::MethodName binding ─────────────────────────────

    [TestMethod]
    public void ExplicitBinding_Calculator_Square()
    {
        var b = Run($@"
        load('{Dll}', 'ReflectFunction.Calculator::Square')
        result = Square(9.0)
end");
        Assert.AreEqual(0, b.ErrorCodeHistory.Count);
        Assert.AreEqual("81.", Str("result", b));
    }

    [TestMethod]
    public void ExplicitBinding_Calculator_Cube()
    {
        var b = Run($@"
        load('{Dll}', 'ReflectFunction.Calculator::Cube')
        result = Cube(3.0)
end");
        Assert.AreEqual(0, b.ErrorCodeHistory.Count);
        Assert.AreEqual("27.", Str("result", b));
    }

    [TestMethod]
    public void ExplicitBinding_BothMethodsFromSameDll()
    {
        // Load Square and Cube from the same DLL in two calls
        var b = Run($@"
        load('{Dll}', 'ReflectFunction.Calculator::Square')
        load('{Dll}', 'ReflectFunction.Calculator::Cube')
        r1 = Square(4.0)
        r2 = Cube(2.0)
end");
        Assert.AreEqual(0, b.ErrorCodeHistory.Count);
        Assert.AreEqual("16.", Str("r1", b));
        Assert.AreEqual("8.",  Str("r2", b));
    }

    [TestMethod]
    public void ExplicitBinding_FailureBranch_MethodNotFound()
    {
        var b = Run($@"
        load('{Dll}', 'ReflectFunction.Calculator::NoSuchMethod')   :S(OK)F(FAIL)
FAIL    result = 'failed'                                            :(END)
OK      result = 'ok'
END
end");
        Assert.AreEqual("failed", Str("result", b));
    }

    [TestMethod]
    public void ExplicitBinding_UnloadByFname()
    {
        var b = Run($@"
        load('{Dll}', 'ReflectFunction.Calculator::Square')
        r1 = Square(6.0)
        unload('Square')
        r2 = r1
end");
        Assert.AreEqual(0, b.ErrorCodeHistory.Count);
        Assert.AreEqual("36.", Str("r1", b));
    }

    // ── Coexistence: IExternalLibrary and auto-prototype in same program ──

    [TestMethod]
    public void AutoProto_CoexistsWithIExternalLibrary()
    {
        var mathDll = SetupTests.MathLibraryPath;
        var b = Run($@"
        load('{mathDll}', 'MathFunction.MathFunctions')
        load('{Dll}', 'ReflectFunction.Doubler')
        r1 = Add(10, 5)
        r2 = Double(7)
end");
        Assert.AreEqual(0, b.ErrorCodeHistory.Count);
        Assert.AreEqual("15",  Str("r1", b));
        Assert.AreEqual("14",  Str("r2", b));
    }
}
