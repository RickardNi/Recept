namespace Recept.Services;

public interface ISettingsStorage
{
    Task<T?> GetSettingAsync<T>(string key);

    Task SaveSettingAsync<T>(string key, T value);

    Task<HashSet<string>> GetFavoriteSlugsAsync();
}