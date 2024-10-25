using System.Threading.Channels;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory;
using streaming;

namespace exportevents;

public class ExportEventsPlugin : BasePlugin
{
    private Channel<eventbuffer_contract.Types.EventPlayerDeath> playerDeathChannel;
    private Channel<eventbuffer_contract.Types.EventPlayerHurt> playerHurtChannel;

    private Task playerDeathTask;
    private Task playerHurtTask;

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

        this.playerHurtChannel = Channel.CreateUnbounded<eventbuffer_contract.Types.EventPlayerHurt>(
            new UnboundedChannelOptions
            {
                SingleReader = true
            }
        );

        var appendPlayerDeath = EventBufferFactory.GetAppendEvent<eventbuffer_contract.Types.EventPlayerDeath>();
        var appendPlayerHurt = EventBufferFactory.GetAppendEvent<eventbuffer_contract.Types.EventPlayerHurt>();
        var cancellationToken = new CancellationTokenSource().Token;

        this.playerDeathTask = this
            .playerDeathChannel
            .Reader
            .ReadAllAsync()
            .ExtractTransformLoad(
                gameEvent => gameEvent,
                gameEvent => appendPlayerDeath(gameEvent));

        this.playerHurtTask = this
            .playerHurtChannel
            .Reader
            .ReadAllAsync()
            .ExtractTransformLoad(
                gameEvent => gameEvent,
                gameEvent => appendPlayerHurt(gameEvent));

        RegisterEventHandler((GameEventHandler<EventPlayerDeath>)((e, info) =>
        {
            playerDeathChannel.Writer.TryWrite(AsSerializeable(e));
            return HookResult.Continue;
        }));

        RegisterEventHandler((GameEventHandler<EventPlayerHurt>)((e, info) =>
        {
            playerHurtChannel.Writer.TryWrite(AsSerializeable(e));
            return HookResult.Continue;
        }));
    }

    public override void Unload(bool hotReload)
    {
        this.playerDeathChannel.Writer.Complete();
        this.playerHurtChannel.Writer.Complete();
        Console.WriteLine("Channel complete");
        this.playerDeathTask.Wait();
        this.playerHurtTask.Wait();
    }

    private static eventbuffer_contract.Types.EventPlayerDeath AsSerializeable(EventPlayerDeath e) =>
        new (
            e.GetType().Name,
            AsSerializeable(e.Userid),
            AsSerializeable(e.Attacker),
            AsSerializeable(e.Assister),
            e.Headshot,
            e.Weapon
        );

    private eventbuffer_contract.Types.EventPlayerHurt AsSerializeable(EventPlayerHurt e) =>
        new (
            e.GetType().Name,
            AsSerializeable(e.Userid),
            AsSerializeable(e.Attacker),
            e.Weapon,
            e.DmgHealth,
            e.DmgArmor,
            HitgroupToString(e.Hitgroup)
        );

    private static string HitgroupToString(int hitgroup) => 
        Enum.GetName((HitGroup_t)hitgroup) ?? string.Empty;

    private static eventbuffer_contract.Types.PlayerController? AsSerializeable(CCSPlayerController? e) =>
        e == null
            ? null 
            : e.IsValid 
                ? new(e.PlayerName, e.IsBot)
                : null;
}