using FluentResults;
using UserService.Entities;

namespace UserService.Repositories;

public interface IUserRepository
{
    Task<Result<int>> CreateUserAsync(IUser user, CancellationToken cancellationToken);
    Task<Result<IUser>> GetUserByIdAsync(int id, CancellationToken cancellationToken);
    Task<Result<List<IUser>>> GetUsersByNameAsync(string name, string surname, CancellationToken cancellationToken);
    Task<Result> UpdateUserAsync(IUser user, CancellationToken cancellationToken);
    Task<Result> DeleteUserByIdAsync(int id, CancellationToken cancellationToken);
}