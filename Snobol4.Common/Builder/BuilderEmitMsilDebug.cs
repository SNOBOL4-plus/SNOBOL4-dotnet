using System.Reflection;
using System.Reflection.Emit;

namespace Snobol4.Common;

public partial class Builder
{
    /// <summary>
    /// Saves every compiled MSIL body delegate to a .NET assembly on disk.
    /// Open the resulting file in ILSpy or run <c>ildasm</c> on it to inspect
    /// the generated IL for every SNOBOL4 statement.
    ///
    /// Must be called after <see cref="EmitMsilForAllStatements"/> has run.
    ///
    /// Usage (e.g. in a test or after <c>Build()</c>):
    /// <code>
    ///     builder.DumpMsilToFile(@"C:\temp\snobol4_msil_dump.dll");
    /// </code>
    /// </summary>
    internal void DumpMsilToFile(string path)
    {
        var ab = new PersistedAssemblyBuilder(
            new AssemblyName("Snobol4MsilDump"), typeof(object).Assembly);
        var modBuilder  = ab.DefineDynamicModule("Snobol4MsilDump");
        var typeBuilder = modBuilder.DefineType(
            "Snobol4_Stmts",
            TypeAttributes.Public | TypeAttributes.Abstract | TypeAttributes.Sealed);

        for (var si = 0; si < Code.SourceLines.Count; si++)
        {
            var line = Code.SourceLines[si];

            // Only dump statements that were actually compiled to MSIL.
            if (!MsilCache.ContainsKey(line.ParseBody)) continue;

            DetectGotoForms(line,
                out var directUncondLabel,  out var indirectUncondExpr, out var indirectUncondCode,
                out var successLabel,       out var successExpr,        out var successExprCode,
                out var failureLabel,       out var failureExpr,        out var failureExprCode);

            // Use the source label (if any) in the method name for readability.
            var label   = string.IsNullOrEmpty(line.Label) ? "" : $"_{line.Label}";
            var mb = typeBuilder.DefineMethod(
                $"Stmt_{si:0000}{label}",
                MethodAttributes.Public | MethodAttributes.Static,
                typeof(int), [typeof(Executive)]);

            EmitBodyIntoIL(mb.GetILGenerator(), line.ParseBody, si, isBody: true,
                           directUncondLabel,  indirectUncondExpr, indirectUncondCode,
                           successLabel,       successExpr,        successExprCode,
                           failureLabel,       failureExpr,        failureExprCode,
                           line.SuccessFirst);
        }

        typeBuilder.CreateType();
        ab.Save(path);
    }
}
