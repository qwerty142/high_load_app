using System.Text;
using FluentResults;
using Grpc.Core;
using RateLimiter.Reader.Repositories;
using RateLimiter.Reader.Services;

namespace RateLimiter.Reader.Controllers;

public class ReaderController : Reader.ReaderBase
{
    private readonly ILimiterService _service;

    public ReaderController(ILimiterService service)
    {
        _service = service;
    }

    public override Task<PingResponse> Ping(PingRequest request, ServerCallContext context)
    {
        return Task.FromResult(new PingResponse
        {
            Status = "Alive"
        });
    }

    public override Task<GetAllLimitsResponse> get(GetAllLimitsRequest request, ServerCallContext context)
    {
        var result = _service.GetAllLimits();
        var limits = CheckResult(result, StatusCode.Internal);

        var response = new GetAllLimitsResponse();
        foreach (var limit in limits)
        {
            response.Limits.Add(new Limit
            {
                Route = limit.Route,
                RequestsPerMinute = limit.Limit
            });
        }

        return Task.FromResult(response);
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
}