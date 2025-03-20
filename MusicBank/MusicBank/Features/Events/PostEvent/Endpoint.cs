using MusicBank.Models;
using MusicBank.Data;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MusicBank.Domain;

namespace MusicBank.Features.Events.PostEvent;

public static class Endpoint
{
    public static IEndpointRouteBuilder MapPostEventEndpoints(
        this IEndpointRouteBuilder routes
    )
    {
        routes.MapPost(
            "/events", 
            async (
                EventDTO request,
                MongoDbContext db, 
                CancellationToken cancellationToken
            ) =>
        {
            var filter = Builders<Event>.Filter.Eq(e => e.EventName, request.EventName);
            var existingEvent = await db.Events.Find(filter).FirstOrDefaultAsync(cancellationToken);
            if (existingEvent is not null)
            {
                return Results.Conflict("An event with the same name already exists.");
            }

            var newEvent = new Event
            {
                EventName = request.EventName,
                EventVenue = request.EventVenue,
                EventDate = request.EventDate
                // Ensure that EventId is set appropriately (or generated) if needed.
            };

            await db.Events.InsertOneAsync(newEvent, cancellationToken: cancellationToken);
            return Results.Created($"/events/{newEvent.Id}", newEvent);

        });

        return routes;
    }
}