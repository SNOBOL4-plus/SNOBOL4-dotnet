namespace Snobol4.Common;

public partial class Executive
{
    #region Differ
    internal void Differ(List<Var> arguments)
    {
        if (!arguments[0].IsIdentical(arguments[1]))
        {
            PredicateSuccess();
            return;
        }

        NonExceptionFailure();
    }

    #endregion
}