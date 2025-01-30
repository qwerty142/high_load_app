using FluentResults;
using MongoDB.Driver;
using RateLimiter.Writer.Models;

namespace RateLimiter.Writer.Repositories;

public class RateLimiterRepository : IRateLimiterRepository
{
    private readonly IMongoCollection<LimiterDbModel> _limiters;

    public RateLimiterRepository(IMongoClient client)
    {
        var database = client.GetDatabase("writerdb");
        _limiters = database.GetCollection<LimiterDbModel>("limiters");
    }

    public async Task<Result<string>> AddAsync(LimiterServiceModel limiter, CancellationToken token)
    {
        var toInsert = Mapper.ServiceModelToDbModel(limiter);
        try
        {
            await _limiters.InsertOneAsync(toInsert, cancellationToken: token);
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error("Database error").CausedBy(ex));
        }
        
        return Result.Ok(toInsert.Id.ToString()).WithSuccess($"Added new limiter. ID={toInsert.Id}");
    }
    
    public async Task<Result<LimiterServiceModel>> GetByRouteAsync(string route, CancellationToken token)
    {
        try
        {
            var result = await _limiters.Find(l => l.Route == route).FirstOrDefaultAsync(token);
            return result is null
                ? Result.Fail($"Limiter for route {route} was not found")
                : Result.Ok(Mapper.DbModelToServiceModel(result));
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error("Database error").CausedBy(ex));
        }
    }

    public async Task<Result> UpdateAsync(string route, int amount, CancellationToken token)
    {
        try
        {
            var filter = Builders<LimiterDbModel>.Filter.Eq(l => l.Route, route);
            var update = Builders<LimiterDbModel>.Update.Set(l => l.Amount, amount);
            var result = await _limiters.UpdateOneAsync(filter, update, cancellationToken: token);
            return Result.OkIf(
                result.IsAcknowledged && result.ModifiedCount > 0,
                $"Limiter with route={route} was not found"
            );
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error("Database error").CausedBy(ex));
        }
    }
    
    public async Task<Result> DeleteAsync(string route, CancellationToken token)
    {
        try
        {
            var result = await _limiters.DeleteOneAsync(l => l.Route == route, cancellationToken: token);
            return Result.OkIf(
                result.IsAcknowledged && result.DeletedCount > 0,
                "Limiter with route={route} was not found");
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error("Database error").CausedBy(ex));
        }
    }

    public async Task<Result<LimiterServiceModel>> GetByIdAsync(string id, CancellationToken token)
    {
        try
        {
            var result = await _limiters.Find(l => l.Id.ToString() == id).FirstOrDefaultAsync(token);
            return result is null 
                ? Result.Fail($"Limiter with ID={id} was not found")
                : Result.Ok(Mapper.DbModelToServiceModel(result));
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error("Database error").CausedBy(ex));
        }
    }

    public async Task<Result<List<LimiterServiceModel>>> GetAllAsync(CancellationToken token)
    {
        try
        {
            var result = await _limiters.Find(l => true).ToListAsync(cancellationToken: token);
            return result is null || result.Count == 0
                ? Result.Fail($"Limiters were not found")
                : Result.Ok(result.Select(Mapper.DbModelToServiceModel).ToList());
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error("Database error").CausedBy(ex));
        }
    }
}