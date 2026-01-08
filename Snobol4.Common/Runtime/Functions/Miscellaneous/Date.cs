using System.Globalization;

namespace Snobol4.Common;

public partial class Executive
{
    internal void Date(List<Var> arguments)
    {
        SystemStack.Push(new StringVar(DateTime.Now.ToString(CultureInfo.InvariantCulture)));
    }
}