namespace streaming;

using eventbuffer_redis;

public static class EventBufferFactory
{
    public static Task<Func<Task<T>>> GetReadEvent<T>() where T : struct  =>
        RedisEventBuffer.GetReadOne<T>(typeof(T).Name);

    public static Func<T, Task> GetAppendEvent<T>() where T : struct =>
        RedisEventBuffer.GetAppendOne<T>(typeof(T).Name);

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

    
}