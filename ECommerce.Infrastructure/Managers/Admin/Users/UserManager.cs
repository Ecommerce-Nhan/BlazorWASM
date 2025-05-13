using ECommerce.Infrastructure.Extensions;
using SharedLibrary.Dtos.Users;
using SharedLibrary.Requests.Identity;
using SharedLibrary.Response.Identity;
using SharedLibrary.Wrappers;
using System.Net.Http.Json;

namespace ECommerce.Infrastructure.Managers.Admin.Users;

public class UserManager : IUserManager
{
    private readonly HttpClient _httpClient;

    public UserManager(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<PagedResponse<List<UserDto>>> GetAllAsync()
    {
        var response = await _httpClient.GetAsync(Routes.UserEndpoints.GetAll);
        return await response.ToPaginatedResponse<List<UserDto>>();
    }

    public async Task<IResponse<UserDto>> GetAsync(string userId)
    {
        var response = await _httpClient.GetAsync($"{Routes.UserEndpoints.Get}/{userId}");
        return await response.ToResponse<UserDto>();
    }

    public async Task<IResponse<UserRoleResponse>> GetRolesAsync(string userId)
    {
        var response = await _httpClient.GetAsync(Routes.UserEndpoints.UserRoles(userId));
        return await response.ToResponse<UserRoleResponse>();
    }

    public async Task<IResponse> UpdateRolesAsync(UpdateUserRoleRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync(Routes.UserEndpoints.UserRoles(request.UserId), request);
        return await response.ToResponse<UserRoleResponse>();
    }
}