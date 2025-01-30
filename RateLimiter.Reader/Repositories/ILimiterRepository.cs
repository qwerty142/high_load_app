using RateLimiter.Reader.Models;

namespace RateLimiter.Reader.Repositories;

public interface ILimiterRepository
{
    IAsyncEnumerable<RateLimit> LoadRateLimitsInBatches();
    IAsyncEnumerable<RateLimitChange> StartWatch();
}