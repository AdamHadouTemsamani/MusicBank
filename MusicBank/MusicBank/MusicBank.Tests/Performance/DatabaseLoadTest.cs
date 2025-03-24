/*
using MusicBank.Infrastructure; // for MusicBankDbContext
using Microsoft.EntityFrameworkCore;// for DbContextOptions
using MusicBank.Tests.FakeDataGenerators;
using System.Diagnostics; 

public class DatabaseLoadPostgresTests: IAsyncLifetime
{
    private DbContextOptions<MusicBankDbContext> GetOptions()
    {
        return new DbContextOptionsBuilder<MusicBankDbContext>()
            .UseNpgsql("Host=localhost;Port=5432;Database=MusicBankDb;Username=postgres;Password=postgres")
            .Options;
    }

    [Fact]
    public async Task Measure_ConcurrentPostgresWrites()
    {
        var options = GetOptions();

        var users = SqlFakeDataGenerator.GenerateUsers(100);;
        var stopwatch = Stopwatch.StartNew();

        var tasks = users.Select(async user =>
        {
            using var context = new MusicBankDbContext(options);
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
        });

        await Task.WhenAll(tasks);
        stopwatch.Stop();

        using var verifyContext = new MusicBankDbContext(options);
        var count = await verifyContext.Users.CountAsync();
        Console.WriteLine($"✅ 100 writes to PostgreSQL took {stopwatch.ElapsedMilliseconds}ms");

        Assert.Equal(100, count);
    }

    [Fact]
    public async Task Measure_ConcurrentPostgresReads()
    {
        var options = GetOptions();

        using (var context = new MusicBankDbContext(options))
        {
            var seeder = new PostgresDatabaseSeeder(context);
            await seeder.SeedTestDataAsync();
        }

        var stopwatch = Stopwatch.StartNew();

        var tasks = Enumerable.Range(0, 100).Select(async _ =>
        {
            using var context = new MusicBankDbContext(options);
            var user = await context.Users.FirstOrDefaultAsync(u => u.Email.Contains("@"));
        });

        await Task.WhenAll(tasks);
        stopwatch.Stop();

        Console.WriteLine($"✅ 100 reads from PostgreSQL took {stopwatch.ElapsedMilliseconds}ms");
    }

    public async Task InitializeAsync()
    {
        var options = GetOptions();
        using var context = new MusicBankDbContext(options);
        var seeder = new PostgresDatabaseSeeder(context);
        await seeder.SeedTestDataAsync();
    }

    public async Task DisposeAsync()
    {
        var options = GetOptions();
        using var context = new MusicBankDbContext(options);
        var seeder = new PostgresDatabaseSeeder(context);
        await seeder.ClearTestDataAsync();
    }
}
*/
