using Dapper;
using FluentResults;
using Npgsql;
using UserService.Entities;
using UserService.Models;
using UserDbMapper = UserService.Models.Mappers.UserDbMapper;

namespace UserService.Repositories;

public class UserRepository : IUserRepository
{
    private readonly NpgsqlDataSource _dataSource;
    private readonly UserDbMapper _mapper;
    public UserRepository(NpgsqlDataSource dataSource, UserDbMapper mapper)
    {
        _dataSource = dataSource;
        _mapper = mapper;
    }

    public async Task<Result<int>> CreateUserAsync(IUser user, CancellationToken cancellationToken)
    {
        var model = _mapper.ToUserDataModel(user);
        
        const string sql = "SELECT * FROM create_user(@Login, @Password, @Name, @Surname, @Age);";

        var parameters = new DynamicParameters();
        parameters.Add("Login", model.Login);
        parameters.Add("Password", model.Password);
        parameters.Add("Name", model.Name);
        parameters.Add("Surname", model.Surname);
        parameters.Add("Age", model.Age);
        
        var command = new CommandDefinition(sql, parameters, cancellationToken: cancellationToken);

        try
        {
            await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
            var id = await connection.QuerySingleAsync<int?>(command);
            return id is null
                ? Result.Fail($"User with LOGIN={model.Login} already exists")
                : Result.Ok(id.Value);
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error("Database Error").CausedBy(ex));
        }
    }

    public async Task<Result<IUser>> GetUserByIdAsync(int id, CancellationToken cancellationToken)
    {
        const string sql = "SELECT * FROM get_user_by_id(@Id);";

        var parameters = new DynamicParameters();
        parameters.Add("Id", id);

        var command = new CommandDefinition(sql, parameters, cancellationToken: cancellationToken);

        try
        {
            await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
            var model = await connection.QueryFirstOrDefaultAsync<UserDataModel>(command);
            return model is null
                ? Result.Fail($"User with ID={id} does not exist")
                : Result.Ok<IUser>(_mapper.ToUser(model));
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error("Database Error").CausedBy(ex));
        }
    }

    public async Task<Result<List<IUser>>> GetUsersByNameAsync(string name, string surname, CancellationToken cancellationToken)
    {
        const string sql = "SELECT * FROM get_user_by_name(@Name, @Surname);";

        var parameters = new DynamicParameters();
        parameters.Add("Name", name);
        parameters.Add("Surname", surname);

        var command = new CommandDefinition(sql, parameters, cancellationToken: cancellationToken);
        
        try
        {
            await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
            var models = await connection.QueryAsync<UserDataModel>(command);
            var users = models.Select(m => _mapper.ToUser(m)).ToList<IUser>();
            
            return users.Count > 0
                ? Result.Ok(users)
                : Result.Fail($"Users with NAME={name} and SURNAME={surname} were not found");
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error("Database Error").CausedBy(ex));
        }
    }

    public async Task<Result> UpdateUserAsync(IUser user, CancellationToken cancellationToken)
    {
        var model = _mapper.ToUserDataModel(user);

        const string sql = "SELECT * FROM update_user(@Id, @Password, @Name, @Surname, @Age);";

        var parameters = new DynamicParameters();
        parameters.Add("Id", model.Id);
        parameters.Add("Password", model.Password);
        parameters.Add("Name", model.Name);
        parameters.Add("Surname", model.Surname);
        parameters.Add("Age", model.Age);

        var command = new CommandDefinition(sql, parameters, cancellationToken: cancellationToken);

        try
        {
            await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
            await connection.ExecuteAsync(command);

            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error("Database Error").CausedBy(ex));
        }
    }

    public async Task<Result> DeleteUserByIdAsync(int id, CancellationToken cancellationToken)
    {
        const string sql = "SELECT * FROM delete_user_by_id(@Id);";

        var parameters = new DynamicParameters();
        parameters.Add("Id", id);

        var command = new CommandDefinition(sql, parameters, cancellationToken: cancellationToken);

        try
        {
            await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken); 
            await connection.ExecuteAsync(command);

            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error("Database Error").CausedBy(ex));
        }
    }
}