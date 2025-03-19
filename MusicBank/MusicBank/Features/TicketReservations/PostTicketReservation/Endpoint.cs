using MusicBank.Data;
using MusicBank.Models;
using Microsoft.EntityFrameworkCore;
using MusicBank.Domain;

namespace MusicBank.Features.TicketReservations.PostTicketReservation;

public static class Endpoint
{
    public static IEndpointRouteBuilder MapPostTicketReservationEndpoints(
        this IEndpointRouteBuilder routes
    )
    {
        routes.MapPost(
            "/ticket-reservations", 
            async (
                TicketReservationDTO request,
                MusicBankDbContext db, 
                CancellationToken cancellationToken
            ) =>
        {
            var existingTicketReservation = await db.TicketReservations.FirstOrDefaultAsync(tr =>
                tr.UserId == request.UserId && tr.EventId == request.EventId,
                cancellationToken
            );
            if (existingTicketReservation is not null)
            {
                return Results.Conflict(
                    "A ticket reservation for the same event and user already exists."
                );
            }

            var newTicketReservation = new TicketReservation
            {
                EventId = request.EventId,
                UserId = request.UserId,
                ReservationDate = request.ReservationDate
            };
            db.TicketReservations.Add(newTicketReservation);
            await db.SaveChangesAsync();
            
            return Results.Created($"/ticket-reservations/{newTicketReservation.TicketReservationId}", newTicketReservation);

        });

        return routes;
    }
    
}