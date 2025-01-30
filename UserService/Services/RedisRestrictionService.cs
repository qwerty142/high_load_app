using StackExchange.Redis;

namespace UserService.Services;

public class RedisRestrictionService : IRestrictionService
{
    private readonly IDatabase _database;

    public RedisRestrictionService(IConnectionMultiplexer connection)
    {
        _database = connection.GetDatabase();
    }
    
    public async Task<bool> IsUserRestrictedAsync(string userId, string endpoint)
    {
        var key = $"restriction:{userId}:{endpoint}";
        return await _database.KeyExistsAsync(key);
    }
}