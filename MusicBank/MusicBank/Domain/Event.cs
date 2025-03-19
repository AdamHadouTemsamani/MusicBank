using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicBank.Domain;

[Table("Event")]
public class Event
{
    [Key]
    [Column("event_id")]
    public int EventId { get; set; }

    [Column("event_name", TypeName = "TEXT")]
    public string EventName { get; set; } = null!;

    [Column("event_venue", TypeName = "TEXT")]
    public string EventVenue { get; set; } = null!;

    [Column("event_date")]
    public DateTime EventDate { get; set; }
}