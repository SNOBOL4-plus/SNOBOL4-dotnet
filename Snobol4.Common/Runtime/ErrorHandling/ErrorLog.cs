namespace Snobol4.Common;

public partial class Executive
{

    public StringVar LogRuntimeException(int code, Exception? e = null)
    {
        AmpErrorType = code;
        Failure = true;
        var nullStringVar = new StringVar(false);
        SystemStack.Push(nullStringVar);
        var ce = new CompilerException(code);
        Parent.ErrorCodeHistory.Add(code);
        Parent.ColumnHistory.Add(0);
        var fi = new FileInfo(SourceFiles[AmpCurrentLineNumber]);
        ce.Message = $"{Environment.NewLine}{fi.Name}({SourceLineNumbers[AmpCurrentLineNumber - 1]}) : error {code} -- {CompilerException.ErrorMessage[code]}{Environment.NewLine}{SourceCode[AmpCurrentLineNumber - 1].Split('\n')[1]}";

        AmpErrorText = ce.Message[2..];

        if (e != null)
        {
            ce.Message += $"{Environment.NewLine}{e.Message}";
            Console.Error.WriteLine(ce.Message);
            AmpErrorText = e.Message[2..];
        }

        Parent.MessageHistory.Add(ce.Message);
        var errorLimit = AmpErrorLimit;
        Console.Error.WriteLine($@"{ce.Message}");
        AmpErrorText = ce.Message;

        if (!Parent.CodeMode && (errorLimit < 1 || Parent.BuildOptions.StopOnRuntimeError))
        {
            AmpErrorType = ce.Code;
            throw ce;
        }

        AmpErrorLimit--;
        return nullStringVar;
    }

    public StringVar NonExceptionFailure()
    {
        Failure = true;
        var nullVar = new StringVar(false);
        SystemStack.Push(nullVar);
        return nullVar;
    }

    public StringVar PredicateSuccess()
    {
        Failure = false;
        var nullVar = new StringVar(true);
        SystemStack.Push(nullVar);
        return nullVar;
    }
}