namespace Snobol4.Common;

//"sort/rsort 1st arg not suitable array or table" /* 256 */,
//"erroneous 2nd arg in sort/rsort of vector" /* 257 */,
//"sort/rsort 2nd arg out of range or non-integer" /* 258 */,

public partial class Executive
{
    internal void Sort(List<Var> arguments) => BaseSort(arguments, true);
}