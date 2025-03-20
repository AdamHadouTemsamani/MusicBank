using MusicBank.Data;
using Microsoft.EntityFrameworkCore;
using MusicBank.Domain;
using MongoDB.Driver;

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
                string userId,
                MongoDbContext db, 
                CancellationToken cancellationToken
            ) =>
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, userId);
            var existingUser = await db.Users.Find(filter).FirstOrDefaultAsync(cancellationToken);
            return existingUser is null
                ? Results.NotFound()
                : Results.Ok(existingUser);

        });

        return routes;
    }
}