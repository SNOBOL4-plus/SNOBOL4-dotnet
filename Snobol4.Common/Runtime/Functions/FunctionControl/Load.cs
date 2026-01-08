using System.Runtime.Loader;

namespace Snobol4.Common;

public partial class Executive
{
    //"load second argument is not a string" /* 136 */,
    //"load first argument is not a string" /* 137 */,
    //"load first argument is null" /* 138 */,
    //"load first argument is missing a left paren" /* 139 */,
    //"load first argument has null function name" /* 140 */,
    //"load first argument is missing a right paren" /* 141 */,
    //"load function does not exist" /* 142 */,
    //"load function caused input error during load" /* 143 */,

    internal Dictionary<string, AssemblyLoadContext> ActiveContexts = []; // Always case-insensitive

    internal void LoadExternalFunction(List<Var> arguments)
    {
        if (!arguments[1].Convert(VarType.STRING, out _, out var name, this))
        {
            LogRuntimeException(136);
            return;
        }

        if (!arguments[0].Convert(VarType.STRING, out _, out var path, this))
        {
            LogRuntimeException(137);
            return;
        }

        if ((string)name == "")
        {
            LogRuntimeException(138);
            return;
        }

        var loadContext = new AssemblyLoadContext(null, true);
        var dll = loadContext.LoadFromAssemblyPath((string)path);
        dynamic? instance = dll.CreateInstance((string)name);

        if (instance == null)
        {
            LogRuntimeException(143);
            return;
        }

        instance.Init(this);
        ActiveContexts[Path.GetFileName((string)path)] = loadContext;
    }
}