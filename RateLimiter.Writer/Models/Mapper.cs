namespace RateLimiter.Writer.Models;

public static class Mapper
{
    public static LimiterServiceModel DbModelToServiceModel(LimiterDbModel dbModel)
    {
        return new LimiterServiceModel(dbModel.Id.ToString(), dbModel.Route, dbModel.Amount);
    }

    public static LimiterDbModel ServiceModelToDbModel(LimiterServiceModel serviceModel)
    {
        return new LimiterDbModel(serviceModel.Route, serviceModel.Amount);
    }

    public static LimiterServiceModel CreateRequestToServiceModel(CreateLimitRequest request)
    {
        return new LimiterServiceModel(string.Empty, request.Route, request.RequestsPerMinute);
    }

    public static LimitMessage ServiceModelToLimitModel(LimiterServiceModel model)
    {
        return new LimitMessage
        {
            Id = model.Id,
            Route = model.Route,
            RequestsPerMinute = model.Amount
        };
    }
}