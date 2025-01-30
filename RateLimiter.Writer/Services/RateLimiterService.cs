using FluentResults;
using RateLimiter.Writer.Models;
using RateLimiter.Writer.Repositories;

namespace RateLimiter.Writer.Services;

public class RateLimiterService : IRateLimiterService
{
    private readonly IRateLimiterRepository _repository;

    public RateLimiterService(IRateLimiterRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<Result<string>> CreateLimiter(LimiterServiceModel limiterServiceModel, CancellationToken token)
    {
        return await _repository.AddAsync(limiterServiceModel, token);
    }
    
    public async Task<Result<LimiterServiceModel>> GetLimiter(string route, CancellationToken token)
    {
        return await _repository.GetByRouteAsync(route, token);
    }
    
    public async Task<Result> UpdateLimiter(string route, int amount, CancellationToken token)
    {
        return await _repository.UpdateAsync(route, amount, token);
    }

    public async Task<Result> DeleteLimiter(string route, CancellationToken token)
    {
        return await _repository.DeleteAsync(route, token);
    }

    public async Task<Result<List<LimiterServiceModel>>> GetAllLimiters(CancellationToken token)
    {
        return await _repository.GetAllAsync(token);
    }
}