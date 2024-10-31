namespace eventbuffer_contract.Types;

public record struct EventBombPlanted(
    string EventName, 
    PlayerController Player,
    int Site,
    string SiteName);