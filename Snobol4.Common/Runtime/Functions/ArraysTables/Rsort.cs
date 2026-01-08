namespace Snobol4.Common;

public partial class Executive
{
    #region RSort

    internal void ReverseSort(List<Var> arguments)
    {
        BaseSort(arguments, false);
    }

    #endregion
}