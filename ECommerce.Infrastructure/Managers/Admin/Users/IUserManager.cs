using SharedLibrary.Dtos.Users;
using SharedLibrary.Wrappers;

namespace ECommerce.Infrastructure.Managers.Admin.Users;

public interface IUserManager : IManager
{
    Task<PagedResponse<List<UserDto>>> GetAllAsync();
}