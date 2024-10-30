using CounterStrikeSharp.API.Core;

namespace exportevents;

public static class Transform
{
    public static eventbuffer_contract.Types.EventPlayerDeath AsSerializeable(EventPlayerDeath e) =>
        new (
            e.GetType().Name,
            AsSerializeable(e.Userid),
            AsSerializeable(e.Attacker),
            AsSerializeable(e.Assister),
            e.Headshot,
            e.Weapon
        );

    public static eventbuffer_contract.Types.EventPlayerHurt AsSerializeable(EventPlayerHurt e) =>
        new (
            e.GetType().Name,
            AsSerializeable(e.Userid),
            AsSerializeable(e.Attacker),
            e.Weapon,
            e.DmgHealth,
            e.DmgArmor,
            HitgroupToString(e.Hitgroup)
        );

    public static eventbuffer_contract.Types.EventRoundMvp AsSerializeable(EventRoundMvp e) =>
        new (
            e.GetType().Name,
            AsSerializeable(e.Userid)!,
            e.Reason
        );

    // See https://github.com/roflmuffin/CounterStrikeSharp/tree/5c9d38b2b006e7edf544bb8f185acb4bd5fb6722/managed/CounterStrikeSharp.API/Core/Schema/Enums
    // and https://github.com/ValveSoftware/source-sdk-2013
    private static string HitgroupToString(int hitgroup) => 
        Enum.GetName((HitGroup_t)hitgroup) ?? string.Empty;

    private static eventbuffer_contract.Types.PlayerController? AsSerializeable(CCSPlayerController? e) =>
        e == null
            ? null 
            : e.IsValid 
                ? new(e.PlayerName, e.IsBot)
                : null;
}
