using System;
using System.Threading.Tasks;
using MongoDB.Driver;
using MusicBank.Infrastructure;
using MusicBank.Domain;
using MusicBank.Tests.FakeDataGenerators; // Contains MongoFakeDataGenerator methods

namespace MusicBank.Tests.FakeDataGenerators
{
    public class MongoDatabaseSeeder
    {
        private readonly MongoDbContext _context;

        public MongoDatabaseSeeder(MongoDbContext context)
        {
            _context = context;
        }

        public async Task SeedTestDataAsync()
        {
            // Optionally, recreate the database to start fresh.
            _context.RecreateDatabase();

            // Generate fake data for users and events.
            // (Make sure your MongoFakeDataGenerator is adapted to produce domain objects.)
            var users = NoSqlFakeDataGenerator.GenerateUsers(1000);
            var events = NoSqlFakeDataGenerator.GenerateEvents(100);

            // Insert generated users and events.
            await _context.Users.InsertManyAsync(users);
            await _context.Events.InsertManyAsync(events);

            // Generate reservations based on the inserted users and events.
            var reservations = NoSqlFakeDataGenerator.GenerateReservations(20000, users, events);
            await _context.TicketReservations.InsertManyAsync(reservations);

            Console.WriteLine("✅ MongoDB test data seeded.");
        }

        public async Task ClearTestDataAsync()
        {
            // Remove all documents from the collections.
            await _context.TicketReservations.DeleteManyAsync(FilterDefinition<TicketReservation>.Empty);
            await _context.Users.DeleteManyAsync(FilterDefinition<User>.Empty);
            await _context.Events.DeleteManyAsync(FilterDefinition<Event>.Empty);

            Console.WriteLine("🗑️ MongoDB test data cleared.");
        }
    }
}