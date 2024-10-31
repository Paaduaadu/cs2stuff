using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Events;

namespace exportevents;

public class ExportEventsPlugin : BasePlugin
{
    private (Task ReadChannelTask, Action<Exception?> StopWriting)[] readEventChannelTasks = [];

    public override string ModuleName => nameof(ExportEventsPlugin);

    public override string ModuleVersion => "0.0.5";

    public override void Load(bool hotReload)
    {
        // Just to indicate in console that "it tried to start and what version it is. Useful to see if the new code was actually loaded".
        Console.WriteLine("ExportEvents:" + ModuleVersion);

        // Event handlers are BLOCKING!.
        // Delay of 1 sec stops a game frame for 1 sec.
        // Doing minimal work in the event handler is a performance goal.


        // Add into this collection new event handlers when needed.
        // This is 90% of the cases the only code block that must be changed in this project.
        readEventChannelTasks = [
            // Player is killed.
            ListenAndPublish<
                // Type of the CSSharp library event. See the cssharp docs and library code for what is availaböe.
                EventPlayerDeath,
                // Type that gets published as metric. These are custom created code objects that can be serialized (transported as bytes over the wire).
                eventbuffer_contract.Types.EventPlayerDeath>(
                    // How to convert between the above 2. We convert to select only the fields we need in the format we need.
                    // The csssharp types are not serializeable (cannot be transported over the wire as bytes).
                    // Also, even if they would be, it would be wasteful to do transport everything. 
                    Transform.AsSerializeable),

            // Damage done to a player.      
            ListenAndPublish<EventPlayerHurt, eventbuffer_contract.Types.EventPlayerHurt>(Transform.AsSerializeable),
            ListenAndPublish<EventRoundMvp, eventbuffer_contract.Types.EventRoundMvp>(Transform.AsSerializeable),
            ListenAndPublish<EventBombPlanted, eventbuffer_contract.Types.EventBombPlanted>(Transform.AsSerializeable)
        ];
    }

    public override void Unload(bool hotReload)
    {
        foreach(var action in readEventChannelTasks.Select(x => x.StopWriting)) {
            action(null);
        }

        Console.WriteLine("Channels complete. Draining events.");

        Task.WaitAll([.. readEventChannelTasks.Select(x => x.ReadChannelTask)]);

        Console.WriteLine("Events drained.");
    }

    private (Task ReadChannelTask, Action<Exception?> StopWriting) ListenAndPublish<TGameEvent, T>(
        Func<TGameEvent, T> transform)
        where TGameEvent : GameEvent
        where T : struct
    {
        Console.WriteLine("Starting to publish:" + typeof(T).Name);
        return OffloadEventsAsync.ListenEventAndPublish(handleEvent =>
            RegisterEventHandler<TGameEvent>((x, _) =>
            {
                handleEvent(x);
                return HookResult.Continue;
            }, HookMode.Post), transform);
    }
}