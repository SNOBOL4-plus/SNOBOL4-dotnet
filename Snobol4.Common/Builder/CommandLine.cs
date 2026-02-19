namespace Snobol4.Common;

// Command Line Options
// - Options are case-sensitive
// - Options must precede file names
// - Options cannot be concatenated (e.g. -lcx) [DIFFERS FROM ORIGINAL SPITBOL]
// - Parameters for command line options must follow the option without an
//   intervening space (e.g. -o=filename) [DIFFERS FROM ORIGINAL SPITBOL]
// - If the parameter has spaces, it must be enclosed in double quotes (e.g. -o="string data")
//
// -a   equivalent to -c, -l -x
// -b   SuppressSignOnMessage
// -c   ShowCompilerStatistics
// -cs  WriteCSharpCode [NEW. DIFFERS FROM ORIGINAL SPITBOL]
// -d   [NOT IMPLEMENTED. HANDLED BY .NET FRAMEWORK. DIFFERS FROM ORIGINAL SPITBOL]
// -e   [NOT IMPLEMENTED. ALL OUTPUT GOES TO STANDARD OUTPUT. DIFFERS FROM ORIGINAL SPITBOL]
// -f   CaseFolding OFF
// -F   CaseFolding ON
// -g   [NOT IMPLEMENTED. RECOMMEND USING A TEXT EDITOR FOR PRINTING. DIFFERS FROM ORIGINAL SPITBOL]
// -h   SuppressListingHeader
// -i   [NOT IMPLEMENTED. HANDLED BY .NET FRAMEWORK. DIFFERS FROM ORIGINAL SPITBOL]
// -j   [UNASSIGNED]
// -k   StopOnRuntimeError
// -l   ShowListing
// -m   [NOT IMPLEMENTED. HANDLED BY .NET FRAMEWORK. DIFFERS FROM ORIGINAL SPITBOL]
// -n   SuppressExecution
// -o   Redirect STDERR to list file
// -p   [NOT IMPLEMENTED. RECOMMEND USING A TEXT EDITOR FOR PRINTING. DIFFERS FROM ORIGINAL SPITBOL]
// -q   [UNASSIGNED]
// -r   [Input After End Statement NOT IMPLEMENTED. HANDLED BY .NET FRAMEWORK. DIFFERS FROM ORIGINAL SPITBOL]
// -s   [NOT IMPLEMENTED. HANDLED BY .NET FRAMEWORK. DIFFERS FROM ORIGINAL SPITBOL]
// -t   [NOT IMPLEMENTED. RECOMMEND USING A TEXT EDITOR FOR PRINTING. DIFFERS FROM ORIGINAL SPITBOL]
// -u   Host Parameter
// -v   Generate Debug Symbols
// -w   WriteDll (DIFFERS FROM ORIGINAL SPITBOL)
// -x   ShowExecutionStatistics
// -y   [UNASSIGNED]
// -z   [NOT IMPLEMENTED. RECOMMEND USING A TEXT EDITOR FOR PRINTING. DIFFERS FROM ORIGINAL SPITBOL]
// -?   Display manual
// -#   [Associate number with file name NOT IMPLEMENTED. DIFFERS FROM ORIGINAL SPITBOL]

// Options that affect runtime:
// -b   SuppressSignOnMessage
// -k   StopOnRuntimeError
// -o   Redirect STDERR to list file
// -x   ShowExecutionStatistics

public partial class Builder
{
    public void ParseCommandLine(string[] commandLine)
    {
        var commandMode = true;
        var count = 0;
        var hostParameterIsNext = false;

        // Default value of HOST(0) is the concatenationof all command line arguments.
        BuildOptions.HostParameter = string.Join(" ", commandLine);

        foreach (var arg in commandLine)
        {
            if (hostParameterIsNext)
            {
                BuildOptions.HostParameter = arg;
                hostParameterIsNext = false;
                continue;
            }

            if (arg == "-u")
            {
                hostParameterIsNext = true;
                continue;
            }

            if (commandMode && arg[0] == '-' && arg.Length > 1)
            {
                ArgumentSwitch(arg);
                continue;
            }

            commandMode = false;
            FilesToCompile.Add(arg);
        }

        if (FilesToCompile.Count > 0)
        {
            count += FilesToCompile.Count;
            Arguments = commandLine[count..];
            return;
        }

        DisplayManual();
    }

    private void ArgumentSwitch(string command)
    {
        switch (command[..2])
        {
            case "-a":
                BuildOptions.ShowCompilerStatistics = true;
                BuildOptions.ShowExecutionStatistics = true;
                BuildOptions.ShowListing = true;
                break;

            case "-b":
                BuildOptions.SuppressSignOnMessage = true;
                break;

            case "-c":
                if (command.Length > 2 && command[..3] == "-cs")
                {
                    BuildOptions.WriteCSharpCode = true;
                    break;
                }

                BuildOptions.ShowCompilerStatistics = true;
                break;

            case "-F":
                BuildOptions.CaseFolding = true;
                break;

            case "-f":
                BuildOptions.CaseFolding = false;
                break;

            case "-h":
                BuildOptions.SuppressListingHeader = true;
                break;

            case "-k":
                BuildOptions.StopOnRuntimeError = true;
                break;

            case "-l":
                BuildOptions.ShowListing = true;
                break;

            case "-n":
                BuildOptions.SuppressExecution = true;
                break;

            case "-o":
                if (command.Length > 3 || command[2] == '=')
                {
                    BuildOptions.ListFileName = command[3..];

                    if (Path.GetExtension(BuildOptions.ListFileName) == "")
                        BuildOptions.ListFileName += ".lst";

                    break;
                }

                Console.Error.WriteLine($@"Invalid parameter for option {command}");
                break;

            case "-r":
                BuildOptions.InputAfterEndStatement = true;
                break;

            case "-v":
                BuildOptions.GenerateDebugSymbols = true;
                break;

            case "-w":
                BuildOptions.WriteDll = true;
                break;

            case "-x":
                BuildOptions.ShowExecutionStatistics = true;
                break;

            case "-?":
                DisplayManual();
                break;

            default:
                Console.Error.WriteLine(@$"Invalid option {command}");
                break;
        }
    }

    public static void DisplayManual()
    {
        Console.Error.WriteLine("""

                          usage: snobol4 [options] files[.sno, .sbl, .spt]
                          source files are concatenated
                           -a equal to -c -l -x                    -b suppress signon message
                           -c compiler statistics                  -cs write c# file
                           -f don't fold source code               -F fold source code case
                           -h suppress sign-on message in listing  -k stop on runtime error
                           -l show listing                         -n suppress execution
                           -o=file[.lst] listing file              -u "host parameter string"
                           -v generate debug symbols               -w write DLL
                           -x execution statistics                 -? display manual

                          option defaults: -F

                          """);
    }
}