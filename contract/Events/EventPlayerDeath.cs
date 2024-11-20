using contract.Types;
using eventbuffer_contract.Types;

namespace eventbuffer_contract.Events;

public record struct EventPlayerDeath(
    string EventName,
    PlayerController? Player,
    PlayerController? Attacker,
    PlayerController? Assister,
    bool Headshot,
    string Weapon) : IHasGameMetadata
{
    public GameMetadata Metadata { get; set; }
}
