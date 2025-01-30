using MongoDB.Driver;
using RateLimiter.Reader.Models;

namespace RateLimiter.Reader.Repositories;

public class LimiterRepository : ILimiterRepository
{
    private readonly IMongoCollection<LimiterDbModel> _rateLimitsCollection;

    public LimiterRepository(IMongoClient client)
    {
        var database = client.GetDatabase("ratelimitsdb");
        _rateLimitsCollection = database.GetCollection<LimiterDbModel>("rate_limits");
    }

    public async IAsyncEnumerable<RateLimitChange> StartWatch()
    {
        await foreach (var change in WatchForChangesAsync())
        {
            switch (change.OperationType)
            {
                case ChangeStreamOperationType.Insert:
                    var insertedRateLimit = change.FullDocument;
                    if (insertedRateLimit is not null)
                    {
                        yield return Mapper
                            .DbModelToRateLimitChange(insertedRateLimit, RateLimitChange.ChangeType.Insert);
                    }

                    break;
                case ChangeStreamOperationType.Update:
                    var updatedRateLimit = change.FullDocument;
                    if (updatedRateLimit is not null)
                    {
                        yield return Mapper
                            .DbModelToRateLimitChange(updatedRateLimit, RateLimitChange.ChangeType.Update);
                    }

                    break;
                case ChangeStreamOperationType.Delete:
                    var deletedRateLimit = change.FullDocumentBeforeChange;
                    if (deletedRateLimit is not null)
                    {
                        yield return Mapper
                            .DbModelToRateLimitChange(deletedRateLimit, RateLimitChange.ChangeType.Delete);
                    }

                    break;
            }
        }
    }

    public async IAsyncEnumerable<RateLimit> LoadRateLimitsInBatches()
    {
        const int batchSize = 1000;

        var filter = Builders<LimiterDbModel>.Filter.Empty;
        var options = new FindOptions<LimiterDbModel> { BatchSize = batchSize };
        
        using var cursor = await _rateLimitsCollection.FindAsync(filter, options);

        var result = new List<RateLimit>();
        while (await cursor.MoveNextAsync())
        {
            foreach (var dbModel in cursor.Current)
            {
                yield return Mapper.DbModelToRateLimit(dbModel);
            }
        }
    }

    private async IAsyncEnumerable<ChangeStreamDocument<LimiterDbModel>> WatchForChangesAsync()
    {
        var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<LimiterDbModel>>()
            .Match(change => change.OperationType == ChangeStreamOperationType.Insert ||
                             change.OperationType == ChangeStreamOperationType.Update ||
                             change.OperationType == ChangeStreamOperationType.Delete);

        var options = new ChangeStreamOptions
        {
            FullDocument = ChangeStreamFullDocumentOption.UpdateLookup,
            FullDocumentBeforeChange = ChangeStreamFullDocumentBeforeChangeOption.WhenAvailable
        };

        using var cursor = await _rateLimitsCollection.WatchAsync(pipeline, options);

        while (await cursor.MoveNextAsync())
        {
            foreach (var change in cursor.Current)
            {
                yield return change;
            }
        }
    }
}