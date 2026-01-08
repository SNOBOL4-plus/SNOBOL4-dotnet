namespace Snobol4.Common;

public partial class Executive
{
    #region Sort

    internal void Sort(List<Var> arguments)
    {
        BaseSort(arguments, true);
    }

    #endregion
}