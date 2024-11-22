namespace streaming;

using eventbuffer_redis;

public static class EventBufferFactory
{
    public async static Task<Task> ProcessEvent<TGameEvent, TMetric>(
        Func<TGameEvent, TMetric> toMetric,
        Func<TMetric, Task> writeMetric)
        where TGameEvent : struct
    {
        Console.WriteLine($"Started to process {typeof(TGameEvent).Name}");
        return OffloadEventsAsync.Process(
                await GetReadEvent<TGameEvent>(),
                toMetric,
                writeMetric);
    }

    private static Task<Func<Task<T>>> GetReadEvent<T>() where T : struct  =>
        RedisEventBuffer.GetReadOne<T>(typeof(T).Name);

    public static Func<T, Task> GetAppendEvent<T>() where T : struct =>
        RedisEventBuffer.GetAppendOne<T>(typeof(T).Name);
}