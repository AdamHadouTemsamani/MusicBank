namespace MusicBank.Models;

public record UserDTO
(
    int Id, 
    string Name,
    string Email,
    string PhoneNumber,
    IList<TicketReservationDTO> TicketReservations
);