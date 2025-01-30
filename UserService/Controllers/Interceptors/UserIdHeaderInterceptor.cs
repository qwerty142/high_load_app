using Grpc.Core;
using Grpc.Core.Interceptors;

namespace UserService.Controllers.Interceptors;

public class UserIdHeaderInterceptor : Interceptor
{
    private const string UserIdHeader = "user_id";

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        if (!context.RequestHeaders.Any(x => x.Key.Equals(UserIdHeader)))
        {
            throw new RpcException(new Status(
                StatusCode.Unauthenticated,
                "Missing user_id header. Authentication needed"
                ));
        }
        
        return await continuation(request, context);
    }
}