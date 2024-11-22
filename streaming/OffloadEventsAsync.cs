using System.Threading.Channels;

namespace streaming;

public static class OffloadEventsAsync
{
    public static Task Process<TEvent, TMetric>(
        Func<Task<TEvent>> readOneEvent,
        Func<TEvent, TMetric> transformToMetric,
        Func<TMetric, Task> loadMetric) where TEvent : struct =>
            ReadToEnd(readOneEvent)
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

    public static async Task ExtractTransformLoad<TIn, TOut>(
        this IAsyncEnumerable<TIn> extract, 
        Func<TIn, TOut> transform,
        Func<TOut, Task> load)
    {
        await foreach(var x in extract)
        {
            await load(transform(x));
        }
    }

    public static async IAsyncEnumerable<T> ReadToEnd<T>(Func<Task<T>> readOne) where T : struct {
        var ct = new CancellationTokenSource().Token;
        while (!ct.IsCancellationRequested) {
            T v = await readOne();
            
            if (v.Equals(default(T))){
                await Task.Delay(TimeSpan.FromSeconds(1));
                yield return default;
                continue;
            }

            yield return v;
        }
    }

    private static Channel<T> CreateSingleReaderChannel<T>() =>
        Channel.CreateUnbounded<T>(
            new UnboundedChannelOptions
            {
                SingleReader = true
            }
        );
}
