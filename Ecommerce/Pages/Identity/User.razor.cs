using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using MudBlazor;
using SharedLibrary.Constants.Permission;
using SharedLibrary.Dtos.Users;

namespace Ecommerce.Pages.Identity
{
    public partial class User
    {
        private List<UserDto> _userList = new();
        private UserDto _user = default!;
        private string _searchString = "";
        private bool _dense = true;
        private bool _striped = true;
        private bool _bordered = true;

        private ClaimsPrincipal _currentUser = default!;
        private bool _canCreateUsers;
        private bool _canSearchUsers;
        private bool _canExportUsers;
        private bool _canViewRoles;
        private bool _loaded;

        protected override async Task OnInitializedAsync()
        {
            _currentUser = await _authenticationManager.CurrentUser();
            _canCreateUsers = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Users.Create)).Succeeded;
            _canSearchUsers = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Users.Search)).Succeeded;
            _canExportUsers = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Users.Export)).Succeeded;
            _canViewRoles = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Roles.View)).Succeeded;

            await GetUsersAsync();
            _loaded = true;
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
                foreach (var message in response.Errors)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }

        private bool Search(UserDto user)
        {
            if (string.IsNullOrWhiteSpace(_searchString)) return true;
            if (user.FirstName?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
            {
                return true;
            }
            if (user.LastName?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
            {
                return true;
            }
            if (user.Email?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
            {
                return true;
            }
            if (user.PhoneNumber?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
            {
                return true;
            }
            if (user.UserName?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
            {
                return true;
            }
            return false;
        }

        //private async Task ExportToExcel()
        //{
        //    var base64 = await _userManager.ExportToExcelAsync(_searchString);
        //    await _jsRuntime.InvokeVoidAsync("Download", new
        //    {
        //        ByteArray = base64,
        //        FileName = $"{nameof(Users).ToLower()}_{DateTime.Now:ddMMyyyyHHmmss}.xlsx",
        //        MimeType = ApplicationConstants.MimeTypes.OpenXml
        //    });
        //    _snackBar.Add(string.IsNullOrWhiteSpace(_searchString)
        //        ? _localizer["Users exported"]
        //        : _localizer["Filtered Users exported"], Severity.Success);
        //}

        //private async Task InvokeModal()
        //{
        //    var parameters = new DialogParameters();
        //    var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true };
        //    var dialog = await _dialogService.ShowAsync<RegisterUserModal>(_localizer["Register New User"], parameters, options);
        //    var result = await dialog.Result;
        //    if (result != null && !result.Canceled)
        //    {
        //        await GetUsersAsync();
        //    }
        //}

        private void ViewProfile(string userId)
        {
            _navigationManager.NavigateTo($"/user-profile/{userId}");
        }

        private void ManageRoles(string userId, string email)
        {
            if (email == "3aef4452-5f15-42b5-8d5c-9eaab0b23476") _snackBar.Add(_localizer["Not Allowed."], Severity.Error);
            else _navigationManager.NavigateTo($"/admin/user-roles/{userId}");
        }
    }
}
