using System.ComponentModel.DataAnnotations.Schema;

namespace publishmetrics.Types
{
    public class EventPlayerDeathRecord
    {
        [Column("value")] public double Value { get; set; }

        [Column("time")] public DateTime Time { get; set; }

        [Column("Attacker")] public string? Attacker { get; set; }
    };
}