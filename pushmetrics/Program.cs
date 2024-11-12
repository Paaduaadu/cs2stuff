using eventbuffer.Events;
using pushmetrics.Core;

Console.WriteLine("0.0.5");

var writeMetric = Influx.GetWrite(ConsoleExtensions.ReadSecret("INFLUXDB_TOKEN_FILE"));

await Task.WhenAll(
    await EventHandlers
        .Create(writeMetric)
        .ToAsyncEnumerable()
        .SelectAwait(async t => await t)
        .ToArrayAsync());
