using MusicBank.Data;
using MusicBank.Models;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace MusicBank.Features.TicketReservtions.PostTicketReservation;

public static class Endpoint
{
    public static IEndpointRouteBuilder MapPostTicketReservation(
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
                tr.User == request.User && tr.Event == request.Event
            );
            if (existingTicketReservation is not null)
            {
                return Results.Conflict(
                    "A ticket reservation for the same event and user already exists."
                );
            }

            var newTicketReservation = new TicketReservationDTO
            {
                Event = request.Event,
                User = request.User,
                ReservationDate = request.ReservationDate
            };
            db.TicketReservations.Add(newTicketReservation);
            await db.SaveChangesAsync();
            
            return Results.NoContent();

        });

        return routes;
    }
    
}