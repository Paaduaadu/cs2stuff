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

    private static string HitgroupToString(int hitgroup) => 
        Enum.GetName((HitGroup_t)hitgroup) ?? string.Empty;

    private static eventbuffer_contract.Types.PlayerController? AsSerializeable(CCSPlayerController? e) =>
        e == null
            ? null 
            : e.IsValid 
                ? new(e.PlayerName, e.IsBot)
                : null;
}
