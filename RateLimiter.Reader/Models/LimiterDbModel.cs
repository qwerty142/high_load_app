using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RateLimiter.Reader.Models;

public class LimiterDbModel
{
    public LimiterDbModel(string route, int limit)
    {
        Route = route;
        Limit = limit;
    }

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId Id { get; set; }

    [BsonElement("route")] 
    public string Route { get; set; }

    [BsonElement("requests_per_minute")] 
    public int Limit { get; set; }
}