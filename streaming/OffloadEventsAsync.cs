using System.Threading.Channels;

namespace streaming;

public static class OffloadEventsAsync
{
    public static Task Process<TEvent, TMetric>(
        Func<Task<TEvent>> readOneEvent,
        Func<TEvent, TMetric> transformToMetric,
        Func<TMetric, Task> loadMetric) where TEvent : struct =>
        EventBufferFactory
            .ReadToEnd(readOneEvent)
            .Where(@event => !@event.Equals(default(TEvent)))
            .ExtractTransformLoad(
                transform: transformToMetric,
                load: loadMetric);
                
    public static (Task ReadChannelTask, Action<Exception?> StopWriting) ListenEventAndPublish<TSourceEvent, TTargetType>(
        Action<Action<TSourceEvent>> registerHandler,
        Func<TSourceEvent, TTargetType> transform,
        Func<TTargetType, Task> targetWriter)
        where TTargetType : struct
    {
        var channel = CreateSingleReaderChannel<TTargetType>();

        registerHandler(sourceEvent =>
            channel.Writer.TryWrite(transform(sourceEvent)));

        return (
            ReadChannelTask: channel.Reader
                .ReadAllAsync()
                .ExtractTransformLoad(
                    item => item,
                    targetWriter),
            StopWriting: channel.Writer.Complete
        );
    }

    private static Channel<T> CreateSingleReaderChannel<T>() =>
        Channel.CreateUnbounded<T>(
            new UnboundedChannelOptions
            {
                SingleReader = true
            }
        );
}
