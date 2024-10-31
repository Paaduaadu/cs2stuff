using eventbuffer_contract.Types;
using InfluxDB.Client.Writes;

namespace eventbuffer;

public static class Transform
{
    public static PointData ToMetric(this EventPlayerDeath x) => 
        PointData
            .Measurement(x.GetType().Name)
            .ToTags("Attacker", x.Attacker)
            .ToTags("Assister", x.Assister)
            .ToTags("Player", x.Player)
            .Tag("Headshot", x.Headshot.ToString())
            .Tag("Weapon", x.Weapon != null ? x.Weapon.ToString() : string.Empty)
            .Field("Count", 1);

    public static PointData ToMetric(this EventPlayerHurt x) => 
        PointData
            .Measurement(x.GetType().Name)
            .ToTags("Attacker", x.Attacker)
            .ToTags("Player", x.Player)
            .Tag("Weapon", x.Weapon != null ? x.Weapon.ToString() : string.Empty)
            .Tag("Hitgroup", x.Hitgroup)
            .Field(nameof(x.DmgHealth), x.DmgHealth)
            .Field(nameof(x.DmgArmor), x.DmgArmor);

    public static PointData ToMetric(this EventRoundMvp x) => 
        PointData
            .Measurement(x.GetType().Name)
            .ToTags("Player", x.Player)
            .Tag("Reason", x.Reason.ToString())
            .Tag("ReasonString", x.ReasonString)
            .Field("Value", x.Value);

    public static PointData ToMetric(this EventBombPlanted x) => 
        PointData
            .Measurement(x.GetType().Name)
            .ToTags("Player", x.Player)
            .Tag("Site", x.Site.ToString())
            .Tag("SiteString", x.SiteName)
            .Field("Count", 1);

    public static PointData ToTags(this PointData pd, string label, PlayerController? pc) =>
        pc == null
            ? pd
            : pd
                .Tag(label, pc.PlayerName)
                .Tag($"{label}.IsBot", pc.IsBot.ToString());

}
