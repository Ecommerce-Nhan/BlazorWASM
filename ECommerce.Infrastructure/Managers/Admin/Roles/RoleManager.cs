using ECommerce.Infrastructure.Extensions;
using SharedLibrary.Requests.Identity;
using SharedLibrary.Response.Identity;
using SharedLibrary.Wrappers;
using System.Net.Http.Json;

namespace ECommerce.Infrastructure.Managers.Admin.Roles;

public class RoleManager : IRoleManager
{
    private readonly HttpClient _httpClient;

    public RoleManager(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IResponse<List<RoleResponse>>> GetAllAsync()
    {
        var response = await _httpClient.GetAsync(Routes.RolesEndpoints.GetAll);
        return await response.ToResponse<List<RoleResponse>>();
    }

    public async Task<IResponse<string>> DeleteAsync(string id)
    {
        var response = await _httpClient.DeleteAsync($"{Routes.RolesEndpoints.Delete}/{id}");
        return await response.ToResponse<string>();
    }

    public async Task<IResponse<string>> CreateAsync(RoleRequest role)
    {
        var response = await _httpClient.PostAsJsonAsync(Routes.RolesEndpoints.Create, role);
        return await response.ToResponse<string>();
    }

    public async Task<IResponse<string>> UpdateAsync(string id, RoleRequest role)
    {
        var response = await _httpClient.PutAsJsonAsync($"{Routes.RolesEndpoints.Update}/{id}", role);
        return await response.ToResponse<string>();
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