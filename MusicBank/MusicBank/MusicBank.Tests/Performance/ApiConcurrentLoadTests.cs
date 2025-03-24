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

namespace MusicBank.MusicBank.Tests.Performance;

public class ApiConcurrentLoadTests
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    private const int ConcurrentRequests = 50;

    public ApiConcurrentLoadTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
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
    public async Task Concurrent_GetEvents_Requests_ShouldMeasureResponseTimes()
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

        var tasks = Enumerable.Range(0, ConcurrentRequests).Select(i => Task.Run(async () =>
        {
            //Wait until all threads are reading to call the API
            barrier.SignalAndWait();

            var stopwatch = Stopwatch.StartNew(); //Start timing
            HttpResponseMessage response = await _client.GetAsync($"ticket-reservations/{i}"); //Api Call
            stopwatch.Stop(); //Stop timer

            response.EnsureSuccessStatusCode();
            return stopwatch.ElapsedMilliseconds;
        })).ToArray();
        
        // Wait for all tasks to complete.
        long[] responseTimes = await Task.WhenAll(tasks);

        // Output each response time for review.
        Console.WriteLine(responseTimes[0]);

    }
}