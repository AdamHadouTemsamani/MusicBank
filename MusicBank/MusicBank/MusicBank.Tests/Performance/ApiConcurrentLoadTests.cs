using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MusicBank.Models;
using MusicBank.Domain;
using MusicBank.Tests.FakeDataGenerators; // Assumes your FakeDataGenerator and seeder are here.
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using MusicBank.Infrastructure;
using Xunit.Abstractions;

namespace MusicBank.MusicBank.Tests.Performance;

public class ApiConcurrentLoadTests: IAsyncLifetime, IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly ITestOutputHelper _output;

    private const int ConcurrentRequests = 50;

    public ApiConcurrentLoadTests(WebApplicationFactory<Program> factory, ITestOutputHelper output)
    {
        _factory = factory;
        _client = _factory.CreateClient();
        _output = output;
        
    }

    public async Task InitializeAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<MusicBankDbContext>();
        var seeder = new PostgresDatabaseSeeder(db);
        await seeder.SeedTestDataAsync(); //Create and populate database with test data
    }

    public async Task DisposeAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<MusicBankDbContext>();
        var seeder = new PostgresDatabaseSeeder(db);
        await seeder.ClearTestDataAsync(); //Dispose of test data
    }

    [Fact]
    public async Task Concurrent_GetUsersReservedTickets_MeasureResponseTimes()
    {
        // Ensure that there is data seeded.
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<MusicBankDbContext>();
            if (!db.Events.Any())
            {
                throw new Exception("Ensure that seeding is working correctly.");
            }
        }
        
        Barrier barrier = new Barrier(ConcurrentRequests);

        var tasks = Enumerable.Range(1, ConcurrentRequests).Select(i => Task.Run(async () =>
        {
            //Wait until all threads are reading to request the database
            barrier.SignalAndWait();

            var stopwatch = Stopwatch.StartNew(); //Start timing
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<MusicBankDbContext>();
                // Query the Events table (or whichever table you want to inspect)
                var userTicketReservations = await db.TicketReservations
                    .Where(tr => tr.UserId == i)
                    .Include(tr => tr.Event)
                    .ToListAsync();
                
                _output.WriteLine($"Task {i}: User {i} has {userTicketReservations.Count} reserved ticket(s).");
            }
            stopwatch.Stop(); //Stop timer

            return stopwatch.ElapsedMilliseconds;
        })).ToArray();
        
        var responseTimes = await Task.WhenAll(tasks);
        var averageResponseTime = responseTimes.Average();
        _output.WriteLine($"Average response time: {averageResponseTime}ms");
        _output.WriteLine($"Slowest response time: {responseTimes.Max()}ms");
        _output.WriteLine($"Fastest respoonse time: {responseTimes.Min()}ms");


    }
}