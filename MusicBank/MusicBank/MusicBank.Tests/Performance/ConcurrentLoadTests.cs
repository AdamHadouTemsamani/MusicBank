using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit.Abstractions;
using MusicBank.Domain;
using MusicBank.Infrastructure;
using MusicBank.Tests.FakeDataGenerators;

namespace MusicBank.MusicBank.Tests.Performance
{
    public class ApiConcurrentLoadTests : IAsyncLifetime, IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly ITestOutputHelper _output;
        private const int ConcurrentRequests = 50;

        public ApiConcurrentLoadTests(WebApplicationFactory<Program> factory, ITestOutputHelper output)
        {
            _factory = factory;
            _output = output;
        }

        public async Task InitializeAsync()
        {
            using var scope = _factory.Services.CreateScope();
            var mongoContext = scope.ServiceProvider.GetRequiredService<MongoDbContext>();
            var seeder = new MongoDatabaseSeeder(mongoContext);
            await seeder.SeedTestDataAsync(); // Create and populate MongoDB with test data.
        }

        public async Task DisposeAsync()
        {
            using var scope = _factory.Services.CreateScope();
            var mongoContext = scope.ServiceProvider.GetRequiredService<MongoDbContext>();
            var seeder = new MongoDatabaseSeeder(mongoContext);
            await seeder.ClearTestDataAsync(); // Dispose of test data.
        }

        [Fact]
        public async Task Concurrent_GetUsersReservedTickets_MeasureResponseTimes()
        {
            // Ensure that test data is seeded by checking the events collection.
            using (var scope = _factory.Services.CreateScope())
            {
                var mongoContext = scope.ServiceProvider.GetRequiredService<MongoDbContext>();
                var eventsCount = await mongoContext.Events.CountDocumentsAsync(FilterDefinition<Event>.Empty);
                if (eventsCount == 0)
                {
                    throw new Exception("Ensure that seeding is working correctly.");
                }
            }

            // Get all user IDs from the seeded data.
            List<string> userIds;
            using (var scope = _factory.Services.CreateScope())
            {
                var mongoContext = scope.ServiceProvider.GetRequiredService<MongoDbContext>();
                // Assume that UserId is stored as a string in MongoDB.
                userIds = await mongoContext.Users
                    .Find(FilterDefinition<User>.Empty)
                    .Project(u => u.Id.ToString())
                    .ToListAsync();
                if (!userIds.Any())
                {
                    throw new Exception("No users were found in the seeded data.");
                }
            }

            Barrier barrier = new Barrier(ConcurrentRequests);

            var tasks = Enumerable.Range(0, ConcurrentRequests).Select(index => Task.Run(async () =>
            {
                // Wait until all tasks are ready.
                barrier.SignalAndWait();

                var stopwatch = Stopwatch.StartNew();
                using (var scope = _factory.Services.CreateScope())
                {
                    var mongoContext = scope.ServiceProvider.GetRequiredService<MongoDbContext>();

                    // Select a user id from the list using round-robin.
                    var userId = userIds[index % userIds.Count];

                    // Query reserved tickets for the user.
                    var filter = Builders<TicketReservation>.Filter.Eq(tr => tr.UserId, userId);
                    var userTicketReservations = await mongoContext.TicketReservations.Find(filter).ToListAsync();
                    
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
    }
}
