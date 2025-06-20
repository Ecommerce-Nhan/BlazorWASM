namespace ECommerce.Infrastructure.Routes;

public static class RolesEndpoints
{
    public static string Default = "api/v1/user/role";
    public static string Delete = Default;
    public static string GetAll = Default;
    public static string Create = Default;
    public static string Update = Default;
    public static string GetPermissions = $"{Default}/permissions/";
    public static string UpdatePermissions = $"{Default}/permissions/update";
}