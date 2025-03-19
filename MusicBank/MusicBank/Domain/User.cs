using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicBank.Domain;

[Table("User")] 
public partial class User
{
    [Key]
    [Column("user_id")]
    public int UserId { get; set; }
    
    [Column("name", TypeName = "TEXT")]
    public string Name { get; set; } = null!;

    [Column("email", TypeName = "TEXT")]
    public string Email { get; set; } = null!;

    [Column("phone_number", TypeName = "TEXT")]
    public string PhoneNumber { get; set; } = null!;

    [Column("ticket-reservations")]
    public ICollection<TicketReservation> TicketReservations { get; set; } = new List<TicketReservation>();

}
