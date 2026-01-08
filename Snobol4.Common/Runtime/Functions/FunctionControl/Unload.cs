namespace Snobol4.Common;

public partial class Executive
{
    internal void UnloadExternalFunction(List<Var> arguments)
    {
        if (!arguments[0].Convert(VarType.STRING, out _, out var path, this))
        {
            LogRuntimeException(137);
            return;
        }

        if ((string)path == "")
        {
            LogRuntimeException(138);
            return;
        }

        ActiveContexts[Path.GetFileName((string)path)].Unload();
        ActiveContexts.Remove(Path.GetFileName((string)path));
    }
}