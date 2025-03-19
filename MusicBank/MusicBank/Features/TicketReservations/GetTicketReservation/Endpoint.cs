using MusicBank.Data;
using MusicBank.Models;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace MusicBank.Features.TicketReservations.GetTicketReservations;

public static class Endpoint
{
    public static IEndpointRouteBuilder MapGetTicketReservationEndpoints(
        this IEndpointRouteBuilder routes
    )
    {
        routes.MapGet(
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
            return Results.Ok(ticketReservation);
        });

        return routes;
    }
}
