using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace publishmetrics.Types
{
    public class EventPlayerDeathRecord
    {
        [Column("SteamID")] public string SteamID { get; set; }
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
    };
}