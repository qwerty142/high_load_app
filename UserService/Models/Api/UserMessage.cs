using UserService.Entities;

namespace UserService.Models.Api;

public partial class UserMessage : IUser
{
    public UserMessage(IUser user)
    {
        Id = user.Id;
        Login = user.Login;
        Password = user.Password;
        Name = user.Name;
        Surname = user.Surname;
        Age = user.Age;
    }
}