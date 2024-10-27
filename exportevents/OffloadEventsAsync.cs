using System.Threading.Channels;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Events;
using streaming;
using static CounterStrikeSharp.API.Core.BasePlugin;

namespace exportevents;

public static class OffloadEventsAsync
{
    public static (Task ReadChannelTask, Action<Exception?> StopWriting) ListenEventAndPublish<TGameEvent, TStatsEvent>(BasePlugin plugin, Func<TGameEvent, TStatsEvent> transform)
        where TStatsEvent : struct 
        where TGameEvent : GameEvent
    {
        var channel = CreateChannel<TStatsEvent>();

        plugin.RegisterEventHandler((GameEventHandler<TGameEvent>)((e, info) =>
        {
            channel.Writer.TryWrite(transform(e));
            return HookResult.Continue;
        }));

        return (
            ReadChannel(
                channel,
                EventBufferFactory.GetAppendEvent<TStatsEvent>()),
            channel.Writer.Complete
        );
    }

    private static Channel<T> CreateChannel<T>()
    {
        var channel = Channel.CreateUnbounded<T>(
            new UnboundedChannelOptions
            {
                SingleReader = true
            }
        );
        return channel;
    }

    private static Task ReadChannel<T>(
        Channel<T> channel,
        Func<T, Task> load) => 
        channel
            .Reader
            .ReadAllAsync()
            .ExtractTransformLoad(
                gameEvent => gameEvent,
                load);
}
