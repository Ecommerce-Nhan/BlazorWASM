namespace ECommerce.Infrastructure.Routes;

public static class UserEndpoints
{
    public static string Default = "api/v1/user";
    public static string GetAll = Default;
    public static string Get = Default;
    public static string Register = Default;

    public static string UserRoles(string userId)
    {
        return $"{Default}/user-role/{userId}";
    }
}