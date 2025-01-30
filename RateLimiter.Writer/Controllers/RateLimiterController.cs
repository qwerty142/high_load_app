using System.Text;
using FluentResults;
using Grpc.Core;
using RateLimiter.Writer.Models;
using RateLimiter.Writer.Services;

namespace RateLimiter.Writer.Controllers;

public class RateLimiterController : Writer.WriterBase
{
    private readonly IRateLimiterService _service;

    public RateLimiterController(IRateLimiterService service)
    {
        _service = service;
    }
    
    public override async Task<CreateLimitResponse> CreateLimit(CreateLimitRequest request, ServerCallContext context)
    {
        var result = await _service.CreateLimiter(Mapper.CreateRequestToServiceModel(request), context.CancellationToken);
        var id = CheckResult(result, StatusCode.InvalidArgument);
        
        return new CreateLimitResponse
        {
            Id = id
        };
    }
    
    public override async Task<GetLimitResponse> GetLimit(GetLimitRequest request, ServerCallContext context)
    {
        var result = await _service.GetLimiter(request.Route, context.CancellationToken);
        var limiter = CheckResult(result, StatusCode.NotFound);
        
        return new GetLimitResponse
        {
            Limit = new LimitMessage
            {
                Id = limiter.Id,
                Route = limiter.Route,
                RequestsPerMinute = limiter.Amount
            }
        };
    }
    
    public override async Task<UpdateLimitResponse> UpdateLimit(UpdateLimitRequest request, ServerCallContext context)
    {
        var result = await _service.UpdateLimiter(request.Route, request.RequestsPerMinute, context.CancellationToken);
        CheckResult(result, StatusCode.Cancelled);
        
        return new UpdateLimitResponse
        {
            Message = "Success"
        };
    }

    public override async Task<DeleteLimitResponse> DeleteLimit(DeleteLimitRequest request, ServerCallContext context)
    {
        var result = await _service.DeleteLimiter(request.Route, context.CancellationToken);
        CheckResult(result, StatusCode.InvalidArgument);

        return new DeleteLimitResponse
        {
            Message = "Success"
        };
    }

    public override async Task<GetAllLimitResponse> GetAllLimit(GetAllLimitRequest request, ServerCallContext context)
    {
        var result = await _service.GetAllLimiters(context.CancellationToken);
        var models = CheckResult(result, StatusCode.NotFound);

        var response = new GetAllLimitResponse();
        foreach (var elem in models)
        {
            response.Limiters.Add(Mapper.ServiceModelToLimitModel(elem));
        }

        return response;
    }
    
    private static T CheckResult<T>(Result<T> result, StatusCode code)
    {
        if (result.IsSuccess)
            return result.Value;

        var builder = new StringBuilder();
        foreach (var error in result.Errors)
        {
            builder.AppendLine(error.Message);
        }

        throw new RpcException(new Status(code, builder.ToString()));
    }

    private static void CheckResult(Result result, StatusCode code)
    {
        if (result.IsSuccess)
            return;

        var builder = new StringBuilder();
        foreach (var error in result.Errors)
        {
            builder.AppendLine(error.Message);
        }

        throw new RpcException(new Status(code, builder.ToString()));
    }
}