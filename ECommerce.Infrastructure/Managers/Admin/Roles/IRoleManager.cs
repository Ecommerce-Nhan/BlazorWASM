using SharedLibrary.Requests.Identity;
using SharedLibrary.Response.Identity;
using SharedLibrary.Wrappers;

namespace ECommerce.Infrastructure.Managers.Admin.Roles;

public interface IRoleManager : IManager
{
    Task<IResponse<List<RoleResponse>>> GetAllAsync();

    Task<IResponse<PermissionResponse>> GetPermissionsAsync(string roleId);

    Task<IResponse<string>> UpdatePermissionsAsync(PermissionRequest request);
}