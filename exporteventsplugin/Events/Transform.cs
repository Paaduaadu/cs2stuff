using contract.Types;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

namespace exportevents.Events;

public static class Transform
{
    public static eventbuffer_contract.Events.EventPlayerDeath AsSerializeable(EventPlayerDeath e) =>
        new(
            e.GetType().Name,
            AsSerializeable(e.Userid),
            AsSerializeable(e.Attacker),
            AsSerializeable(e.Assister),
            e.Headshot,
            e.Weapon
        );

    public static eventbuffer_contract.Events.EventPlayerHurt AsSerializeable(EventPlayerHurt e) =>
        new(
            e.GetType().Name,
            AsSerializeable(e.Userid),
            AsSerializeable(e.Attacker),
            e.Weapon,
            e.DmgHealth,
            e.DmgArmor,
            HitgroupToString(e.Hitgroup)
        );

    public static eventbuffer_contract.Events.EventRoundMvp AsSerializeable(EventRoundMvp e) =>
        new(
            e.GetType().Name,
            AsSerializeable(e.Userid)!,
            e.Reason,
            Enum.GetName((CSMvpReason_t)e.Reason) ?? string.Empty,
            e.Value
        );

    public static eventbuffer_contract.Events.EventBombPlanted AsSerializeable(EventBombPlanted e) =>
        new(
            e.GetType().Name,
            AsSerializeable(e.Userid)!,
            e.Site,
            //A = 425 B = 426
            e.Site == 425 ? "A" : "B"
        );

    // See https://github.com/roflmuffin/CounterStrikeSharp/tree/5c9d38b2b006e7edf544bb8f185acb4bd5fb6722/managed/CounterStrikeSharp.API/Core/Schema/Enums
    // and https://github.com/ValveSoftware/source-sdk-2013
    private static string HitgroupToString(int hitgroup) =>
        Enum.GetName((HitGroup_t)hitgroup) ?? string.Empty;

    private static eventbuffer_contract.Types.PlayerController? AsSerializeable(CCSPlayerController? e) =>
        e == null
            ? null
            : e.IsValid
                ? new(e.PlayerName, e.IsBot, e.AuthorizedSteamID == null ? string.Empty: e.AuthorizedSteamID.SteamId2)
                : null;

    internal static GameMetadata AsSerializeable(CCSGameRules? x) =>
        x == null ? new GameMetadata() : new (x.WarmupPeriod, x.HasMatchStarted, Server.MapName);

    enum CSMvpReason_t
    {
        CSMVP_UNDEFINED = 0,
        CSMVP_ELIMINATION,
        CSMVP_BOMBPLANT,
        CSMVP_BOMBDEFUSE,
        CSMVP_HOSTAGERESCUE,
    };
}
