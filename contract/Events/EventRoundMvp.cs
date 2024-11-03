using eventbuffer_contract.Types;

namespace eventbuffer_contract.Events;

public record struct EventRoundMvp(
    string EventName, 
    PlayerController Player, 
    int Reason,
    string ReasonString,
    long Value);
