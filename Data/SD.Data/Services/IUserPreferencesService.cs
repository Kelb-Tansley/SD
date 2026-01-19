using SD.Core.Shared.Entity;

namespace SD.Data.Services;

public interface IUserPreferencesService
{
    public Task SaveUserPreferences(UserPreferences userPreferences);
    public Task<UserPreferences?> GetUserPreferences(string userName);
}