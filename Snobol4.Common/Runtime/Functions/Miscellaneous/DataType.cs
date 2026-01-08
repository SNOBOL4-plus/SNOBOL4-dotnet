namespace Snobol4.Common;

public partial class Executive
{
    #region DataType

    public void DataType(List<Var> arguments)
    {
        SystemStack.Push(new StringVar(GetDataType(arguments[0])));
    }

    internal string GetDataType(Var arg)
    {
        return arg.DataType();
    }

    #endregion
}