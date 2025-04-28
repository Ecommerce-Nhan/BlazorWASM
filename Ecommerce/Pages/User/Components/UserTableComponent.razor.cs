using Microsoft.AspNetCore.Components;
using MudBlazor;
using SharedLibrary.Dtos.Users;

namespace Ecommerce.Pages.User.Components;

public partial class UserTableComponent : ComponentBase
{
    private List<UserDto> UserList { get; set; } = new();
    protected override async Task OnInitializedAsync()
    {
        await GetUsersAsync();
    }
    private async Task GetUsersAsync()
    {
        var response = await _userManager.GetAllAsync();
        if (response.Succeeded)
        {
            UserList = response.Data.ToList();
        }
        else
        {
            foreach (var message in response.Errors ?? [])
            {
                _snackBar.Add(message, Severity.Error);
            }
        }
    }
}
