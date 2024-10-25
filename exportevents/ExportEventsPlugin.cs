using System.Threading.Channels;
using CounterStrikeSharp.API.Core;
using eventbuffer_contract;
using streaming;

namespace exportevents;

public class ExportEventsPlugin : BasePlugin
{
    private Channel<eventbuffer_contract.Types.EventPlayerDeath> channel;
    private Task channelTask;

    public override string ModuleName => nameof(ExportEventsPlugin);

    public override string ModuleVersion => "0.0.2";

    public override void Load(bool hotReload)
    {
        // Event handlers are BLOCKING!.
        // Delay of 1 sec stops a game frame for 1 sec.
        // Doing minimal work in the event handler is a performance goal.

        this.channel = Channel.CreateUnbounded<eventbuffer_contract.Types.EventPlayerDeath>(
            new UnboundedChannelOptions
            {
                SingleReader = true
            }
        );

        var appendPlayerDeath = EventBufferFactory.GetAppendPlayerDeath();
        var cancellationToken = new CancellationTokenSource().Token;
        this.channelTask = HandleAll(appendPlayerDeath);

        RegisterEventHandler((GameEventHandler<EventPlayerDeath>)((e, info) =>
        {
            channel.Writer.TryWrite(AsSerializeable(e));
            return HookResult.Continue;
        }));
    }

    private async Task HandleAll(EventBufferContract<eventbuffer_contract.Types.EventPlayerDeath>.Append appendPlayerDeath)
    {
        var asyncEnumerable = this
                            .channel
                            .Reader
                            .ReadAllAsync();

        await foreach(var t in asyncEnumerable){
            await appendPlayerDeath(t);
        }
    }

    public override void Unload(bool hotReload)
    {
        this.channel.Writer.Complete();
        Console.WriteLine("Channel complete");
        this.channelTask.Wait();
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