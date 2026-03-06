namespace Snobol4.Common;

//"unload argument is not a string" /* 201 */,

public partial class Executive
{
    internal void UnloadExternalFunction(List<Var> arguments)
    {
        if (!arguments[0].Convert(VarType.STRING, out _, out var pathObj, this))
        {
            LogRuntimeException(201);
            return;
        }

        var rawPath = (string)pathObj;

        if (string.IsNullOrEmpty(rawPath))
        {
            LogRuntimeException(201);
            return;
        }

        // Resolve path the same way Load does
        var resolvedPath = Path.IsPathRooted(rawPath)
            ? rawPath
            : Path.GetFullPath(rawPath,
                Parent.FilesToCompile.Count > 0
                    ? Path.GetDirectoryName(Parent.FilesToCompile[^1]) ?? Directory.GetCurrentDirectory()
                    : Directory.GetCurrentDirectory());

        if (!ActiveContexts.TryGetValue(resolvedPath, out var entry))
        {
            // Not loaded — treat as success (idempotent unload)
            SystemStack.Push(StringVar.Null());
            PredicateSuccess();
            return;
        }

        // Give the library a chance to de-register functions / release resources
        entry.Library.Unload();

        entry.Context.Unload();
        ActiveContexts.Remove(resolvedPath);

        SystemStack.Push(StringVar.Null());
        PredicateSuccess();
    }
}
