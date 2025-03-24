
using Microsoft.EntityFrameworkCore; // for DbContextOptions
using MusicBank.Infrastructure;
using MusicBank.Domain;
using MusicBank.Migrations;

namespace MusicBank.Tests.FakeDataGenerators;

public class PostgresDatabaseSeeder
{
    private readonly MusicBankDbContext _context;

    public PostgresDatabaseSeeder(MusicBankDbContext context)
    {
        _context = context;
    }

    public async Task SeedTestDataAsync()
    {
        var users = SqlFakeDataGenerator.GenerateUsers(1000);
        var events = SqlFakeDataGenerator.GenerateEvents(100);

        await _context.Users.AddRangeAsync(users);
        await _context.Events.AddRangeAsync(events);
        await _context.SaveChangesAsync();

        // Reload to get generated IDs
        var savedUsers = _context.Users.ToList();
        var savedEvents = _context.Events.ToList();

        var reservations = SqlFakeDataGenerator.GenerateReservations(2000, savedUsers, savedEvents);
        await _context.TicketReservations.AddRangeAsync(reservations);
        await _context.SaveChangesAsync();

        Console.WriteLine("✅ PostgreSQL test data seeded.");
    }

    public async Task ClearTestDataAsync()
    {
        await _context.Database.ExecuteSqlRawAsync("DELETE FROM ticket_reservation;");
        await _context.Database.ExecuteSqlRawAsync("DELETE FROM users");
        await _context.Database.ExecuteSqlRawAsync("DELETE FROM event;");
        Console.WriteLine("🗑️ PostgreSQL test data cleared.");
    }
}