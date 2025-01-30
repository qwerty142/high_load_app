using FluentResults;
using FluentValidation.Results;
using Microsoft.Extensions.Caching.Memory;
using UserService.Entities;
using UserService.Entities.Validation;
using UserService.Repositories;

namespace UserService.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly UserValidator _userValidator;

    private readonly IMemoryCache _cache;
    private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(5);
    
    private readonly ILogger<UserService> _logger;

    public UserService(IUserRepository userRepository, UserValidator userValidator, IMemoryCache cache)
    {
        _userRepository = userRepository;
        _userValidator = userValidator;
        _cache = cache;
        _logger = LoggerFactory
            .Create(builder => builder.AddConsole())
            .CreateLogger<UserService>();
    }

    public async Task<Result<int>> CreateUserAsync(IUser user, CancellationToken cancellationToken)
    {
        var validationResult = _userValidator.Validate(user);
        if (!validationResult.IsValid)
        {
            return CollectValidationErrors(validationResult);
        }

        return await _userRepository.CreateUserAsync(user, cancellationToken);
    }

    public async Task<Result<IUser>> GetUserByIdAsync(int id, CancellationToken cancellationToken)
    {
        var cacheKey = ConfigureGetByIdCacheKey(id);

        if (_cache.TryGetValue<IUser>(cacheKey, out var user))
        {
            _logger.LogInformation("[GetUserByIdAsync] Cache used!");
            return Result.Ok(user!);
        }
        
        var getResult = await _userRepository.GetUserByIdAsync(id, cancellationToken);
        if (getResult.IsSuccess)
        {
            _cache.Set(cacheKey, getResult.Value, _cacheDuration);
        }
        return getResult;
    }

    public async Task<Result<List<IUser>>> GetUsersByNameAsync(string name, string surname, CancellationToken cancellationToken)
    {
        var cacheKey = ConfigureGetByNameCacheKey(name, surname);

        if (_cache.TryGetValue<List<IUser>>(cacheKey, out var users))
        {
            _logger.LogInformation("[GetUsersByNameAsync] Cache used!");
            return Result.Ok(users!);
        }
        
        var getResult =  await _userRepository.GetUsersByNameAsync(name, surname, cancellationToken);
        if (getResult.IsSuccess)
        {
            _cache.Set(cacheKey, getResult.Value, _cacheDuration);
        }
        
        return getResult;
    }
    
    public async Task<Result> UpdateUserAsync(IUser user, CancellationToken cancellationToken)
    { 
        var validationResult = _userValidator.Validate(user);
        if (!validationResult.IsValid)
        {
            return CollectValidationErrors(validationResult);
        }

        var updateResult = await _userRepository.UpdateUserAsync(user, cancellationToken);
        if (updateResult.IsSuccess)
        {
            InvalidateCache(ConfigureGetByIdCacheKey(user.Id));
            InvalidateCache(ConfigureGetByNameCacheKey(user.Name, user.Surname));
        }

        return updateResult;
    }

    public async Task<Result> DeleteUserAsync(IUser user, CancellationToken cancellationToken)
    {
        var deleteResult = await _userRepository.DeleteUserByIdAsync(user.Id, cancellationToken);
        if (deleteResult.IsSuccess)
        {
            InvalidateCache(ConfigureGetByIdCacheKey(user.Id));
            InvalidateCache(ConfigureGetByNameCacheKey(user.Name, user.Surname));
        }
        
        return deleteResult;
    }

    private static Result CollectValidationErrors(ValidationResult validationResult)
    {
        var fails = Result.Fail("Fields did not pass validation");
        foreach (var error in validationResult.Errors)
        {
            fails.WithError(error.ErrorMessage);
        }

        return fails;
    }

    private string ConfigureGetByNameCacheKey(string name, string surname) => $"users-by-name-{name}-{surname}";
    private string ConfigureGetByIdCacheKey(int id) => $"user-{id}";

    private void InvalidateCache(string cacheKey)
    {
        _cache.Remove(cacheKey);
    }
}