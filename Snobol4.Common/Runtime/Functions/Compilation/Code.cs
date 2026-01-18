namespace Snobol4.Common;

//"goto operand in direct goto is not code" /* 24 */,

public partial class Executive
{
    public void CreateCode(List<Var> arguments)
    {
        CodeVar code = new()
        {
            StatementNumber = Statements.Count,
            Data = ""
        };

        var previousCaseFolding = Parent.CaseFolding;
        Parent.CaseFolding = ((IntegerVar)IdentifierTable["&case"]).Data != 0;
        Parent.CodeMode = true;

        switch (arguments[0])
        {
            case NameVar:
                if (IdentifierTable.TryGetValue(arguments[0].Symbol, out var argument) && argument is StringVar stringVar1)
                {
                    Parent.Code = new SourceCode(Parent);
                    code.Data = stringVar1.Data;
                    Parent.Code.ReadCodeInString(code.Data, Parent.FilesToCompile[^1]);
                    Parent.BuildCode();
                    Parent.CaseFolding = previousCaseFolding;
                    Parent.CodeMode = false;
                    SystemStack.Push(code);
                    return;
                }

                NonExceptionFailure();
                return;

            case StringVar stringVar:
                Parent.Code = new SourceCode(Parent);
                code.Data = stringVar.Data;
                Parent.Code.ReadCodeInString(code.Data, Parent.FilesToCompile[^1]);
                Parent.BuildCode();
                Parent.CaseFolding = previousCaseFolding;
                Parent.CodeMode = false;
                SystemStack.Push(code);
                return;

            default:
                NonExceptionFailure();
                return;
        }
    }
}