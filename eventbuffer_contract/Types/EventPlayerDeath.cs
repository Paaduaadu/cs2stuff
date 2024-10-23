namespace eventbuffer_contract.Types;

public record struct EventPlayerDeath(
    string EventName,
    PlayerController? Player,
    PlayerController? Attacker,
    PlayerController? Assister, 
    bool Headshot, 
    string Weapon);
