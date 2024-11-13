using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace publishmetrics.Types
{
    public class PlayersDataRecord : BaseRecord
    {
        [JsonPropertyName("name")]
        [Column(nameof(Name))] public string Name { get; set; } = string.Empty;
        [JsonPropertyName("avatar")]
        public string Avatar => "/avatars/" + Name.ToLowerInvariant() + ".png";
    }
}