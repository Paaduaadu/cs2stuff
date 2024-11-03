using eventbuffer.Events;

Console.WriteLine("0.0.4");

var writeMetric = Influx.GetWrite(Extensions.ReadSecret("INFLUXDB_TOKEN_FILE"));

await Task.WhenAll(
    await EventHandlers
        .Create(writeMetric)
        .ToAsyncEnumerable()
        .SelectAwait(async t => await t)
        .ToArrayAsync());
