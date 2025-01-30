using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RateLimiter.Writer.Models;

public class LimiterServiceModel
{
    public LimiterServiceModel(string id, string route, int amount)
    {
        Id = id;
        Route = route;
        Amount = amount;
    }
    public string Id { get; set; }
    public string Route { get; set; }
    public int Amount { get; set; }
}