using System.Text.Json;
using publishmetrics.Core;
using publishmetrics.Types;
using streaming;

var influxHost = args.ElementAtOrDefault(0) ?? "http://influxdb2:8086";
var queryFilesPath = args.ElementAtOrDefault(1) ?? "/App/Queries";
var resultFilesPath = args.ElementAtOrDefault(2) ?? "/App/Results";

var read = Influx
    .GetRead<EventPlayerDeathRecord>(ConsoleExtensions.ReadSecret("INFLUXDB_TOKEN_FILE"), influxHost);

var cache = new Dictionary<string, StreamWriter>();

var getWriter = FunctionalExtensions.Memoize(path => 
    new StreamWriter(new FileStream(path, FileMode.Create)), cache);

await foreach(var t in Directory
    .EnumerateFiles(queryFilesPath)
    .Select(file => (Name: Path.GetFileNameWithoutExtension(file), Query: File.ReadAllText(file)))
    .ToAsyncEnumerable()
    .SelectAwait(async query => 
        (
            query.Name, 
            Results: await read(query.Query).AppendIf(new EventPlayerDeathRecord(), async e => !await e.AnyAsync()),
            Writer: getWriter(Path.Combine(resultFilesPath, query.Name + ".json"))))
    .Select(records => 
        records
            .Results
            .ExtractTransformLoad(
                rec => JsonSerializer.Serialize(rec),
                json => records.Writer.WriteLineAsync(json)))) {
    await t;
};

cache.Values.ToList().ForEach(w => w.Close());