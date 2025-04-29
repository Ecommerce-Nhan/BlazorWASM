using ECommerce.Infrastructure.Settings;
using SharedLibrary.Wrappers;

namespace ECommerce.Infrastructure.Managers.Preferences;

public interface IPreferenceManager
{
    Task SetPreference(IPreference preference);

    Task<IPreference> GetPreference();

    Task<IResponse> ChangeLanguageAsync(string languageCode);
}