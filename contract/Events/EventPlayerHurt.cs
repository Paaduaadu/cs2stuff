using eventbuffer_contract.Types;

namespace eventbuffer_contract.Events;

public record struct EventPlayerHurt(
    string EventName,
    PlayerController? Player,
    PlayerController? Attacker,
    string Weapon,
    int DmgHealth,
    int DmgArmor,
    string Hitgroup);
