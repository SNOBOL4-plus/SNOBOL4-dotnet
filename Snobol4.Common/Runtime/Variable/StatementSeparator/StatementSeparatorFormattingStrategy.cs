namespace Snobol4.Common;

/// <summary>
/// Formatting strategy for statement separator
/// </summary>
public class StatementSeparatorFormattingStrategy : IFormattingStrategy
{
    public string ToString(Var self)
    {
        return "<statement-separator>";
    }

    public string DumpString(Var self)
    {
        return "───"; // Visual separator
    }

    public string DebugString(Var self)
    {
        return "STATEMENT SEPARATOR";
    }
}