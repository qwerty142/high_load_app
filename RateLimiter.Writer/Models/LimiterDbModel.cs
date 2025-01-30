using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RateLimiter.Writer.Models;

public class LimiterDbModel
{
    public LimiterDbModel(string route, int amount)
    {
        Route = route;
        Amount = amount;
    }
    
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId Id { get; set; }
    
    [BsonElement("route")]
    public string Route { get; set; }

    [BsonElement("requests_per_minute")]
    public int Amount { get; set; }
}