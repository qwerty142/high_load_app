using System.Text;
using FluentResults;
using Grpc.Core;
using UserService.Models.Api;
using UserService.Models.Mappers;
using UserService.Services;

namespace UserService.Controllers;

public class UserController : Models.Api.UserService.UserServiceBase
{
    private readonly IUserService _userService;
    private readonly UserGrpcMapper _mapper;

    public UserController(IUserService userService, UserGrpcMapper mapper)
    {
        _userService = userService;
        _mapper = mapper;
    }

    public override async Task<CreateUserResponse> CreateUser(CreateUserRequest request, ServerCallContext context)
    {
        var result = await _userService.CreateUserAsync(
            _mapper.ToUser(request),
            context.CancellationToken);

        var id = CheckResult(result, StatusCode.InvalidArgument);

        return new CreateUserResponse
        {
            Id = id
        };
    }

    public override async Task<UserResponse> GetUserById(GetUserByIdRequest request, ServerCallContext context)
    {
        var result = await _userService.GetUserByIdAsync(request.Id, context.CancellationToken);

        var user = CheckResult(result, StatusCode.NotFound);
        
        return new UserResponse
        {
            User = _mapper.ToUserMessage(user)
        };
    }

    public override async Task<ListOfUsersResponse> GetUserByName(GetUserByNameRequest request, ServerCallContext context)
    {
        var result = await _userService.GetUsersByNameAsync(request.Name, request.Surname, context.CancellationToken);

        var users = CheckResult(result, StatusCode.NotFound);

        var response = new ListOfUsersResponse();
        foreach (var user in users)
        {
            response.User.Add(_mapper.ToUserMessage(user));
        }

        return response;
    }

    public override async Task<EmptyResponse> UpdateUser(UpdateUserRequest request, ServerCallContext context)
    {
        var result = await _userService
            .UpdateUserAsync(_mapper.ToUser(request), context.CancellationToken);

        CheckResult(result, StatusCode.NotFound);
        
        return new EmptyResponse();
    }

    public override async Task<EmptyResponse> DeleteUser(DeleteUserByIdRequest request, ServerCallContext context)
    {
        var result = await _userService.DeleteUserAsync(_mapper.ToUser(request), context.CancellationToken);

        CheckResult(result, StatusCode.NotFound);
        
        return new EmptyResponse();
    }

    private static T CheckResult<T>(Result<T> result, StatusCode statusCode)
    {
        if (result.IsSuccess)
            return result.Value;

        var builder = new StringBuilder();
        foreach (var error in result.Errors)
        {
            builder.AppendLine(error.Message);
        }

        throw new RpcException(new Status(statusCode, builder.ToString()));
    }

    private static void CheckResult(Result result, StatusCode statusCode)
    {
        if (result.IsSuccess)
            return;

        var builder = new StringBuilder();
        foreach (var error in result.Errors)
        {
            builder.AppendLine(error.Message);
        }

        throw new RpcException(new Status(statusCode, builder.ToString()));
    }
}