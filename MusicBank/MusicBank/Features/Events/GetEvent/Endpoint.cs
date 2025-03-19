using MusicBank.Models;
using MusicBank.Data;
using Microsoft.EntityFrameworkCore;

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
                int eventId,
                MusicBankDbContext db, 
                CancellationToken cancellationToken
            ) =>
        {
            var existingEvent = await db.Events.FirstOrDefaultAsync(e =>
                e.Id == eventId
            );
            if (existingEvent is null)
            {
                return Results.NotFound();
            }

            return Results.Ok(existingEvent);

        });

        return routes;
    }
}