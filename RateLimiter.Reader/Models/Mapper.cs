namespace RateLimiter.Reader.Models;

public static class Mapper
{
    public static RateLimit DbModelToRateLimit(LimiterDbModel dbModel)
    {
        return new RateLimit(dbModel.Route, dbModel.Limit);
    }

    public static RateLimit RateLimitChangeToRateLimit(RateLimitChange rateLimitChange)
    {
        return new RateLimit(rateLimitChange.Route, rateLimitChange.Limit);
    }

    public static RateLimitChange DbModelToRateLimitChange(LimiterDbModel dbModel, RateLimitChange.ChangeType changeType)
    {
        return new RateLimitChange(dbModel.Route, dbModel.Limit, changeType);
    }
}