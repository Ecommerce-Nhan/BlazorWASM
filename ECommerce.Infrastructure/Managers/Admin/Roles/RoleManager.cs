using ECommerce.Infrastructure.Extensions;
using SharedLibrary.Requests.Identity;
using SharedLibrary.Response.Identity;
using SharedLibrary.Wrappers;

namespace ECommerce.Infrastructure.Managers.Admin.Roles;

public class RoleManager : IRoleManager
{
    private readonly HttpClient _httpClient;

    public RoleManager(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<RoleResponse>> GetAllAsync()
    {
        var response = await _httpClient.GetAsync(Routes.RolesEndpoints.GetAll);
        return await response.ToSelfResponse<List<RoleResponse>>();
    }

    public async Task<IResponse<PermissionResponse>> GetPermissionsAsync(string roleId)
    {
        throw new NotImplementedException();
    }

    public async Task<IResponse<string>> UpdatePermissionsAsync(PermissionRequest request)
    {
        throw new NotImplementedException();
    }
}