using Microsoft.AspNetCore.Authorization;
using MudBlazor;
using SharedLibrary.Constants.Permission;
using SharedLibrary.Dtos.Users;
using System.Security.Claims;

namespace Ecommerce.Pages.Identity;

public partial class User
{
    private MudTable<UserDto> _table = default!;
    private string _searchString = "";

    private ClaimsPrincipal _currentUser = default!;
    private bool _canCreate;
    private bool _canSearch;
    private bool _canViewRoles;

    protected override async Task OnInitializedAsync()
    {
        _currentUser = await _authenticationManager.CurrentUser();
        _canCreate = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Users.Create)).Succeeded;
        _canSearch = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Users.Search)).Succeeded;
        _canViewRoles = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Roles.View)).Succeeded;
    }

    private async Task InvokeModal()
    {
        var parameters = new DialogParameters();
        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, BackdropClick = false };
        var dialog = await _dialogService.ShowAsync<RegisterUserModal>(_localizer["Register New User"], parameters, options);
        var result = await dialog.Result;
        if (result != null && !result.Canceled)
        {
            await _table.ReloadServerData();
        }
    }

    private void ManageRoles(string userId, string? email)
    {
        if (email == "3aef4452-5f15-42b5-8d5c-9eaab0b23476") 
            _snackBar.Add(_localizer["Not Allowed."], Severity.Error);
        else 
            _navigationManager.NavigateTo($"/admin/user-roles/{userId}");
    }

    private void OnSearch(string text)
    {
        _searchString = text;
        _table.ReloadServerData();
    }

    private void OnFilter(ref List<UserDto> data, TableState state)
    {
        switch (state.SortLabel)
        {
            case "FirstName":
                data = data.OrderByDirection(state.SortDirection, o => o.FirstName).ToList();
                break;
            case "LastName":
                data = data.OrderByDirection(state.SortDirection, o => o.LastName).ToList();
                break;
            case "UserName":
                data = data.OrderByDirection(state.SortDirection, o => o.UserName).ToList();
                break;
            case "Email":
                data = data.OrderByDirection(state.SortDirection, o => o.Email).ToList();
                break;
            case "PhoneNumber":
                data = data.OrderByDirection(state.SortDirection, o => o.PhoneNumber).ToList();
                break;
        }
    }

    private async Task<TableData<UserDto>> ServerReload(TableState state, CancellationToken token)
    {
        var response = await _userManager.GetAllAsync();
        var data = response.Data.ToList();
        OnFilter(ref data, state);

        return new TableData<UserDto>()
        {
            Items = data,
            TotalItems = response.TotalRecords
        };
    }
}
