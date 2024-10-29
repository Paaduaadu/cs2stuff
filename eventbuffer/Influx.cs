using InfluxDB.Client;
using InfluxDB.Client.Writes;
using streaming;

internal static class Influx {

    public async static Task<Task> ProcessEvent<T>(
        Func<T, PointData> toMetric, 
        Func<PointData, Task> writeMetric)
        where T : struct => 
            Extensions.Process(
                await EventBufferFactory.GetReadEvent<T>(),
                toMetric,
                writeMetric);

    public static Func<PointData, Task> GetWrite(string token)
    {
        var write = new InfluxDBClient("http://influxdb2:8086", token)
            .GetWriteApiAsync();
        return metric => write.WritePointAsync(metric, "CS2", "Wolves");
    }
}