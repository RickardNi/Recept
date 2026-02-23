using Blazored.LocalStorage;

namespace Recept.Services;

public class LocalStorageSettings(ILocalStorageService localStorageService) : ISettingsStorage
{
    public const string FavoriteRecipeSlugs = "favoriteRecipeSlugs";

    private readonly ILocalStorageService _localStorageService = localStorageService;

    public async Task<T?> GetSettingAsync<T>(string key)
    {
        try
        {
            return await _localStorageService.GetItemAsync<T>(key);
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
            await _localStorageService.SetItemAsync(key, value);
        }
        catch
        {
            // Local storage might be disabled, full, or in private browsing mode
        }
    }
}
