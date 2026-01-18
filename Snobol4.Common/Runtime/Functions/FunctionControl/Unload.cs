namespace Snobol4.Common;

//"unload argument is not natural variable name" /* 201 */,

public partial class Executive
{
    internal void UnloadExternalFunction(List<Var> arguments)
    {
        if (!arguments[0].Convert(VarType.STRING, out _, out var path, this))
        {
            LogRuntimeException(201);
            return;
        }

        if ((string)path == "")
        {
            LogRuntimeException(201);
            return;
        }

        ActiveContexts[Path.GetFileName((string)path)].Unload();
        ActiveContexts.Remove(Path.GetFileName((string)path));
    }
}