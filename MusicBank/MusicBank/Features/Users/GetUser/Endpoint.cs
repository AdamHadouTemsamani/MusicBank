using MusicBank.Data;
using MusicBank.Models;
using Microsoft.EntityFrameworkCore;

namespace MusicBank.Features.Users.GetUser;

public static class Endpoint
{
    public static IEndpointRouteBuilder MapGetUserEndpoints(
        this IEndpointRouteBuilder routes
    )
    {
        routes.MapGet(
            "/users/{userId}", 
            async (
                int userId,
                MusicBankDbContext db, 
                CancellationToken cancellationToken
            ) =>
        {
            var existingUser = await db.Users.FirstOrDefaultAsync(u =>
                u.UserId == userId
            );
            if (existingUser is null)
            {
                return Results.NotFound();
            }

            return Results.Ok(existingUser);

        });

        return routes;
    }
}