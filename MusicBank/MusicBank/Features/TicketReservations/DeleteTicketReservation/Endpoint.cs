using MusicBank.Data;
using MusicBank.Models;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace MusicBank.Features.TicketReservtions.DeleteTicketReservation;

public static class Endpoint
{
    public static IEndpointRouteBuilder MapDeleteTicketReservation(
        this IEndpointRouteBuilder routes
    )
    {
        routes.MapDelete(
            "/ticket-reservations/{ticketReservationId}", 
            async (
                int ticketReservationId,
                MusicBankDbContext db, 
                CancellationToken cancellationToken
            ) =>
        {
            var ticketReservation = await db.TicketReservations.FirstOrDefaultAsync(tr =>
                tr.Id == ticketReservationId
            );
            if (ticketReservation is null)
            {
                return Results.NotFound();
            }
            db.TicketReservations.Remove(ticketReservation);
            await db.SaveChangesAsync();
            return Results.NoContent();
        });

        return routes;
    }
}