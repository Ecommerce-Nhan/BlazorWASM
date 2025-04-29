using System.Security.Claims;

namespace Ecommerce.Pages.User;

public partial class User
{
    private ClaimsPrincipal CurrentUser { get; set; } = default!;
    protected override async Task OnInitializedAsync()
    {
        CurrentUser = await _authenticationManager.CurrentUser();
    }
}
