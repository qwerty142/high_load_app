using FluentResults;
using RateLimiter.Reader.Models;

namespace RateLimiter.Reader.Services;

public interface ILimiterService
{
    public Result<RateLimit[]> GetAllLimits();
    public Result<RateLimit> GetLimitByRoute(string route);
    public Task InitializeAsync();
}