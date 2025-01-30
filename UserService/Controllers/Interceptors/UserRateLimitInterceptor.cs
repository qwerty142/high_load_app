using Grpc.Core;
using Grpc.Core.Interceptors;
using UserService.Services;

namespace UserService.Controllers.Interceptors;

public class UserRateLimitInterceptor : Interceptor
{
    private readonly IRestrictionService _restrictionService;

    public UserRateLimitInterceptor(IRestrictionService restrictionService)
    {
        _restrictionService = restrictionService;
    }

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        var userId = context
            .RequestHeaders
            .FirstOrDefault(x => x.Key.Equals("user_id"))!.Value;
        var endpoint = context.Method;

        if (await _restrictionService.IsUserRestrictedAsync(userId, endpoint))
        {
            throw new RpcException(new Status(
                StatusCode.ResourceExhausted,
                "Exceeded request per minute limit"
                ));
        }
        
        return await continuation(request, context);
    }
}