using eventbuffer_contract.Types;

namespace eventbuffer_contract.Events;

public record struct EventBombPlanted(
    string EventName,
    PlayerController Player,
    int Site,
    string SiteName);