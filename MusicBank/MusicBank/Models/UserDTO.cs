namespace MusicBank.Models;

public class UserDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public ICollection<TicketReservationDTO> TicketReservations { get; set; } = new List<TicketReservationDTO>();
}