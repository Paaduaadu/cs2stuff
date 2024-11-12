using CounterStrikeSharp.API.Core;
using exportevents.Events;

namespace exportevents;

public class ExportEventsPlugin : BasePlugin
{
    private IReadOnlyCollection<(Task ReadChannelTask, Action<Exception?> StopWriting)> readEventChannelTasks = [];

    public override string ModuleName => nameof(ExportEventsPlugin);

    public override string ModuleVersion => "0.0.7";

    public override void Load(bool hotReload)
    {
        // Just to indicate in console that "it tried to start and what version it is. Useful to see if the new code was actually loaded".
        Console.WriteLine("ExportEvents:" + ModuleVersion);

        // To add new event listeners go into this method and copy what has been done, more details inside.
        readEventChannelTasks = EvendHandlers.Create(this);
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