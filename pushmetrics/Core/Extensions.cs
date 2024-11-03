using streaming;

public static class Extensions
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

    public static string ReadSecret(string envVariableName) =>
        File.ReadAllText(Environment.GetEnvironmentVariable(envVariableName)!).Trim();
}

