using SharedLibrary.Dtos.Users;
using SharedLibrary.Requests.Identity;
using SharedLibrary.Response.Identity;
using SharedLibrary.Wrappers;

namespace ECommerce.Infrastructure.Managers.Admin.Users;

public interface IUserManager : IManager
{
    Task<PagedResponse<List<UserDto>>> GetAllAsync();

    Task<IResponse<UserDto>> GetAsync(string userId);

    Task<IResponse<UserRoleResponse>> GetRolesAsync(string userId);

    Task<IResponse> UpdateRolesAsync(UpdateUserRoleRequest request);
}