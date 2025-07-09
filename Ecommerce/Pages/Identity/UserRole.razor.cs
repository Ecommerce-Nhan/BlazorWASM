using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using SharedLibrary.Constants.Permission;
using SharedLibrary.Requests.Identity;
using SharedLibrary.Response.Identity;

namespace Ecommerce.Pages.Identity
{
    public partial class UserRole
    {
        [Parameter] public string Id { get; set; } = default!;
        [Parameter] public string Title { get; set; } = default!;
        [Parameter] public string Description { get; set; } = default!;
        public List<UserRoleModel> UserRolesList { get; set; } = new();

        private UserRoleModel _userRole = new();
        private string _searchString = "";

        private ClaimsPrincipal _currentUser = default!;
        private bool _canEditUsers;
        private bool _canSearchRoles;
        private bool _loaded;

        protected override async Task OnInitializedAsync()
        {
            _currentUser = await _authenticationManager.CurrentUser();
            _canEditUsers = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Users.Edit)).Succeeded;
            _canSearchRoles = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Roles.Search)).Succeeded;

            var userId = Id;
            var result = await _userManager.GetAsync(userId);
            if (result.Succeeded)
            {
                var user = result.Data;
                if (user != null)
                {
                    Title = $"{user.FirstName} {user.LastName}";
                    Description = string.Format(_localizer["Manage {0} {1}'s Roles"], user.FirstName, user.LastName);
                    var response = await _userManager.GetRolesAsync(user.Id);
                    UserRolesList = response.Data.UserRoles;
                }
            }

            _loaded = true;
        }

        private async Task SaveAsync()
        {
            var request = new UpdateUserRoleRequest()
            {
                UserId = Id,
                UserRoles = UserRolesList
            };
            var result = await _userManager.UpdateRolesAsync(request);
            if (result.Succeeded)
            {
                _snackBar.Add(result.Message, Severity.Success);
                _navigationManager.NavigateTo("/admin/user");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    _snackBar.Add(error, Severity.Error);
                }
            }
        }

        private bool Search(UserRoleModel userRole)
        {
            if (string.IsNullOrWhiteSpace(_searchString)) return true;
            if (userRole.RoleName?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
            {
                return true;
            }
            if (userRole.RoleDescription?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
            {
                return true;
            }
            return false;
        }
    }
}
