using eventbuffer;
using eventbuffer_contract.Types;
using InfluxDB.Client;
using InfluxDB.Client.Writes;
using streaming;

Console.WriteLine("0.0.2");

using var client = new InfluxDBClient("http://influxdb2:8086", Extensions.ReadSecret("INFLUXDB_TOKEN_FILE"));
var write = client.GetWriteApiAsync();
Func<PointData, Task> writeMetric = metric => write.WritePointAsync(metric,  "CS2", "Wolves");

var tasks = new [] {
    Process( 
        await EventBufferFactory.GetReadEvent<EventPlayerDeath>(), 
        Transform.ToMetric),
    Process(
        await EventBufferFactory.GetReadEvent<EventPlayerHurt>(),
        Transform.ToMetric)
};

await Task.WhenAll(tasks);

Task Process<T>(
    Func<Task<T>> readOneGameEvent,
    Func<T, PointData> toMetric) where T : struct => 
        Extensions.Process<T>(
            writeMetric, 
            readOneGameEvent, 
            toMetric);

public static class Extensions
{
    public static Task Process<T>(
        Func<PointData, Task> writeMetric,
        Func<Task<T>> readOneGameEvent,
        Func<T, PointData> toMetric) where T : struct => 
        EventBufferFactory
            .ReadToEnd(readOneGameEvent)
            .Where(gameEvent => !gameEvent.Equals(default(T)))
            .ExtractTransformLoad(
                transform: toMetric,
            load: writeMetric);

    public static string ReadSecret(string envVariableName) => 
        File.ReadAllText(Environment.GetEnvironmentVariable(envVariableName)!).Trim();
}

