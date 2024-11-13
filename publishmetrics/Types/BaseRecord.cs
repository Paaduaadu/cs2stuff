using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace publishmetrics.Types;

public class BaseRecord
{
    [JsonIgnore]
    [Column("SteamID")] public string? SteamID { get; set; }
}
