using MudBlazor;
using SharedLibrary.Constants.Permission;
using SharedLibrary.Dtos.Users;
using System.Security.Claims;

namespace Ecommerce.Pages.User;

public partial class User
{
    private List<UserDto> _userList = new();
    private ClaimsPrincipal _currentUser = default!;
    private bool _canCreate;
    protected override async Task OnInitializedAsync()
    {
        _currentUser = await _authenticationManager.CurrentUser();
        _canCreate = (await _authorizationService.AuthorizeAsync(_currentUser, resource: null, policyName: Permissions.Users.Create)).Succeeded;

        await GetUsersAsync();
    }

    private async Task GetUsersAsync()
    {
        var response = await _userManager.GetAllAsync();
        if (response.Succeeded)
        {
            _userList = response.Data.ToList();
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
