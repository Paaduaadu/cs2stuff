using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using streaming;

var influxHost = args.ElementAtOrDefault(0) ?? "http://influxdb2:8086";
var queryFilesPath = args.ElementAtOrDefault(1) ?? "/App/Queries";
var resultFilesPath = args.ElementAtOrDefault(2) ?? "/App/Results";

var token = "HOE7FFYwTNTAE2zl3ddOqw2XomadVRPNAtp8vx--Bm9MdgzLUkfeqDfDkliYdcUlhHRyb-w-qXnRFqgaQi957A==";

var read = Influx
    .GetRead<Record>(token, influxHost);

var cache = new Dictionary<string, StreamWriter>();


Func<string, StreamWriter> getWriter = Memoize(path => 
    new StreamWriter(new FileStream(path, FileMode.Create)), cache);


Func<string, T> Memoize<T>(Func<string, T> value, Dictionary<string, T> cache)
{
    return key => {
        if (!cache.TryGetValue(key, out T result))
        {
            result = value(key);
            cache.Add(key, result);
        }

        return result;
    };
}

await foreach(var t in Directory
    .EnumerateFiles(queryFilesPath)
    .Select(file => (Name: Path.GetFileNameWithoutExtension(file), Query: File.ReadAllText(file)))
    .Select(query => (query.Name,  Results: read(query.Query)))
    .ToAsyncEnumerable()
    .Select(records => 
        records
            .Results
            .ExtractTransformLoad(
                rec => JsonSerializer.Serialize(rec),
                json => getWriter(Path.Combine(resultFilesPath, records.Name + ".json")).WriteLineAsync(json)))) {
    await t;
};

cache.Values.ToList().ForEach(w => w.Close());

class Record {
    [Column("value")] public double Value { get; set; }

    [Column("time")] public DateTime Time { get; set; }

    [Column("Attacker")] public string? Attacker { get; set; }

    [Column("Player")] public string? Player { get; set; }
};