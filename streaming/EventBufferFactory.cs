namespace streaming;

using eventbuffer_redis;

public static class EventBufferFactory
{
    public async static Task<Task> ProcessEvent<TGameEvent, TMetric>(
        Func<TGameEvent, TMetric> toMetric,
        Func<TMetric, Task> writeMetric,
        string redisHostName)
        where TGameEvent : struct
    {
        Console.WriteLine($"Started to process {typeof(TGameEvent).Name}");
        return OffloadEventsAsync.Process(
                await GetReadEvent<TGameEvent>(redisHostName),
                toMetric,
                writeMetric);
    }

    private static Task<Func<Task<T>>> GetReadEvent<T>(string redisHostName) where T : struct  =>
        RedisEventBuffer.GetReadOne<T>(typeof(T).Name, redisHostName);

    public static Func<T, Task> GetAppendEvent<T>(string redisHostName) where T : struct =>
        RedisEventBuffer.GetAppendOne<T>(typeof(T).Name, redisHostName);
}