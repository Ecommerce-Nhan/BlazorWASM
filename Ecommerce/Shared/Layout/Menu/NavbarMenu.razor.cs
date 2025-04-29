using Microsoft.AspNetCore.Components;

namespace Ecommerce.Shared.Layout.Menu;

public partial class NavbarMenu : ComponentBase
{
    [CascadingParameter]
    public Func<Task> ToggleDarkModeAsync { get; set; } = default!;
    bool _darkMode = false;
    private async Task LogOut()
    {
        await _authenticationManager.Logout();
        _navigationManager.NavigateTo("/admin/login");
    }
    private async Task ToggleDarkMode()
    {
        if (ToggleDarkModeAsync != null)
        {
            await ToggleDarkModeAsync.Invoke();
            _darkMode = !_darkMode;
        }
    }
}