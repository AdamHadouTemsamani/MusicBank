using System.Runtime.InteropServices.JavaScript;

namespace MusicBank.Models;

public class TicketReservationDTO
{
    public int Id { get; set; }
    public UserDTO User { get; set; }
    public EventDTO Event { get; set; }
    public DateTime ReservationDate { get; set; }
    
}