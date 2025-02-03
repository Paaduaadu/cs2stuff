using pushmetrics.Core;
using pushmetrics.Events;

Console.WriteLine("0.0.5");

var writeMetric = Influx.GetWrite(ConsoleExtensions.ReadSecret("INFLUXDB_TOKEN_FILE"));
var redisHostName = args.ElementAtOrDefault(0) ?? "buffer";;

await Task.WhenAll(
    await EventHandlers
        .Create(writeMetric, redisHostName)
        .ToAsyncEnumerable()
        .SelectAwait(async t => await t)
        .ToArrayAsync());
