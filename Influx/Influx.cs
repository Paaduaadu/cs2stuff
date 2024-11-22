using InfluxDB.Client;
using InfluxDB.Client.Writes;

public static class Influx {
    public static Func<PointData, Task> GetWrite(string token)
    {
        var write = new InfluxDBClient("http://influxdb2:8086", token)
            .GetWriteApiAsync();
        return metric => write.WritePointAsync(metric, "CS2", "Wolves");
    }

    public static Func<string, IAsyncEnumerable<T>> GetRead<T>(string token, string url)
    {
        var queryApi = new InfluxDBClient(url, token)
            .GetQueryApi();
        return query => queryApi.QueryAsyncEnumerable<T>(query, "Wolves");
    }
}