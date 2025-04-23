using SharedLibrary.Requests.Identity;
using SharedLibrary.Wrappers;
using System.Security.Claims;

namespace ECommerce.Infrastructure.Managers.Identity.Authentication;

public interface IAuthenticationManager : IManager
{
    Task<IResponse> Login(TokenRequest model);

    Task<IResponse> Logout();

    Task<ClaimsPrincipal> CurrentUser();
}