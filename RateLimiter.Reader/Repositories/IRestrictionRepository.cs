namespace RateLimiter.Reader.Repositories;

public interface IRestrictionRepository
{
    Task SetRestrictionAsync(int userId, string endpoint, TimeSpan ttl);
    Task<bool> IsRestrictedAsync(int userId, string endpoint);
}