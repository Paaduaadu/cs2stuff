using eventbuffer.Events;
using eventbuffer_contract.Events;
using InfluxDB.Client.Writes;

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

        private static Task<Task> ProcessEvent<T>(Func<T, PointData> toMetric, Func<PointData, Task> writeMetric)
            where T : struct, IHasGameMetadata =>
                Influx.ProcessEvent(WithGameMetadata(toMetric), writeMetric);

        private static Func<T, PointData> WithGameMetadata<T>(Func<T, PointData> toMetric) where T : IHasGameMetadata =>
            x =>
                toMetric(x)
                    .Tag("Metadata.MapName", x.Metadata.MapName)
                    .Tag("Metadata.HasMatchStarted", x.Metadata.HasMatchStarted.ToString());
    }
}