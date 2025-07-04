using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using SharedLibrary.Constants.Permission;
using SharedLibrary.Constants.Storage;
using System.Net.Http.Headers;
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
        var savedToken = await _localStorage.GetItemAsync<string>(StorageConstants.Local.AccessToken);
        if (string.IsNullOrWhiteSpace(savedToken) || string.IsNullOrWhiteSpace(savedToken))
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", savedToken);
        var state = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(GetClaimsFromJwt(savedToken), "jwt")));
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

        if (roles is JsonElement rolesJson && rolesJson.ValueKind == JsonValueKind.Array)
        {
            claims.AddRange(rolesJson.EnumerateArray()
                                     .SelectMany(r => r.GetString()?.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? [])
                                     .Select(r => new Claim(ClaimTypes.Role, r.Trim()))
            );

            keyValuePairs.Remove(ClaimTypes.Role);
        }

        keyValuePairs.TryGetValue(ApplicationClaimTypes.Permission, out var permissions);
        if (permissions is JsonElement permissionsJson && permissionsJson.ValueKind == JsonValueKind.Array)
        {
            claims.AddRange(permissionsJson.EnumerateArray()
                                     .SelectMany(r => r.GetString()?.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? [])
                                     .Select(r => new Claim(ApplicationClaimTypes.Permission, r.Trim()))
            );

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