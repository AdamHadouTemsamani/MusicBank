using MusicBank.Models;
using MusicBank.Data;
using Microsoft.EntityFrameworkCore;

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
                MusicBankDbContext db, 
                CancellationToken cancellationToken
            ) =>
        {
            // Check if an event with the same name already exists.
            var existingEvent = await db.Events.FirstOrDefaultAsync(e =>
                e.EventName == request.EventName
            );
            if (existingEvent is not null)
            {
                return Results.Conflict(
                    "An event with the same name already exists."
                );
            }

            var newEvent = new EventDTO
            {
                EventName = request.EventName,
                EventVenue = request.EventVenue,
                EventDate = request.EventDate
            };
            db.Events.Add(newEvent);
            await db.SaveChangesAsync();
            
            return Results.NoContent();

        });

        return routes;
    }
}