using Blazored.LocalStorage;
using ECommerce.Infrastructure.Settings;
using Microsoft.Extensions.Localization;
using MudBlazor;
using SharedLibrary.Constants.Storage;
using SharedLibrary.Wrappers;

namespace ECommerce.Infrastructure.Managers.Preferences;

public class ClientPreferenceManager : IClientPreferenceManager
{
    private readonly ILocalStorageService _localStorageService;
    private readonly IStringLocalizer<ClientPreferenceManager> _localizer;

    public ClientPreferenceManager(
        ILocalStorageService localStorageService,
        IStringLocalizer<ClientPreferenceManager> localizer)
    {
        _localStorageService = localStorageService;
        _localizer = localizer;
    }

    public async Task<bool> ToggleDarkModeAsync()
    {
        var preference = await GetPreference() as ClientPreference;
        if (preference != null)
        {
            preference.IsDarkMode = !preference.IsDarkMode;
            await SetPreference(preference);
            return !preference.IsDarkMode;
        }

        return false;
    }
    public async Task<bool> ToggleLayoutDirection()
    {
        var preference = await GetPreference() as ClientPreference;
        if (preference != null)
        {
            await SetPreference(preference);
        }
        return false;
    }

    public async Task<IResponse> ChangeLanguageAsync(string languageCode)
    {
        var preference = await GetPreference() as ClientPreference;
        if (preference != null)
        {
            preference.LanguageCode = languageCode;
            await SetPreference(preference);
            return new Response
            {
                Succeeded = true,
                Errors = new List<string> { _localizer["Client Language has been changed"] }
            };
        }

        return new Response
        {
            Succeeded = false,
            Errors = new List<string> { _localizer["Failed to get client preferences"] }
        };
    }

    public async Task<MudTheme> GetCurrentThemeAsync()
    {
        var preference = await GetPreference() as ClientPreference;
        return BlazorTheme.DefaultTheme;
    }

    public async Task<IPreference> GetPreference()
    {
        return await _localStorageService.GetItemAsync<ClientPreference>(StorageConstants.Local.Preference) ?? new ClientPreference();
    }

    public async Task SetPreference(IPreference preference)
    {
        await _localStorageService.SetItemAsync(StorageConstants.Local.Preference, preference as ClientPreference);
    }
}