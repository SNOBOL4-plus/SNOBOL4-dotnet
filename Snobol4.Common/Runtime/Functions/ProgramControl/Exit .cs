using System.Diagnostics;

namespace Snobol4.Common;

//"exit first argument is not suitable integer or string" /* 104 */,
//"exit action not available in this implementation" /* 105 */,
//"exit action caused irrecoverable error" /* 106 */,

public partial class Executive
{
    internal void Exit(List<Var> arguments)
    {
        if(!arguments[0].Convert(VarType.STRING,out _, out var command, this))
        {
            LogRuntimeException(104);
            return;
        } 

        var startInfo = new ProcessStartInfo
        {
            FileName = @$"cmd.exe",
            Arguments = @$"/C {command}",
            CreateNoWindow = false, // Hides the command prompt window
            UseShellExecute = false, // Required to redirect output/error
            RedirectStandardOutput = false, // Redirects the output to the C# application
            RedirectStandardError = false // Redirects the error output
        };

        using var process = Process.Start(startInfo);
        Environment.Exit(process!.ExitCode);
    }
}