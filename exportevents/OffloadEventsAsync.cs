using System.Threading.Channels;
using streaming;

namespace exportevents;

public static class OffloadEventsAsync
{
    public static (Task ReadChannelTask, Action<Exception?> StopWriting) ListenEventAndPublish<TSourceEvent, TTargetType>(
        Action<Action<TSourceEvent>> publishCallback, 
        Func<TSourceEvent, TTargetType> transform)
        where TTargetType : struct
    {
        var channel = CreateChannel<TTargetType>();

        publishCallback(e =>
        {
            channel.Writer.TryWrite(transform(e));
        });

        return (
            ReadChannel(
                channel,
                EventBufferFactory.GetAppendEvent<TTargetType>()),
            channel.Writer.Complete
        );
    }

    private static Channel<T> CreateChannel<T>()
    {
        var channel = Channel.CreateUnbounded<T>(
            new UnboundedChannelOptions
            {
                SingleReader = true
            }
        );
        return channel;
    }

    private static Task ReadChannel<T>(
        Channel<T> channel,
        Func<T, Task> load) => 
        channel
            .Reader
            .ReadAllAsync()
            .ExtractTransformLoad(
                gameEvent => gameEvent,
                load);
}
