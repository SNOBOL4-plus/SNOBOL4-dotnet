namespace Snobol4.Common;

/// <summary>
/// Contract that every external SNOBOL4 library DLL must implement.
/// The runtime calls Init() after loading the assembly, giving the library
/// a chance to register its functions into the Executive's FunctionTable.
/// Unload() is called before the AssemblyLoadContext is unloaded, allowing
/// the library to de-register functions or release resources.
/// </summary>
public interface IExternalLibrary
{
    /// <summary>
    /// Called once after the assembly is loaded. Register functions here
    /// by inserting FunctionTableEntry objects into executive.FunctionTable.
    /// </summary>
    void Init(Executive executive);

    /// <summary>
    /// Called before the AssemblyLoadContext is unloaded. Default is no-op.
    /// Override to de-register functions or release unmanaged resources.
    /// </summary>
    void Unload() { }
}
