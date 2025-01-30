namespace UserService.Services;

public interface IRestrictionService
{
    Task<bool> IsUserRestrictedAsync(string userId, string endpoint);
}