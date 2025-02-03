using eventbuffer_contract.Events;
using InfluxDB.Client.Writes;
using streaming;

namespace pushmetrics.Events
{
    public static class EventHandlers
    {
        public static Task<Task>[] Create(Func<PointData, Task> writeMetric, string redisHostName) =>
            [
                ProcessEvent<EventPlayerDeath>(Transform.ToMetric, writeMetric, redisHostName),
                ProcessEvent<EventPlayerHurt>(Transform.ToMetric, writeMetric, redisHostName),
                ProcessEvent<EventRoundMvp>(Transform.ToMetric, writeMetric, redisHostName),
                ProcessEvent<EventBombPlanted>(Transform.ToMetric, writeMetric, redisHostName)
            ];

        private static Task<Task> ProcessEvent<TGameEvent>(Func<TGameEvent, PointData> toMetric, Func<PointData, Task> writeMetric, string redisHostName)
            where TGameEvent : struct, IHasGameMetadata =>
                EventBufferFactory
                    .ProcessEvent(
                        toMetric.WithGameMetadata(),
                        writeMetric,
                        redisHostName);
    }
}