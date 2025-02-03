using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Events;
using eventbuffer_contract.Events;
using exportevents.Events;
using streaming;

namespace exportevents.Core;

public static class PluginExtensions
{
    public static (Task ReadChannelTask, Action<Exception?> StopWriting) ListenAndPublish<TGameEvent, TTargetType>(
        this BasePlugin plugin,
        Func<TGameEvent, TTargetType> transform,
        string redisHostName)
        where TGameEvent : GameEvent
        where TTargetType : struct, IHasGameMetadata
    {
        var getGameRules = Memoize(() => 
            Utilities.FindAllEntitiesByDesignerName<CBaseEntity>("cs_gamerules").Single().As<CCSGameRulesProxy>().GameRules!);

        Console.WriteLine("Starting to publish:" + typeof(TTargetType).Name);
        
        return OffloadEventsAsync.ListenEventAndPublish<TGameEvent, TTargetType>(handleEvent =>
            plugin.RegisterEventHandler<TGameEvent>((gameEvent, _) =>
            {
                handleEvent(gameEvent);
                return HookResult.Continue;
            }, HookMode.Post),
            gameEvent =>
                transform(gameEvent) with { 
                    Metadata = Transform.AsSerializeable(getGameRules())},
            EventBufferFactory.GetAppendEvent<TTargetType>(redisHostName));
    }

    public static Func<T?> Memoize<T>(Func<T> f)
        {   
            bool isMemoized = false;
            T? result = default;
            return () =>
                {
                    if (!isMemoized)
                    {
                        result = f();
                        isMemoized = true;
                    }
                    return result;
                };
        }

}
