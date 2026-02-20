using Blazored.SessionStorage;

namespace Recept.Services;

public class SessionStorageSettings(ISessionStorageService sessionStorageService) : ISettingsStorage
{
    public const string FavoriteRecipeSlugs = "favoriteRecipeSlugs";

    private readonly ISessionStorageService _sessionStorageService = sessionStorageService;

    public async Task<T?> GetSettingAsync<T>(string key)
    {
        try
        {
            return await _sessionStorageService.GetItemAsync<T>(key);
        }
        catch
        {
            // Local storage might be disabled, full, or data might be corrupted
            return default;
        }
    }

    public async Task SaveSettingAsync<T>(string key, T value)
    {
        try
        {
            await _sessionStorageService.SetItemAsync(key, value);
        }
        catch
        {
            // Local storage might be disabled, full, or in private browsing mode
        }
    }
}