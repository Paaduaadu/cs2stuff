using System.Threading.Channels;
using CounterStrikeSharp.API.Core;
using eventbuffer_contract;
using streaming;

namespace exportevents;

public class ExportEventsPlugin : BasePlugin
{
    private Channel<eventbuffer_contract.Types.EventPlayerDeath> playerDeathChannel;
    private Task playerDeathTask;

    public override string ModuleName => nameof(ExportEventsPlugin);

    public override string ModuleVersion => "0.0.2";

    public override void Load(bool hotReload)
    {
        // Event handlers are BLOCKING!.
        // Delay of 1 sec stops a game frame for 1 sec.
        // Doing minimal work in the event handler is a performance goal.

        this.playerDeathChannel = Channel.CreateUnbounded<eventbuffer_contract.Types.EventPlayerDeath>(
            new UnboundedChannelOptions
            {
                SingleReader = true
            }
        );

        var appendPlayerDeath = EventBufferFactory.GetAppendPlayerDeath();
        var cancellationToken = new CancellationTokenSource().Token;

        this.playerDeathTask = this
            .playerDeathChannel
            .Reader
            .ReadAllAsync()
            .ExtractTransformLoad(
                gameEvent => gameEvent,
                gameEvent => appendPlayerDeath(gameEvent));

        RegisterEventHandler((GameEventHandler<EventPlayerDeath>)((e, info) =>
        {
            playerDeathChannel.Writer.TryWrite(AsSerializeable(e));
            return HookResult.Continue;
        }));
    }

    public override void Unload(bool hotReload)
    {
        this.playerDeathChannel.Writer.Complete();
        Console.WriteLine("Channel complete");
        this.playerDeathTask.Wait();
    }

    private static eventbuffer_contract.Types.EventPlayerDeath AsSerializeable(EventPlayerDeath e) =>
        new (
            e.EventName,
            AsSerializeable(e.Userid),
            AsSerializeable(e.Attacker),
            AsSerializeable(e.Assister),
            e.Headshot,
            e.Weapon
        );

    private static eventbuffer_contract.Types.PlayerController? AsSerializeable(CCSPlayerController? e) =>
        e == null
            ? null 
            : e.IsValid 
                ? new(e.PlayerName, e.IsBot)
                : null;
}