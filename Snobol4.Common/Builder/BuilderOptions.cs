

namespace Snobol4.Common;

public class BuilderOptions
{
    public bool SuppressSignOnMessage;            // -b
    public bool ShowCompilerStatistics;           // -c
    public bool WriteCSharpCode;                  // -cs
    public bool CaseFolding;                      // -F and -f
    public bool SuppressListingHeader;            // -h
    public bool StopOnRuntimeError;               // -k
    public bool ShowListing;                      // -l
    public bool SuppressExecution;                // -n
    public bool InputAfterEndStatement;           // -r
    public string HostParameter;                  // -u
    public bool GenerateDebugSymbols;             // -v
    public bool WriteDll;                         // -w
    public bool ShowExecutionStatistics;          // -x
    public bool TraceStatements;                  // True if tracing is on
    public bool ListSource;                       // LIST/NOLIST
    internal bool ErrorOnUnhandledFail;           // FAIL/NOFAIL
    public string ListFileName;                // Name of list file;


    public BuilderOptions()
    {
        SuppressSignOnMessage = false;            // -b
        ShowCompilerStatistics = false;           // -c
        WriteCSharpCode = false;                  // -cs
        CaseFolding = true;                       // -F and -f
        SuppressListingHeader = false;            // -h
        StopOnRuntimeError = false;               // -k
        ShowListing = false;                      // -l
        SuppressExecution = false;                // -n
        InputAfterEndStatement = false;           // -r
        HostParameter = "";                       // -u
        GenerateDebugSymbols = true;              // -v
        WriteDll = false;                         // -w
        ShowExecutionStatistics = false;          // -x
        TraceStatements = false;                  // True if tracing is on
        ListSource = false;                       // LIST/NOLIST
        ErrorOnUnhandledFail = false;             // FAIL/NOFAIL
        ListFileName = "";                       // Name of list file;
    }
}

