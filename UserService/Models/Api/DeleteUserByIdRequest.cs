using UserService.Entities;

namespace UserService.Models.Api;

public partial class DeleteUserByIdRequest : IUser
{
    public string Login => string.Empty;
    public string Password => string.Empty;
    public string Name => string.Empty;
    public string Surname => string.Empty;
    public int Age => 0;
}