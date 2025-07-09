using ECommerce.Infrastructure.Managers.Admin.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using SharedLibrary.Constants.Permission;
using SharedLibrary.Requests.Identity;
using SharedLibrary.Response.Identity;
using System.Security.Claims;

namespace Ecommerce.Pages.Identity;

public partial class Role
{
    [Inject] private IRoleManager RoleManager { get; set; } = default!;

    private RoleResponse _role = new();
    private List<RoleResponse> _roleList = new();
    private string _searchString = "";

    private ClaimsPrincipal _currentUser = default!;
    private bool _canCreateRoles;
    private bool _canEditRoles;
    private bool _canDeleteRoles;
    private bool _canSearchRoles;
    private bool _canViewRoleClaims;

    protected override async Task OnInitializedAsync()
    {
        _currentUser = await _authenticationManager.CurrentUser();
        _canCreateRoles = (await _authorizationService.AuthorizeAsync(_currentUser, resource: null, policyName: Permissions.Roles.Create)).Succeeded;
        _canEditRoles = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Roles.Edit)).Succeeded;
        _canDeleteRoles = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Roles.Delete)).Succeeded;
        _canSearchRoles = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Roles.Search)).Succeeded;
        _canViewRoleClaims = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.RoleClaims.View)).Succeeded;

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

    private async Task Delete(string id)
    {
        string deleteContent = _localizer["Delete Content"];
        var parameters = new DialogParameters
            {
                {nameof(ECommerce.Shared.Dialogs.DeleteConfirmation.ContentText), string.Format(deleteContent, id)}
            };
        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, BackdropClick = false };
        var dialog = await _dialogService.ShowAsync<ECommerce.Shared.Dialogs.DeleteConfirmation>(_localizer["Delete"], parameters, options);
        var result = await dialog.Result;
        if (result != null && !result.Canceled)
        {
            var response = await RoleManager.DeleteAsync(id);
            if (response.Succeeded)
            {
                await Reset();
                //await HubConnection.SendAsync(ApplicationConstants.SignalR.SendUpdateDashboard);
                _snackBar.Add(response.Message, Severity.Success);
            }
            else
            {
                await Reset();
                foreach (var message in response.Errors!)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }
    }

    private async Task InvokeModal(string? id = null)
    {
        var parameters = new DialogParameters();
        if (id != null)
        {
            _role = _roleList.FirstOrDefault(c => c.Id == id) ?? new RoleResponse();
            parameters.Add(nameof(RoleModal.RoleModel), new RoleRequest
            {
                Id = _role.Id,
                Name = _role.Name,
                Description = _role.Description
            });
        }
        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, BackdropClick = false };
        var dialog = await _dialogService.ShowAsync<RoleModal>(id == null ? _localizer["Create"] : _localizer["Edit"], parameters, options);
        var result = await dialog.Result;
        if (result != null && !result.Canceled)
        {
            await Reset();
        }
    }

    private async Task Reset()
    {
        _role = new RoleResponse();
        await GetRolesAsync();
    }

    private void ManagePermissions(string roleId)
    {
        _navigationManager.NavigateTo($"/admin/role-permissions/{roleId}");
    }
}