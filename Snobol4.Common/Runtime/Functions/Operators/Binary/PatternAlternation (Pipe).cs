namespace Snobol4.Common;

public partial class Executive
{
    private void CreateAlternatePattern(List<Var> arguments)
    {
        if (!arguments[0].Convert(VarType.PATTERN, out _, out var patternLeft, this))
        {
            LogRuntimeException(5);
            return;
        }

        if (!arguments[1].Convert(VarType.PATTERN, out _, out var patternRight, this))
        {
            LogRuntimeException(6);
            return;
        }

        SystemStack.Push(new PatternVar(new AlternatePattern((Pattern)patternLeft, (Pattern)patternRight)));
    }
}