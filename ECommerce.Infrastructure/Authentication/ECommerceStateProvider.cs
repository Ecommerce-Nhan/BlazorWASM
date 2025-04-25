using Blazored.LocalStorage;
using ECommerce.Infrastructure.Extensions;
using ECommerce.Infrastructure.Routes;
using Microsoft.AspNetCore.Components.Authorization;
using SharedLibrary.Constants.Permission;
using SharedLibrary.Constants.Storage;
using SharedLibrary.Wrappers;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;

namespace ECommerce.Infrastructure.Authentication;

public class ECommerceStateProvider(
    HttpClient httpClient,
    ILocalStorageService localStorage) : AuthenticationStateProvider
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly ILocalStorageService _localStorage = localStorage;

    public async Task StateChangedAsync()
    {
        var authState = Task.FromResult(await GetAuthenticationStateAsync());

        NotifyAuthenticationStateChanged(authState);

    }

    public void MarkUserAsLoggedOut()
    {
        var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
        var authState = Task.FromResult(new AuthenticationState(anonymousUser));

        NotifyAuthenticationStateChanged(authState);
    }

    public async Task<ClaimsPrincipal> GetAuthenticationStateProviderUserAsync()
    {
        var state = await this.GetAuthenticationStateAsync();
        var authenticationStateProviderUser = state.User;
        return authenticationStateProviderUser;
    }

    public ClaimsPrincipal AuthenticationStateUser { get; set; } = default!;

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var savedToken = await _localStorage.GetItemAsync<string>(StorageConstants.Local.AuthToken);
        var payload = await _localStorage.GetItemAsync<string>(StorageConstants.Local.PayloadToken);
        if (string.IsNullOrWhiteSpace(savedToken) || string.IsNullOrWhiteSpace(payload))
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", savedToken);
        var state = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(GetClaimsFromJwt(payload), "jwt")));
        AuthenticationStateUser = state.User;
        return state;
    }

    private IEnumerable<Claim> GetClaimsFromJwt(string jwt)
    {
        var claims = new List<Claim>();
        var payload = jwt.Split('.')[1];
        var jsonBytes = ParseBase64WithoutPadding(payload);
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
        if (keyValuePairs is null) return claims;

        keyValuePairs.TryGetValue(ClaimTypes.Role, out var roles);

        if (roles is JsonElement rolesJson)
        {
            var parseRoles = JsonSerializer.Deserialize<string>(rolesJson);

            if (parseRoles is string rolesString)
            {
                var roleArray = rolesString.Split(',')
                                       .Select(role => role.Trim())
                                       .ToArray();
                claims.AddRange(roleArray.Select(role => new Claim(ClaimTypes.Role, role)));
            }

            keyValuePairs.Remove(ClaimTypes.Role);
        }

        keyValuePairs.TryGetValue(ApplicationClaimTypes.Permission, out var permissions);
        if (permissions is JsonElement permissionsJson)
        {
            var parsedPermissions = JsonSerializer.Deserialize<string>(permissionsJson);
            if (parsedPermissions is string permissionsString)
            {
                var permissionArray = permissionsString.Split(',')
                                      .Select(role => role.Trim())
                                      .ToArray();

                claims.AddRange(permissionArray.Select(permission => new Claim(ApplicationClaimTypes.Permission, permission)));
            }

            keyValuePairs.Remove(ApplicationClaimTypes.Permission);
        }

        claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString() ?? string.Empty)));

        return claims;
    }

    private byte[] ParseBase64WithoutPadding(string payload)
    {
        payload = payload.Trim().Replace('-', '+').Replace('_', '/');
        var base64 = payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4, '=');
        return Convert.FromBase64String(base64);
    }
}