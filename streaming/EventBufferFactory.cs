namespace streaming;

using System.Text.Json;
using eventbuffer_contract;
using eventbuffer_redis;

public static class EventBufferFactory
{
    public static Task<EventBufferContract<T>.Read> GetReadEvent<T>() where T : struct  =>
        RedisEventBuffer.GetReadOne<T>(nameof(T));
    public static EventBufferContract<T>.Append GetAppendEvent<T>() where T : struct =>
        RedisEventBuffer.GetAppendOne<T>(nameof(T));

    private static EventBufferContract<string>.Append GetConsoleAppend() =>
        AsAsync(Console.WriteLine);

    private static EventBufferContract<string>.Append AsAsync(Action<string> value) =>
        x =>
            {
                value(x);
                return Task.CompletedTask;
            };

    public static async Task ExtractTransformLoad<TIn, TOut>(
        this IAsyncEnumerable<TIn> extract, 
        Func<TIn, TOut> transform,
        Func<TOut, Task> load)
    {
        await foreach(var x in extract)
        {
            Console.WriteLine(JsonSerializer.Serialize(x));
            await load(transform(x));
        }
    }

    public static async IAsyncEnumerable<T> ReadToEnd<T>(EventBufferContract<T>.Read readOne) where T : struct {
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

    
}