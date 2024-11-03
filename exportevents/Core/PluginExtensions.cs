using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Events;
using streaming;

namespace exportevents.Core;

public static class PluginExtensions
{
    public static (Task ReadChannelTask, Action<Exception?> StopWriting) ListenAndPublish<TGameEvent, TTargetType>(
        this BasePlugin plugin,
        Func<TGameEvent, TTargetType> transform)
        where TGameEvent : GameEvent
        where TTargetType : struct
    {
        Console.WriteLine("Starting to publish:" + typeof(TTargetType).Name);
        return OffloadEventsAsync.ListenEventAndPublish(handleEvent =>
            plugin.RegisterEventHandler<TGameEvent>((gameEvent, _) =>
            {
                handleEvent(gameEvent);
                return HookResult.Continue;
            }, HookMode.Post),
            transform,
            EventBufferFactory.GetAppendEvent<TTargetType>());
    }
}