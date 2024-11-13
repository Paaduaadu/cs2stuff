using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace publishmetrics.Types
{
    public class EventPlayerDeathRecord
    {
        [JsonIgnore]
        [Column("SteamID")] public string? SteamID { get; set; }
        [Column(nameof(Player))] public string? Player { get; set; }
        [JsonPropertyName("total_mvps")]
        [Column(nameof(Mvp))] public int Mvp { get; set; }
        [JsonPropertyName("total_assists")]
        [Column(nameof(Assists))] public int Assists { get; set; }
        [JsonPropertyName("total_kills")]
        [Column(nameof(Kills))] public int Kills { get; set; }      
        [JsonPropertyName("total_deaths")]
        [Column(nameof(Deaths))] public int Deaths { get; set; }
        [JsonPropertyName("total_kdr")]
        [Column(nameof(KDR))] public float KDR { get; set; }  
        [JsonPropertyName("total_damage")]
        [Column(nameof(Damage))] public int Damage { get; set; }  
        [JsonPropertyName("total_headshots")]
        [Column(nameof(Headshots))] public int Headshots { get; set; }

        [JsonPropertyName("weapons")]
        public Dictionary<string, Weapon> Weapons { get; set; } =  new Dictionary<string, Weapon>{
            {"Not-Implemented", new Weapon()}
  
        };

        public class Weapon
        {
            [JsonPropertyName("kills")]
            public int Kills { get; set; } = 0;
            [JsonPropertyName("headshots")]
            public int Headshots { get; set; } = 0;
            [JsonPropertyName("damage")]
            public int Damage { get; set; } = 0;
        }
    };
}