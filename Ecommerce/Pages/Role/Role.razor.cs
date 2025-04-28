using ECommerce.Infrastructure.Managers.Admin.Roles;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using SharedLibrary.Constants.Permission;
using SharedLibrary.Response.Identity;
using System.Security.Claims;

namespace Ecommerce.Pages.Role;

public partial class Role
{
    [Inject] private IRoleManager RoleManager { get; set; } = default!;

    private RoleResponse _role = new();
    private List<RoleResponse> _roleList = new();

    private bool _dense = false;
    private bool _striped = true;
    private bool _bordered = false;
    private string _searchString = "";

    private ClaimsPrincipal _currentUser = default!;
    private bool _canCreate;

    protected override async Task OnInitializedAsync()
    {
        _currentUser = await _authenticationManager.CurrentUser();
        _canCreate = (await _authorizationService.AuthorizeAsync(_currentUser, resource: null, policyName: Permissions.Roles.Create)).Succeeded;

        await GetRolesAsync();
    }

    private async Task GetRolesAsync()
    {
        var response = await RoleManager.GetAllAsync();
        if (response.Succeeded)
        {
            _roleList = response.Data.ToList();
        }
        else
        {
            foreach (var message in response.Errors!)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }
    }

    private bool Search(RoleResponse role)
    {
        if (string.IsNullOrWhiteSpace(_searchString)) return true;
        if (role.Name?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
        {
            return true;
        }
        if (role.Description?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
        {
            return true;
        }
        return false;
    }
}
