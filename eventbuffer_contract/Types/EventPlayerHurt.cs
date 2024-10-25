namespace eventbuffer_contract.Types;

public record struct EventPlayerHurt(
    string EventName,
    PlayerController? Player,
    PlayerController? Attacker,
    string Weapon,
    int DmgHealth,
    int DmgArmor,
    string Hitgroup);
