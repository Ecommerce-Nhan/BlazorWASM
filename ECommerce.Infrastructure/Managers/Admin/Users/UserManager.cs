using ECommerce.Infrastructure.Extensions;
using SharedLibrary.Dtos.Users;
using SharedLibrary.Wrappers;

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
}