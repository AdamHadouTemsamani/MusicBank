using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MusicBank.Domain;

namespace MusicBank.Data
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;
        private readonly MongoClient _client;

        public MongoDbContext(IOptions<MongoDbSettings> settings)
        {
            _client = new MongoClient(settings.Value.ConnectionString);
            _database = _client.GetDatabase(settings.Value.DatabaseName);
        }

        public IMongoCollection<User> Users => _database.GetCollection<User>("users");
        public IMongoCollection<Event> Events => _database.GetCollection<Event>("events");
        public IMongoCollection<TicketReservation> TicketReservations => _database.GetCollection<TicketReservation>("ticket_reservations");

        // **Deletes and recreates the database**
        public void RecreateDatabase()
        {
            _client.DropDatabase(_database.DatabaseNamespace.DatabaseName);
        }
    }

    public class MongoDbSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
    }
}
