using System.Text.Json;
using publishmetrics.Core;
using publishmetrics.Types;

var influxHost = args.ElementAtOrDefault(0) ?? "http://influxdb2:8086";
var queryFilesPath = args.ElementAtOrDefault(1) ?? "/App/Queries";
var resultFilesPath = args.ElementAtOrDefault(2) ?? "/App/Results";

var writersCache = new Dictionary<string, StreamWriter>();
var getWriter = FunctionalExtensions.Memoize(path =>
    new StreamWriter(new FileStream(path, FileMode.Create)), writersCache);


await Task.WhenAll(
    Process<PlayersDataRecord>([Path.Combine(queryFilesPath, "players_data.flux")], x => x.First()),
    Process<PlayerStatsDataRecord>([Path.Combine(queryFilesPath, "player_stats_data.flux")], x => x.Single()));

writersCache.Values.ToList().ForEach(w => w.Close());

IAsyncEnumerable<Task> EnumerateResults<T>(IEnumerable<string> queryFiles, Func<IEnumerable<T>, T> aggregate) where T : BaseRecord, new() =>
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
        .Select(query =>
            (
                query.Name,
                Results: query.Read(query.Query),
                Writer: getWriter(Path.Combine(resultFilesPath, query.Name + ".json"))))
        .Select(async records =>
            await JsonSerializer.SerializeAsync(
                records.Writer.BaseStream,
                await records
                    .Results
                    .Where(rec => !string.IsNullOrEmpty(rec.SteamID))
                    .GroupBy(x => x.SteamID)
                    .ToDictionaryAsync(rec => rec.Key!, rec => aggregate(rec.ToEnumerable()))));

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