using SD.Core.Shared.Entity;
using System.Text.Json;

namespace SD.Data.Services;

public partial class UserPreferencesService(IAppSettings appSettings) : BlobFileService, IUserPreferencesService
{
    private readonly IAppSettings _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));

    public async Task SaveUserPreferences(UserPreferences userPreferences)
    {
        ArgumentNullException.ThrowIfNull(userPreferences);

        string fileName = $"{userPreferences.UserName}.json";
        string path = $"{_appSettings.UserPreferencesLocation}\\{fileName}";
        string json = JsonSerializer.Serialize(userPreferences);

        await StoreBlobFileAsync(path, json);
    }
    public async Task<UserPreferences?> GetUserPreferences(string userName)
    {
        string fileName = $"{userName}.json";
        string path = $"{_appSettings.UserPreferencesLocation}\\{fileName}";
        string content = ReadBlobFile(path);
        if (string.IsNullOrEmpty(content))
            return null;

        return JsonSerializer.Deserialize<UserPreferences>(content);
    }
}