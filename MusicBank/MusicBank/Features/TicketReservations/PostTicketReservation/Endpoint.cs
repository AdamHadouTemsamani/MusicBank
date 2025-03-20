using MusicBank.Data;
using MusicBank.Models;
using MusicBank.Domain;
using MongoDB.Driver;
using MongoDB.Bson;

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
                MongoDbContext db, 
                CancellationToken cancellationToken
            ) =>
        {
            var userId = request.UserId;
            var eventId = request.EventId;

            // Check if a reservation for this user and event already exists
            var filter = Builders<TicketReservation>.Filter.And(
                Builders<TicketReservation>.Filter.Eq(tr => tr.UserId, userId.ToString()),
                Builders<TicketReservation>.Filter.Eq(tr => tr.EventId, eventId.ToString())
            );

            var existingTicketReservation = await db.TicketReservations.Find(filter).FirstOrDefaultAsync(cancellationToken);
            if (existingTicketReservation is not null)
            {
                return Results.Conflict("A ticket reservation for the same event and user already exists.");
            }

            var newTicketReservation = new TicketReservation
            {
                UserId = userId.ToString(),  // Store as ObjectId string
                EventId = eventId.ToString(),
                ReservationDate = request.ReservationDate
            };

            await db.TicketReservations.InsertOneAsync(newTicketReservation, cancellationToken: cancellationToken);

            return Results.Created($"/ticket-reservations/{newTicketReservation.Id}", newTicketReservation);
        });

        return routes;
    }
}
