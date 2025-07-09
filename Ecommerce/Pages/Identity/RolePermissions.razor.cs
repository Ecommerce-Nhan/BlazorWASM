using AutoMapper;
using ECommerce.Infrastructure.Managers.Admin.Roles;
using ECommerce.Infrastructure.Mappings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using SharedLibrary.Constants.Permission;
using SharedLibrary.Requests.Identity;
using SharedLibrary.Response.Identity;
using System.Security.Claims;

namespace Ecommerce.Pages.Identity;

public partial class RolePermissions
{
    [Inject] private IRoleManager RoleManager { get; set; } = default!;

    //[CascadingParameter] private HubConnection HubConnection { get; set; }
    [EditorRequired][Parameter] public string Id { get; set; } = default!;
    [Parameter] public string Title { get; set; } = default!;
    [Parameter] public string Description { get; set; } = default!;

    private PermissionResponse _model = new();
    private Dictionary<string, List<RoleClaimResponse>> GroupedRoleClaims { get; } = new();
    private IMapper _mapper = default!;
    private RoleClaimResponse _roleClaims = new();
    private RoleClaimResponse _selectedItem = new();
    private string _searchString = "";

    private ClaimsPrincipal _currentUser = default!;
    private bool _canEditRolePermissions;
    private bool _canSearchRolePermissions;
    private bool _loaded;

    protected override async Task OnInitializedAsync()
    {
        _currentUser = await _authenticationManager.CurrentUser();
        _canEditRolePermissions = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.RoleClaims.Edit)).Succeeded;
        _canSearchRolePermissions = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.RoleClaims.Search)).Succeeded;

        await GetRolePermissionsAsync();
        _loaded = true;
        //HubConnection = HubConnection.TryInitialize(_navigationManager, _localStorage);
        //if (HubConnection.State == HubConnectionState.Disconnected)
        //{
        //    await HubConnection.StartAsync();
        //}
    }

    private async Task GetRolePermissionsAsync()
    {
        _mapper = new MapperConfiguration(c => { c.AddProfile<RoleProfile>(); }).CreateMapper();
        var roleId = Id;
        var result = await RoleManager.GetPermissionsAsync(roleId);
        if (result.Succeeded)
        {
            _model = result.Data;
            GroupedRoleClaims.Add(_localizer["All Permissions"], _model.RoleClaims);
            foreach (var claim in _model.RoleClaims)
            {
                if (GroupedRoleClaims.ContainsKey(claim.Group))
                {
                    GroupedRoleClaims[claim.Group].Add(claim);
                }
                else
                {
                    GroupedRoleClaims.Add(claim.Group, new List<RoleClaimResponse> { claim });
                }
            }
            if (_model != null)
            {
                Description = string.Format(_localizer["Manage {0} {1}'s Permissions"], _model.RoleId, _model.RoleName);
            }
        }
        else
        {
            foreach (var error in result.Errors)
            {
                _snackBar.Add(error, Severity.Error);
            }
            _navigationManager.NavigateTo("/admin/role");
        }
    }

    private async Task SaveAsync()
    {
        var request = _mapper.Map<PermissionResponse, PermissionRequest>(_model);
        var result = await RoleManager.UpdatePermissionsAsync(request);
        if (result.Succeeded)
        {
            _snackBar.Add(result.Message, Severity.Success);
            //await HubConnection.SendAsync(ApplicationConstants.SignalR.SendRegenerateTokens);
            //await HubConnection.SendAsync(ApplicationConstants.SignalR.OnChangeRolePermissions, _currentUser.GetUserId(), request.RoleId);
            _navigationManager.NavigateTo("/admin/role");
        }
        else
        {
            foreach (var error in result.Errors)
            {
                _snackBar.Add(error, Severity.Error);
            }
        }
    }

    private bool Search(RoleClaimResponse roleClaims)
    {
        if (string.IsNullOrWhiteSpace(_searchString)) return true;
        if (roleClaims.ClaimValue?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
        {
            return true;
        }
        if (roleClaims.Description?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
        {
            return true;
        }
        return false;
    }

    private Color GetGroupBadgeColor(int selected, int all)
    {
        if (selected == 0)
            return Color.Error;

        if (selected == all)
            return Color.Success;

        return Color.Info;
    }
}