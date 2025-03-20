using MusicBank.Domain;
using MusicBank.Data;
using MusicBank.Models;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace MusicBank.Features.Users.PostUser;

public static class Endpoint
{
    public static IEndpointRouteBuilder MapPostUserEndpoints(
        this IEndpointRouteBuilder routes
    )
    {
        routes.MapPost(
            "/users", 
            async (
                UserDTO request,
                MongoDbContext db, 
                CancellationToken cancellationToken
            ) =>
        {
           var filter = Builders<User>.Filter.Or(
                        Builders<User>.Filter.Eq(u => u.Email, request.Email),
                        Builders<User>.Filter.Eq(u => u.Name, request.Name)
            );
            var existingUser = await db.Users.Find(filter).FirstOrDefaultAsync(cancellationToken);
            if (existingUser is not null)
            {
                return Results.Conflict("A user with the same email or username already exists.");
            }

            var newUser = new User
            {
                Name = request.Name,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                TicketReservationIds = new List<string>()
                // Ensure UserId is set/generated as needed.
            };

            await db.Users.InsertOneAsync(newUser, cancellationToken: cancellationToken);
            return Results.Created($"/users/{newUser.Id}", newUser);

        });

        return routes;
    }
    
}