using System.Collections.Concurrent;
using FluentResults;
using RateLimiter.Reader.Models;
using RateLimiter.Reader.Repositories;

namespace RateLimiter.Reader.Services;

public class LimiterService : ILimiterService
{
    private readonly ILimiterRepository _limiterRepository;
    private readonly ConcurrentDictionary<string, RateLimit> _cache = new();

    public LimiterService(ILimiterRepository limiterRepository)
    {
        _limiterRepository = limiterRepository;
    }

    public Result<RateLimit[]> GetAllLimits()
    {
        var limiters = _cache.Values.ToArray();
        return Result
            .OkIf(limiters.Length > 0, "Limiters not found")
            .ToResult(limiters);
    }

    public Result<RateLimit> GetLimitByRoute(string route)
    {
        var limiter = _cache.GetValueOrDefault(route);
        return Result
            .OkIf(limiter is not null, $"Limiter for route={route} not found")
            .ToResult(limiter!);
    }

    public async Task InitializeAsync()
    {
        await foreach (var limiter in _limiterRepository.LoadRateLimitsInBatches())
        {
            _cache.TryAdd(limiter.Route, limiter);
        }

        await foreach (var change in _limiterRepository.StartWatch())
        {
            ApplyRateLimitChange(change);
        }
    }

    private void ApplyRateLimitChange(RateLimitChange change)
    {
        switch (change.Type)
        {
            case RateLimitChange.ChangeType.Insert:
                _cache.TryAdd(change.Route, Mapper.RateLimitChangeToRateLimit(change));
                break;
            case RateLimitChange.ChangeType.Update:
                var limiter = Mapper.RateLimitChangeToRateLimit(change);
                _cache[limiter.Route] = limiter;
                break;
            case RateLimitChange.ChangeType.Delete:
                _cache.TryRemove(change.Route, out _);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(change), "Unsupported operation type");
        }
    }
}