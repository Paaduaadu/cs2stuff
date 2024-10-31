using eventbuffer;
using eventbuffer_contract.Types;

Console.WriteLine("0.0.4");

var writeMetric = Influx.GetWrite(Extensions.ReadSecret("INFLUXDB_TOKEN_FILE"));

await Task.WhenAll(
    await Influx.ProcessEvent<EventPlayerDeath>(Transform.ToMetric, writeMetric),
    await Influx.ProcessEvent<EventPlayerHurt>(Transform.ToMetric, writeMetric),
    await Influx.ProcessEvent<EventRoundMvp>(Transform.ToMetric, writeMetric),
    await Influx.ProcessEvent<EventBombPlanted>(Transform.ToMetric, writeMetric));