using AutoMapper;
using SharedLibrary.Requests.Identity;
using SharedLibrary.Response.Identity;

namespace ECommerce.Infrastructure.Mappings;

public class RoleProfile : Profile
{
    public RoleProfile()
    {
        CreateMap<PermissionResponse, PermissionRequest>().ReverseMap();
        CreateMap<RoleClaimResponse, RoleClaimRequest>().ReverseMap();
    }
}