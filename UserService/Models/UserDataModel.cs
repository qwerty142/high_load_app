using UserService.Entities;

namespace UserService.Models;

public record UserDataModel(int Id, string Login, string Password, string Name, string Surname, int Age) : IUser;