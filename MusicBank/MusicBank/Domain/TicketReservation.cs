using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicBank.Domain;

[Table("TicketReservation")]
public partial class TicketReservation
{
    [Key]
    [Column("ticket_reservation_id")]
    public int TicketReservationId { get; set; }
    
    [ForeignKey("user_id")]
    public int UserId { get; set; }

    [ForeignKey("event_id")]
    public int EventId { get; set; }

    [Column("reservation_date")]
    public DateTime ReservationDate { get; set; }

    // Navigation properties
    public User? User { get; set; }
    public Event? Event { get; set; }
}