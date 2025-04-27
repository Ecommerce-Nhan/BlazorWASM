using ECommerce.Infrastructure.Managers.Admin.Roles;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using SharedLibrary.Constants.Permission;
using SharedLibrary.Dtos.Roles;
using SharedLibrary.Response.Identity;
using System.Security.Claims;

namespace Ecommerce.Pages.Role;

public partial class Role
{
    [Inject] private IRoleManager RoleManager { get; set; } = default!;

    private List<RoleResponse> _roleList = new();
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
        if (response is [])
        {
            _roleList = response;
        }
        else
        {
            _snackBar.Add("Error", Severity.Error);
        }
    }
}
