﻿namespace streaming;

using eventbuffer_contract;
using eventbuffer_contract.Types;
using eventbuffer_redis;

public static class EventBufferFactory
{
    public static Task<EventBufferContract<EventPlayerDeath>.Read> GetReadEventPlayerDeath()  =>
        RedisEventBuffer.GetReadOne<EventPlayerDeath>(nameof(EventPlayerDeath));

     public static EventBufferContract<EventPlayerDeath>.Append GetAppendPlayerDeath() =>
        RedisEventBuffer.GetAppendOne<EventPlayerDeath>(nameof(EventPlayerDeath));   

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