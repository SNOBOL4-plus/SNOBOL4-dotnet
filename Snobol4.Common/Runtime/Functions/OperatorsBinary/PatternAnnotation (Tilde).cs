namespace Snobol4.Common;

// Binary ~ operator: pat ~ label
// Returns pat as a pattern; label is a parse-tree annotation (ignored at match time).
// Used by beauty.sno parse-tree builder: *Id ~ 'Id', BREAK(...) ~ 'Label', etc.
// Error 29 (undefined operator) is raised only when both operands are non-pattern strings
// and no OPSYN has been defined for __~.

public partial class Executive
{
    public void PatternAnnotation(List<Var> arguments)
    {
        // left: pattern (or convertible to pattern)
        // right: annotation label (string) — used by tree builder, ignored here
        if (arguments[0] is ExpressionVar expressionVar0)
            arguments[0] = new PatternVar(UnevaluatedPattern.Structure(expressionVar0.FunctionName));

        if (!arguments[0].Convert(VarType.PATTERN, out _, out var leftPattern, this))
        {
            LogRuntimeException(29);
            return;
        }

        // Return the pattern unchanged; the label is annotation metadata only.
        SystemStack.Push(new PatternVar((Pattern)leftPattern));
    }
}
