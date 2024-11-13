using System.Text.Json;
using publishmetrics.Core;
using publishmetrics.Types;

var influxHost = args.ElementAtOrDefault(0) ?? "http://influxdb2:8086";
var queryFilesPath = args.ElementAtOrDefault(1) ?? "/App/Queries";
var resultFilesPath = args.ElementAtOrDefault(2) ?? "/App/Results";

var streamCache = new Dictionary<string, Stream>();
var getTargetStream = FunctionalExtensions.Memoize(path =>
    new FileStream(path, FileMode.Create), streamCache);

await Task.WhenAll(
    Process<PlayersDataRecord>([Path.Combine(queryFilesPath, "players_data.flux")], x => x.Last()),
    Process<PlayerStatsDataRecord>([Path.Combine(queryFilesPath, "player_stats_data.flux")], x => x.Single()));

streamCache.Values.ToList().ForEach(w => w.Close());

IAsyncEnumerable<Task> EnumerateResults<T>(IEnumerable<string> queryFiles, Func<IEnumerable<T>, T> aggregateDuplicates) where T : BaseRecord, new() =>
    queryFiles
        .Select(file =>
            new
            {
                Name = Path.GetFileNameWithoutExtension(file),
                Query = File.ReadAllText(file),
                Read = GetRead<T>(influxHost)
            }
        )
        .ToAsyncEnumerable()
        .SelectAwait(async querySpec =>
            (
                Result: await querySpec
                    .Read(querySpec.Query)
                    .Where(rec => !string.IsNullOrEmpty(rec.SteamID))
                    .ToDictionaryAsync(x => x.SteamID!, aggregateDuplicates),
                TargetStream: getTargetStream(Path.Combine(resultFilesPath, querySpec.Name + ".json"))))
        .Select(resultsSpec =>
            JsonSerializer.SerializeAsync(
                resultsSpec.TargetStream,
                resultsSpec.Result));

static Func<string, IAsyncEnumerable<T>> GetRead<T>(string influxHost) where T : BaseRecord, new()
{
    var read = Influx.GetRead<T>(ConsoleExtensions.ReadSecret("INFLUXDB_TOKEN_FILE"), influxHost);
    return q => read(q);
}

async Task Process<T>(IEnumerable<string> queryFiles, Func<IEnumerable<T>, T> aggregate) where T : BaseRecord, new()
{
    await foreach (var t in EnumerateResults<T>(queryFiles, aggregate))
    {
        await t;
    };
}
