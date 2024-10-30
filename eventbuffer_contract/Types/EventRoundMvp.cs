namespace eventbuffer_contract.Types;

public record struct EventRoundMvp(
    string EventName, 
    PlayerController Player, 
    int Reason);

