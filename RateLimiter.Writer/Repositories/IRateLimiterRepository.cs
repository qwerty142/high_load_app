using FluentResults;
using RateLimiter.Writer.Models;

namespace RateLimiter.Writer.Repositories;

public interface IRateLimiterRepository
{
    Task<Result<List<LimiterServiceModel>>> GetAllAsync(CancellationToken token);
    Task<Result<LimiterServiceModel>> GetByIdAsync(string id, CancellationToken token);
    Task<Result<LimiterServiceModel>> GetByRouteAsync(string route, CancellationToken token);
    Task<Result<string>> AddAsync(LimiterServiceModel limiter, CancellationToken token);
    Task<Result> UpdateAsync(string route, int amount, CancellationToken token);
    Task<Result> DeleteAsync(string id, CancellationToken token);
}