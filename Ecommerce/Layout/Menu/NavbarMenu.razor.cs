using Microsoft.AspNetCore.Components;

namespace Ecommerce.Layout.Menu;

public partial class NavbarMenu : ComponentBase
{
    private async Task LogOut()
    {
        await _authenticationManager.Logout();
        _navigationManager.NavigateTo("/login");
    }
}