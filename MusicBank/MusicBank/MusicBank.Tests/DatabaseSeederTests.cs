using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MusicBank.Infrastructure;
using MusicBank.Models;
using MusicBank.Domain;
using MusicBank.Tests.FakeDataGenerators;
using Xunit;

public class DatabaseSeederTests: IAsyncLifetime
{
    private const string ConnectionString =
        "Host=localhost;Port=5432;Database=MusicBankDb;Username=postgres;Password=postgres";
    
    private MusicBankDbContext GetPostgresDbContext()
    {
        var options = new DbContextOptionsBuilder<MusicBankDbContext>()
            .UseNpgsql(ConnectionString)
            .Options;

        return new MusicBankDbContext(options);
    }

    [Fact]
    public async Task SeedData_ShouldInsertUsersEventsAndReservations()
    {
        // Arrange
        using var context = GetPostgresDbContext();
        var seeder = new PostgresDatabaseSeeder(context);

        await seeder.ClearTestDataAsync(); // Clean before test

        await seeder.SeedTestDataAsync();

        // Act
        var userCount = await context.Users.CountAsync();
        var eventCount = await context.Events.CountAsync();
        var reservationCount = await context.TicketReservations.CountAsync();

        // Assert
        Assert.True(userCount > 0, "Users should be inserted.");
        Assert.True(eventCount > 0, "Events should be inserted.");
        Assert.True(reservationCount > 0, "Reservations should be inserted.");
    }

    [Fact]
    public async Task ClearTestData_ShouldDeleteUsersAndReservations()
    {
        // Arrange
        using var context = GetPostgresDbContext();
        var seeder = new PostgresDatabaseSeeder(context);

        await seeder.SeedTestDataAsync();

        // Act
        await seeder.ClearTestDataAsync();

        var remainingUsers = await context.Users.CountAsync();
        var remainingReservations = await context.TicketReservations.CountAsync();
        var remainingEvents = await context.Events.CountAsync();

        // Assert
        Assert.Equal(0, remainingUsers);
        Assert.Equal(0, remainingReservations);
        Assert.Equal(0, remainingEvents);
    }


    public async Task InitializeAsync()
    {
        var context = GetPostgresDbContext();
        var seeder = new PostgresDatabaseSeeder(context);

        await seeder.SeedTestDataAsync();
    }

    public async Task DisposeAsync()
    {
        var context = GetPostgresDbContext();
        var seeder = new PostgresDatabaseSeeder(context); 
        await seeder.ClearTestDataAsync();
    }
}
