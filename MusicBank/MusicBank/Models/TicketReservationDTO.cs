using System.Runtime.InteropServices.JavaScript;

namespace MusicBank.Models;

public record TicketReservationDTO
(
    int UserId,
    int EventId,
    DateTime ReservationDate
);