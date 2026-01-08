namespace Snobol4.Common;

public class FunctionTableEntry
{
    #region Members

    public delegate void FunctionHandler(List<Var> list);

    internal FunctionHandler Handler;
    internal int ArgumentCount;
    internal bool IsProtected;
    internal List<string> Locals;
    internal Stack<List<Var>> StateStack;
    internal string Symbol;
    internal string Prototype;

    #endregion

    #region Constructors

    public FunctionTableEntry(string symbol, FunctionHandler handler, int argumentCount, bool isProtected)
    {
        Handler = handler;
        ArgumentCount = argumentCount;
        Locals = [];
        IsProtected = isProtected;
        StateStack = [];
        Symbol = symbol;
        Prototype = "";
    }

    public FunctionTableEntry(string symbol, FunctionHandler handler, int argumentCount, List<string> locals, string prototype)
    {
        Handler = handler;
        ArgumentCount = argumentCount;
        Locals = locals;
        IsProtected = false;
        StateStack = [];
        Symbol = symbol;
        Prototype = prototype;
    }

    #endregion
}