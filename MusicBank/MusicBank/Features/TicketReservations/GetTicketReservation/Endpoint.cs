using MusicBank.Data;
using MusicBank.Models;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MusicBank.Domain;

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
                string ticketReservationId,
                MongoDbContext db, 
                CancellationToken cancellationToken
            ) =>
        {
            var filter = Builders<TicketReservation>.Filter.Eq(tr => tr.Id, ticketReservationId);
                    var ticketReservation = await db.TicketReservations.Find(filter).FirstOrDefaultAsync(cancellationToken);
                    return ticketReservation is null
                        ? Results.NotFound()
                        : Results.Ok(ticketReservation);
        });

        return routes;
    }
}
