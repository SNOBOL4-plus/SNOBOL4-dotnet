namespace Snobol4.Common;

public partial class Executive
{
    //172 REWIND argument is not a suitable name
    //173 REWIND argument is null
    //174 REWIND file does not exist
    //175 REWIND file does not permit rewind
    //176 REWIND caused non-recoverable error

    internal void Rewind(List<Var> arguments)
    {
        if (!arguments[0].Convert(VarType.STRING, out _, out var channelStr, this))
            LogRuntimeException(172);

        var channel = (string)channelStr;

        if (channel == "")
        {
            LogRuntimeException(173);
            return;
        }

        if (!StreamReadersByChannel.TryGetValue(channel, out var stream))
        {
            if (!StreamOutputs.TryGetValue(channel, out var streamOut))
            {
                LogRuntimeException(174);
                return;
            }

            streamOut.Position = 0;
            PredicateSuccess();
        }

        if (stream != null)
        {
            stream.BaseStream.Position = 0;
            stream.DiscardBufferedData();
            PredicateSuccess();
            return;
        }

        NonExceptionFailure();
    }
}