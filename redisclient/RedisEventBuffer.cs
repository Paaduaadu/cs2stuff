using System.Text.Json;
using StackExchange.Redis;
namespace eventbuffer_redis;

public static class RedisEventBuffer
{
    private const string GroupName = "loader";
    private const string ConsumerName = GroupName + "-0";

    public static async Task<Func<Task<T>>> GetReadOne<T>(string streamName, string redisHostName) where T : struct
    {
        var db = GetDatabase(redisHostName);

        await EnsureConsumerGroup(db, streamName, GroupName);

        return () => ReadOne<T>(db, streamName);
    }

    public static Func<T, Task> GetAppendOne<T>(string streamName, string redisHostName)
    {
        var db = GetDatabase(redisHostName);
        
        return e =>
            db.StreamAddAsync(
                streamName,
                [new NameValueEntry("SerializedEvent", JsonSerializer.Serialize(e))]);
    }

    private static async Task<T> ReadOne<T>(IDatabase db, string streamName) where T : struct
    {
        var result = await db.StreamReadGroupAsync(streamName, GroupName, ConsumerName, ">", 1);

        if (!HasSome(result)) return default;

        var firstResult = result.Single();

        var value = JsonSerializer.Deserialize<T>(firstResult.Values.Single().Value.ToString())!;

        await db.StreamAcknowledgeAsync(streamName, GroupName, firstResult.Id);

        return value;
    }

    private static bool HasSome(StreamEntry[] result) => result.Length != 0;

    private static async Task EnsureConsumerGroup(IDatabase db, string streamName, string groupName)
    {
        if (!await db.KeyExistsAsync(streamName) ||
            (await db.StreamGroupInfoAsync(streamName)).All(x => x.Name != groupName))
        {
            await db.StreamCreateConsumerGroupAsync(streamName, groupName, "0-0", true);
        }
    }

    private static IDatabase GetDatabase(string redisHostName) =>
        ConnectionMultiplexer
            .Connect(redisHostName)
            .GetDatabase();
}
