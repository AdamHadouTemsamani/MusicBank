using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MusicBank.Domain;

public class TicketReservation
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    [BsonElement("user_id")]
    [BsonRepresentation(BsonType.ObjectId)] // Store reference to User as ObjectId
    public string? UserId { get; set; }

    [BsonElement("event_id")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? EventId { get; set; }

    [BsonElement("reservation_date")]
    public DateTime ReservationDate { get; set; }
}
