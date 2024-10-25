using eventbuffer_contract.Types;
using InfluxDB.Client;
using InfluxDB.Client.Writes;
using streaming;

Console.WriteLine("0.0.1");

using var client = new InfluxDBClient("http://influxdb2:8086", Extensions.ReadSecret("INFLUXDB_TOKEN_FILE"));
var write = client.GetWriteApiAsync();
Func<PointData, Task> writeMetric = metric => write.WritePointAsync(metric,  "CS2", "Wolves");

await EventBufferFactory
    .ReadToEnd(await EventBufferFactory.GetReadEventPlayerDeath())
    .Where(gameEvent => !gameEvent.Equals(default))
    .ExtractTransformLoad(
        transform: Extensions.ToMetric,
        load: writeMetric);

public static class Extensions
{
    public static string ReadSecret(string envVariableName) => 
        File.ReadAllText(Environment.GetEnvironmentVariable(envVariableName)!).Trim();

    public static PointData ToMetric(this EventPlayerDeath x) => 
        PointData
            .Measurement("EventPlayerDeath")
            .ToTags("Attacker", x.Attacker)
            .ToTags("Assister", x.Assister)
            .ToTags("Player", x.Player)
            .Tag("Headshot", x.Headshot.ToString())
            .Tag("Weapon", x.Weapon != null ? x.Weapon.ToString() : string.Empty)
            .Field("Count", 1);

    public static PointData ToTags(this PointData pd, string label, PlayerController? pc) =>
        pc == null
            ? pd
            : pd
                .Tag(label, pc.PlayerName)
                .Tag($"{label}.IsBot", pc.IsBot.ToString());

    
    public static async Task ExtractTransformLoad<TIn, TOut>(
        this IAsyncEnumerable<TIn> extract, 
        Func<TIn, TOut> transform,
        Func<TOut, Task> load)
    {
        await foreach(var x in extract)
        {
            await load(transform(x));
        }
    }
}

