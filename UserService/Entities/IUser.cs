namespace UserService.Entities;

public interface IUser
{
    public int Id { get; }
    public string Login { get; }
    public string Password { get; }
    public string Name { get; }
    public string Surname { get; }
    public int Age { get; }
}