namespace UserService.Entities;

public record User(int Id, string Login, string Password, string Name, string Surname, int Age) : IUser;