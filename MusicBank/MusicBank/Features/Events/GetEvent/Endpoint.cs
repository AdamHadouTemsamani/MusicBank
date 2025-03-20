using MusicBank.Models;
using MusicBank.Data;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MusicBank.Domain;

namespace MusicBank.Features.Events.GetEvent;

public static class Endpoint
{
    public static IEndpointRouteBuilder MapGetEventEndpoints(
        this IEndpointRouteBuilder routes
    )
    {
        routes.MapGet(
            "/events/{eventId}", 
            async (
                string eventId,
                MongoDbContext db, 
                CancellationToken cancellationToken
            ) =>
        {
            var filter = Builders<Event>.Filter.Eq(e => e.Id, eventId);
                    var existingEvent = await db.Events.Find(filter).FirstOrDefaultAsync(cancellationToken);
                    return existingEvent is null
                        ? Results.NotFound()
                        : Results.Ok(existingEvent);

        });

        return routes;
    }
}