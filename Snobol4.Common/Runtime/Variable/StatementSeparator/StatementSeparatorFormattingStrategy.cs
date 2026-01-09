using System.Runtime.CompilerServices;

namespace Snobol4.Common;

/// <summary>
/// Formatting strategy for statement separator
/// </summary>
public sealed class StatementSeparatorFormattingStrategy : IFormattingStrategy
{
    private const string ToStringValue = "<statement-separator>";
    private const string DumpStringValue = "───";
    private const string DebugStringValue = "STATEMENT SEPARATOR";

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string ToString(Var self)
    {
        return ToStringValue;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string DumpString(Var self)
    {
        return DumpStringValue;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string DebugString(Var self)
    {
        return DebugStringValue;
    }
}