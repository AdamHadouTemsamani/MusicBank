using MusicBank.Domain;
using MusicBank.Data;
using MusicBank.Models;
using Microsoft.EntityFrameworkCore;

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
                MusicBankDbContext db, 
                CancellationToken cancellationToken
            ) =>
        {
           // Check if a user with the same email or username already exists.
            var existingUser = await db.Users.FirstOrDefaultAsync(u =>
                u.Email == request.Email || u.Name == request.Name
            );
            if (existingUser is not null)
            {
                return Results.Conflict(
                    "A user with the same email or username already exists."
                );
            }

            var newUser = new User
            {
                Name = request.Name,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
            };
            db.Users.Add(newUser);
            await db.SaveChangesAsync();
            
            return Results.Created($"/users/{newUser.UserId}", newUser); 

        });

        return routes;
    }
    
}