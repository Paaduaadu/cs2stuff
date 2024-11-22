using eventbuffer_contract.Events;
using InfluxDB.Client.Writes;
using streaming;

namespace pushmetrics.Events
{
    public static class EventHandlers
    {
        public static Task<Task>[] Create(Func<PointData, Task> writeMetric) =>
            [
                ProcessEvent<EventPlayerDeath>(Transform.ToMetric, writeMetric),
                ProcessEvent<EventPlayerHurt>(Transform.ToMetric, writeMetric),
                ProcessEvent<EventRoundMvp>(Transform.ToMetric, writeMetric),
                ProcessEvent<EventBombPlanted>(Transform.ToMetric, writeMetric)
            ];

        private static Task<Task> ProcessEvent<TGameEvent>(Func<TGameEvent, PointData> toMetric, Func<PointData, Task> writeMetric)
            where TGameEvent : struct, IHasGameMetadata =>
                EventBufferFactory
                    .ProcessEvent(
                        toMetric.WithGameMetadata(),
                        writeMetric);
    }
}