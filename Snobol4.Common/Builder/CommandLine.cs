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
// -u   [HOST() NOT IMPLEMENTED. DIFFERS FROM ORIGINAL SPITBOL]
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

        foreach (var arg in commandLine)
        {
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
                ShowCompilerStatistics = true;
                ShowExecutionStatistics = true;
                ShowListing = true;
                break;

            case "-b":
                SuppressSignOnMessage = true;
                break;

            case "-c":
                if (command.Length > 2 && command[..3] == "-cs")
                {
                    WriteCSharpCode = true;
                    break;
                }

                ShowCompilerStatistics = true;
                break;

            case "-F":
                CaseFolding = true;
                break;

            case "-f":
                CaseFolding = false;
                break;

            case "-h":
                SuppressListingHeader = true;
                break;

            case "-k":
                StopOnRuntimeError = true;
                break;

            case "-l":
                ShowListing = true;
                break;

            case "-n":
                SuppressExecution = true;
                break;

            case "-o":
                if (command.Length > 3 || command[2] == '=')
                {
                    ListFileName = command[3..];

                    if (Path.GetExtension(ListFileName) == "")
                        ListFileName += ".lst";

                    break;
                }

                Console.Error.WriteLine($@"Invalid parameter for option {command}");
                break;

            case "-r":
                InputAfterEndStatement = true;
                break;

            case "-v":
                GenerateDebugSymbols = true;
                break;

            case "-w":
                WriteDll = true;
                break;

            case "-x":
                ShowExecutionStatistics = true;
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
                           -a equal to -c -l -c                                       -b suppress signon message
                           -c compiler statistics                                     -cs write c# file
                           -f don't fold source code (don't ignore source code case)  -F fold source code case (ignore source code case)
                           -h suppress sign-on message in listing                     -k stop on runtime error
                           -l show listing                                            -n suppress execution
                           -o=file[.lst] listing file                                 -v generate debug symbols
                           -w write DLL                                               -x execution statistics
                           -? display manual

                          option defaults: -F

                          """);
    }

    //public static void DisplayManualDll()
    //{
    //    Console.Error.WriteLine("""

    //                      usage: snobol4 [options] file.dll
    //                       -b suppress signon message  -k stop on runtime error
    //                       -o=file[.lst] listing file  -x execution statistics
    //                       -? display manual

    //                      """);
    //}

}