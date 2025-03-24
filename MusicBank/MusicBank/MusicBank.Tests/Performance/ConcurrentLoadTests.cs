using System;
using System.Diagnostics;
using System.Linq;
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

namespace MusicBank.MusicBank.Tests.Performance
{
    public class ConcurrentLoadTests : IAsyncLifetime, IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly ITestOutputHelper _output;

        public ConcurrentLoadTests(WebApplicationFactory<Program> factory, ITestOutputHelper output)
        {
            _factory = factory;
            _output = output;
        }

        public async Task InitializeAsync()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MusicBankDbContext>();
            var seeder = new PostgresDatabaseSeeder(db);
            await seeder.SeedTestDataAsync(); // Create and populate database with test data.
        }

        public async Task DisposeAsync()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MusicBankDbContext>();
            var seeder = new PostgresDatabaseSeeder(db);
            await seeder.ClearTestDataAsync(); // Dispose of test data.
        }

        [Theory]
        [InlineData(0)]
        [InlineData(8)]
        [InlineData(9)]
        [InlineData(6)]
        [InlineData(6)]
        [InlineData(7)]
        public async Task Concurrent_GetUsersReservedTickets_MeasureResponseTimes(int exponent)
        {
            // Calculate number of concurrent calls as 2^n.
            int concurrentRequests = (int)Math.Pow(2, exponent);
            _output.WriteLine($"Running test with {concurrentRequests} concurrent request(s).");
            // Ensure that test data is seeded by checking the events table.
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<MusicBankDbContext>();
                if (!await db.Events.AnyAsync())
                {
                    throw new Exception("Ensure that seeding is working correctly.");
                }
            }
            // Get all user IDs from the seeded data.
            var userIds = await GetSeededUserIdsAsync();
            // Create an asynchronous barrier using TaskCompletionSource.
            var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            int readyCount = 0;
            object lockObj = new object();
            var tasks = Enumerable.Range(0, concurrentRequests).Select(index => Task.Run(async () =>
            {
                // Signal that this task is ready.
                lock (lockObj)
                {
                    readyCount++;
                    if (readyCount == concurrentRequests)
                    {
                        // When the last task is ready, signal all tasks to proceed.
                        tcs.SetResult(true);
                    }
                }
                // Wait asynchronously until all tasks are ready.
                await tcs.Task;
                var stopwatch = Stopwatch.StartNew();
                using (var scope = _factory.Services.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<MusicBankDbContext>();
                    // Use round-robin selection for a valid user id.
                    var userId = userIds[index % userIds.Count];
                    // Query reserved tickets for the selected user.
                    var userTicketReservations = await db.TicketReservations
                        .Where(tr => tr.UserId == userId)
                        .Include(tr => tr.Event)
                        .ToListAsync();
                    _output.WriteLine($"Task {index}: User {userId} has {userTicketReservations.Count} reserved ticket(s).");
                }
                stopwatch.Stop();
                return stopwatch.ElapsedMilliseconds;
            })).ToArray();
            var responseTimes = await Task.WhenAll(tasks);
            var averageResponseTime = responseTimes.Average();
            _output.WriteLine($"Average response time: {averageResponseTime}ms");
            _output.WriteLine($"Slowest response time: {responseTimes.Max()}ms");
            _output.WriteLine($"Fastest response time: {responseTimes.Min()}ms");
        }

        private async Task<System.Collections.Generic.List<int>> GetSeededUserIdsAsync()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<MusicBankDbContext>();
                // Project the user IDs.
                var userIds = await db.Users
                    .Select(u => u.UserId)
                    .ToListAsync();
                if (userIds == null || !userIds.Any())
                {
                    throw new Exception("No users were found in the seeded data.");
                }
                return userIds;
            }
        }
    }
}