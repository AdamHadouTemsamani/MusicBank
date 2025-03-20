using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MusicBank.Domain;

public class Event
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    [BsonElement("event_name")]
    public string EventName { get; set; } = null!;

    [BsonElement("event_venue")]
    public string EventVenue { get; set; } = null!;

    [BsonElement("event_date")]
    public DateTime EventDate { get; set; }
}
