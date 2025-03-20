using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MusicBank.Domain;

public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString(); // MongoDB `_id`

    [BsonElement("name")]
    public string Name { get; set; } = null!;

    [BsonElement("email")]
    public string Email { get; set; } = null!;

    [BsonElement("phone_number")]
    public string PhoneNumber { get; set; } = null!;

    // Store TicketReservation IDs instead of navigation properties
    [BsonElement("ticket_reservation_ids")]
    public List<string> TicketReservationIds { get; set; } = new();
}
