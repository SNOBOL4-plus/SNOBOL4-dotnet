using System.Runtime.InteropServices;
using System.Text;
using Snobol4.Common;

namespace Test.TestLexer;

[TestClass]
public class SetupTests
{
    public static string WindowsOutput = @"..\..\..\..\..\TestSnobol4\Output\";
    public static string LinuxOutput =   @"../../../../../TestSnobol4/Output/";

    public static string WindowsDll =     @"C:\Users\jcooper\Documents\Visual Studio 2022\Snobol4.Net.Master\Snobol4.Net\CustomFunction\bin\Debug\net10.0\AreaLibrary.dll";
    public static string LinuxDll =   @"\mnt\c\Users\jcooper\Documents\Visual Studio 2022\Snobol4.Net.Master\Snobol4.Net\CustomFunction\bin\Debug\net10.0\AreaLibrary.dll";

    internal static Builder SetupScript(string directives, string script)
    {
        // Get array of commands and source files
        var commands = new List<string>(
            directives.Split(" ",
                StringSplitOptions.RemoveEmptyEntries));

        // Get cross-platform test file path
        var testDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestSnobol4");
        var testFilePath = Path.Combine(testDirectory, "Test.sno");

        // Ensure directory exists
        Directory.CreateDirectory(testDirectory);

        List<string> files = [testFilePath];

        var args = commands.Concat(files).ToArray();

        Builder builder = new();
        builder.ParseCommandLine(args);
        builder.Code.ReadTestScript(new MemoryStream(Encoding.UTF8.GetBytes(script)));
        builder.BuildMain();
        return builder;
    }

    public static bool IsWindows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
    public static bool IsLinux => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
    public static bool IsMacOs => RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
}