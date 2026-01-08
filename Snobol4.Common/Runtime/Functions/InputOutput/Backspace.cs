namespace Snobol4.Common;

public partial class Executive
{
    //"backspace argument is not a suitable name" /* 316 */,
    //"backspace file does not exist" /* 317 */,
    //"backspace file does not permit backspace" /* 318 */,
    //"backspace caused non-recoverable error" /* 319 */,

    internal void BackspaceFile(List<Var> arguments)
    {
        // Channel must be a string
        if (!arguments[0].Convert(VarType.STRING, out _, out var channel, this))
        {
            LogRuntimeException(316);
            return;
        }

        // Channel must exist as a reader
        if (StreamReadersByChannel.ContainsKey((string)channel))
        {
            Seek(StreamReadersByChannel[(string)channel], -1, 1);
            PredicateSuccess();
            return;
        }

        LogRuntimeException(StreamOutputs.ContainsKey((string)channel) ? 318 : 317);
    }
}