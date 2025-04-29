using Microsoft.AspNetCore.Components;

namespace Ecommerce.Shared.Layout.Menu;

public partial class NavbarMenu : ComponentBase
{
    private async Task LogOut()
    {
        await _authenticationManager.Logout();
        _navigationManager.NavigateTo("/admin/login");
    }
}