using CounterStrikeSharp.API.Core;

namespace exportevents;

public class ExportEventsPlugin : BasePlugin
{
    private (Task ReadChannelTask, Action<Exception?> StopWriting)[] readEventChannelTasks = [];

    public override string ModuleName => nameof(ExportEventsPlugin);

    public override string ModuleVersion => "0.0.2";

    public override void Load(bool hotReload)
    {
        // Event handlers are BLOCKING!.
        // Delay of 1 sec stops a game frame for 1 sec.
        // Doing minimal work in the event handler is a performance goal.

        readEventChannelTasks = [
            // Player is killed.
            OffloadEventsAsync.ListenEventAndPublish<
                EventPlayerDeath, 
                eventbuffer_contract.Types.EventPlayerDeath>(this, Transform.AsSerializeable),
            // Damage done to a player.      
            OffloadEventsAsync.ListenEventAndPublish<
                EventPlayerHurt, 
                eventbuffer_contract.Types.EventPlayerHurt>(this, Transform.AsSerializeable)
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
}