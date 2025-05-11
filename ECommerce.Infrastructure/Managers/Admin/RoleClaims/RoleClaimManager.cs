using ECommerce.Infrastructure.Extensions;
using SharedLibrary.Requests.Identity;
using SharedLibrary.Response.Identity;
using SharedLibrary.Wrappers;
using System.Net.Http.Json;

namespace ECommerce.Infrastructure.Managers.Admin.RoleClaims;

public class RoleClaimManager : IRoleClaimManager
{
    private readonly HttpClient _httpClient;

    public RoleClaimManager(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IResponse<string>> DeleteAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"{Routes.RoleClaimsEndpoints.Delete}/{id}");
        return await response.ToResponse<string>();
    }

    public async Task<IResponse<List<RoleClaimResponse>>> GetRoleClaimsAsync()
    {
        var response = await _httpClient.GetAsync(Routes.RoleClaimsEndpoints.GetAll);
        return await response.ToResponse<List<RoleClaimResponse>>();
    }

    public async Task<IResponse<List<RoleClaimResponse>>> GetRoleClaimsByRoleIdAsync(string roleId)
    {
        var response = await _httpClient.GetAsync($"{Routes.RoleClaimsEndpoints.GetAll}/{roleId}");
        return await response.ToResponse<List<RoleClaimResponse>>();
    }

    public async Task<IResponse<string>> CreateAsync(RoleClaimRequest role)
    {
        var response = await _httpClient.PostAsJsonAsync(Routes.RoleClaimsEndpoints.Create, role);
        return await response.ToResponse<string>();
    }

    public async Task<IResponse<string>> UpdateAsync(int id, RoleClaimRequest role)
    {
        var response = await _httpClient.PutAsJsonAsync($"{Routes.RoleClaimsEndpoints.Update}/{id}", role);
        return await response.ToResponse<string>();
    }
}