using FluentResults;
using RateLimiter.Writer.Models;

namespace RateLimiter.Writer.Services;

public interface IRateLimiterService
{
    public Task<Result<string>> CreateLimiter(LimiterServiceModel limiterServiceModel, CancellationToken token);
    public Task<Result> DeleteLimiter(string route, CancellationToken token);
    public Task<Result> UpdateLimiter(string route, int amount, CancellationToken token);
    public Task<Result<LimiterServiceModel>> GetLimiter(string route, CancellationToken token);
    public Task<Result<List<LimiterServiceModel>>> GetAllLimiters(CancellationToken token);
}