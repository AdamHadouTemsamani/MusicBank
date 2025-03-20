using MusicBank.Data;
using MusicBank.Models;
using System.Text.Json;
using MusicBank.Domain;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace MusicBank.Features.TicketReservations.DeleteTicketReservation;

public static class Endpoint
{
    public static IEndpointRouteBuilder MapDeleteTicketReservationEndpoints(
        this IEndpointRouteBuilder routes
    )
    {
        routes.MapDelete(
            "/ticket-reservations/{ticketReservationId}", 
            async (
                string ticketReservationId,
                MongoDbContext db, 
                CancellationToken cancellationToken
            ) =>
        {
            // Create a filter for the ticket reservation
                var filter = Builders<TicketReservation>.Filter.Eq(tr => tr.Id, ticketReservationId);

                // Find the ticket reservation using the filter
                var ticketReservation = await db.TicketReservations.Find(filter)
                    .FirstOrDefaultAsync(cancellationToken);

                if (ticketReservation is null)
                {
                    return Results.NotFound();
                }

                // Delete the ticket reservation
                var deleteResult = await db.TicketReservations.DeleteOneAsync(filter, cancellationToken);

                if (deleteResult.DeletedCount > 0)
                {
                    return Results.NoContent();
                }

                return Results.Problem("Failed to delete ticket reservation.");
        });

        return routes;
    }
}