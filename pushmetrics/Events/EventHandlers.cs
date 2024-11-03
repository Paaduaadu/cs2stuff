using eventbuffer_contract.Events;
using InfluxDB.Client.Writes;

namespace eventbuffer.Events
{
    public static class EventHandlers
    {
        public static Task<Task>[] Create(Func<PointData, Task> writeMetric) =>
            [
                Influx.ProcessEvent<EventPlayerDeath>(Transform.ToMetric, writeMetric),
            Influx.ProcessEvent<EventPlayerHurt>(Transform.ToMetric, writeMetric),
            Influx.ProcessEvent<EventRoundMvp>(Transform.ToMetric, writeMetric),
            Influx.ProcessEvent<EventBombPlanted>(Transform.ToMetric, writeMetric)
            ];
    }
}