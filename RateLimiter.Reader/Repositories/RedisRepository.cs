using StackExchange.Redis;

namespace RateLimiter.Reader.Repositories;

public class RedisRepository : IRestrictionRepository
{
    private readonly IDatabase _database;

    public RedisRepository(IConnectionMultiplexer connection)
    {
        _database = connection.GetDatabase();
    }
    
    public async Task SetRestrictionAsync(int userId, string endpoint, TimeSpan ttl)
    {
        var key = $"restriction:{userId}:{endpoint}";
        await _database.StringSetAsync(key, "restricted",  ttl);
    }

    public async Task<bool> IsRestrictedAsync(int userId, string endpoint)
    {
        var key = $"restriction:{userId}:{endpoint}";
        return await _database.KeyExistsAsync(key);
    }
}