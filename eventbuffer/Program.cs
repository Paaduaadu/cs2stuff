using eventbuffer_contract.Types;
using InfluxDB.Client;
using InfluxDB.Client.Writes;
using streaming;

Console.WriteLine("0.0.1");

using var client = new InfluxDBClient("http://influxdb2:8086", ReadSecret("INFLUXDB_TOKEN_FILE"));
var write = client.GetWriteApiAsync();
var readOnePlayerDeath = await EventBufferFactory.GetReadEventPlayerDeath();

var seq = EventBufferFactory
        .ReadToEnd(readOnePlayerDeath)
        .Where(x => !x.Equals(default));

await HandleAll(write, seq);

static string ReadSecret(string envVariableName) => 
    File.ReadAllText(Environment.GetEnvironmentVariable(envVariableName)!).Trim();

static async Task HandleAll(WriteApiAsync write, IAsyncEnumerable<EventPlayerDeath> seq)
    {
        await foreach(var x in seq){
            await write
                .WritePointAsync(
                    PointData
                        .Measurement("EventPlayerDeath")
                        .ToTags("Attacker", x.Attacker)
                        .ToTags("Assister", x.Assister)
                        .ToTags("Player", x.Player)
                        .Tag("Headshot", x.Headshot.ToString())
                        .Tag("Weapon", x.Weapon.ToString())
                        .Field("Count", 1), "CS2", "Wolves");
        }
    }

public static class InfluxDbExtensions
{
    public static PointData ToTags(this PointData pd, string label, PlayerController? pc) =>
        pc == null
            ? pd
            : pd
                .Tag(label, pc.PlayerName)
                .Tag($"{label}.IsBot", pc.IsBot.ToString());
}