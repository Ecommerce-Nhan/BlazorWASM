using SharedLibrary.Constants.Localization;

namespace ECommerce.Infrastructure.Settings;

public record ClientPreference : IPreference
{
    public bool IsDarkMode { get; set; }
    public bool IsDrawerOpen { get; set; }
    public string PrimaryColor { get; set; } = string.Empty;
    public string LanguageCode { get; set; } = LocalizationConstants.SupportedLanguages.FirstOrDefault()?.Code ?? "en-US";
}