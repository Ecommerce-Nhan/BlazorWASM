using SharedLibrary.Requests.Identity;
using SharedLibrary.Response.Identity;
using SharedLibrary.Wrappers;

namespace ECommerce.Infrastructure.Managers.Admin.RoleClaims;

public interface IRoleClaimManager : IManager
{
    Task<IResponse<List<RoleClaimResponse>>> GetRoleClaimsAsync();

    Task<IResponse<List<RoleClaimResponse>>> GetRoleClaimsByRoleIdAsync(string roleId);

    Task<IResponse<string>> CreateAsync(RoleClaimRequest role);

    Task<IResponse<string>> UpdateAsync(int id, RoleClaimRequest role);

    Task<IResponse<string>> DeleteAsync(int id);
}