using CounterStrikeSharp.API.Core;
using exportevents.Core;

namespace exportevents.Events;

public static class EvendHandlers
{
    /// <summary>
    /// Event handlers are BLOCKING!.
    /// Delay of 1 sec stops a game frame for 1 sec.
    /// Doing minimal work in the event handler is a performance goal.
    /// Add into this collection new event handlers when needed.
    /// This is 90% of the cases the only code block that must be changed in this project.
    /// </summary>
    /// <param name="plugin"></param>
    /// <returns></returns>
    public static IReadOnlyCollection<(Task ReadChannelTask, Action<Exception?> StopWriting)> Create(BasePlugin plugin, string redisHostName) =>
        [
            // Player is killed.
            plugin.ListenAndPublish<EventPlayerDeath, eventbuffer_contract.Events.EventPlayerDeath>(
                    // How to convert between the above 2. We convert to select only the fields we need in the format we need.
                    // The csssharp types are not serializeable (cannot be transported over the wire as bytes).
                    // Also, even if they would be, it would be wasteful to do transport everything. 
                    Transform.AsSerializeable,
                    redisHostName),

            // Damage done to a player.      
            plugin.ListenAndPublish<EventPlayerHurt, eventbuffer_contract.Events.EventPlayerHurt>(Transform.AsSerializeable, redisHostName),
            // Most Valuable Player and why.
            plugin.ListenAndPublish<EventRoundMvp, eventbuffer_contract.Events.EventRoundMvp>(Transform.AsSerializeable, redisHostName),
            // Who planted the bomb.
            plugin.ListenAndPublish<EventBombPlanted, eventbuffer_contract.Events.EventBombPlanted>(Transform.AsSerializeable, redisHostName)
        ];
}
