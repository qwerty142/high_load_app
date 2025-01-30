using UserService.Entities;
using UserService.Models.Api;

namespace UserService.Models.Mappers;

public class UserGrpcMapper
{
    public User ToUser(IUser request)
    {
        return new User(request.Id, request.Login, request.Password, request.Name, request.Surname, request.Age);
    }

    public UserMessage ToUserMessage(IUser user)
    {
        return new UserMessage(user);
    }
}