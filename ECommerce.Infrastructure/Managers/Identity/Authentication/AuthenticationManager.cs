using Blazored.LocalStorage;
using ECommerce.Infrastructure.Authentication;
using ECommerce.Infrastructure.Routes;
using Microsoft.AspNetCore.Components.Authorization;
using SharedLibrary.Constants.Storage;
using SharedLibrary.Requests.Identity;
using SharedLibrary.Response.Identity;
using SharedLibrary.Wrappers;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection;
using System.Security.Claims;
using System.Text.Json;

namespace ECommerce.Infrastructure.Managers.Identity.Authentication;

public class AuthenticationManager : IAuthenticationManager
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;
    private readonly AuthenticationStateProvider _authenticationStateProvider;

    public AuthenticationManager(
        HttpClient httpClient,
        ILocalStorageService localStorage,
        AuthenticationStateProvider authenticationStateProvider)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
        _authenticationStateProvider = authenticationStateProvider;
    }

    public async Task<ClaimsPrincipal> CurrentUser()
    {
        var state = await _authenticationStateProvider.GetAuthenticationStateAsync();
        return state.User;
    }

    public async Task<IResponse> Login(TokenRequest model)
    {
        var requestData = new Dictionary<string, string>
        {
            ["grant_type"] = "password",
            ["username"] = model.Email,
            ["password"] = model.Password,
            ["client_id"] = "2ddc26af-9623-4f6d-9abb-9b412bae5ef5",
            ["client_secret"] = "388D45FA-B36B-4988-BA59-B187D329C207"
        };

        var content = new FormUrlEncodedContent(requestData);

        var response = await _httpClient.PostAsync(TokenEndpoints.Identity, content);
        var responseData = await response.Content.ReadFromJsonAsync<TokenResponse>();

        if (response.IsSuccessStatusCode && responseData is TokenResponse)
        {
            var token = responseData.Access_Token;
            var refreshToken = responseData.Refresh_Token;
            await _localStorage.SetItemAsync(StorageConstants.Local.AccessToken, token);
            await _localStorage.SetItemAsync(StorageConstants.Local.RefreshToken, refreshToken);

            await ((ECommerceStateProvider)this._authenticationStateProvider).StateChangedAsync();

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return await Response.SuccessAsync();
        }
        else
        {
            return await Response.FailAsync();
        }
    }

    public async Task<IResponse> Logout()
    {
        await _localStorage.RemoveItemAsync(StorageConstants.Local.AccessToken);
        await _localStorage.RemoveItemAsync(StorageConstants.Local.RefreshToken);
        ((ECommerceStateProvider)_authenticationStateProvider).MarkUserAsLoggedOut();
        _httpClient.DefaultRequestHeaders.Authorization = null;
        return await Response.SuccessAsync();
    }

    public async Task<string> RefreshTokenAsync()
    {
        var token = await _localStorage.GetItemAsync<string>(StorageConstants.Local.RefreshToken);
        var refreshToken = await _localStorage.GetItemAsync<string>(StorageConstants.Local.RefreshToken);
        var requestData = new
        {
            Token = token,
            RefreshToken = refreshToken
        };

        var response = await _httpClient.PostAsJsonAsync(TokenEndpoints.Identity, requestData);
        var responseData = await response.Content.ReadFromJsonAsync<TokenResponse>();

        if (response.IsSuccessStatusCode && responseData is TokenResponse)
        {
            await _localStorage.SetItemAsync(StorageConstants.Local.AccessToken, responseData.Access_Token);
            await _localStorage.SetItemAsync(StorageConstants.Local.RefreshToken, responseData.Refresh_Token);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", responseData.Access_Token);

            return responseData.Access_Token;
        }
        else
        {
            throw new Exception("Token refresh failed");
        }
    }

    public async Task<string> TryRefreshToken()
    {
        var availableToken = await _localStorage.GetItemAsync<string>(StorageConstants.Local.RefreshToken);
        if (string.IsNullOrEmpty(availableToken)) return string.Empty;
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        var exp = user.FindFirst(c => c.Type.Equals("exp"))?.Value;
        var expTime = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(exp));
        var timeUTC = DateTime.UtcNow;
        var diff = expTime - timeUTC;
        if (diff.TotalMinutes <= 1)
            return await RefreshTokenAsync();
        return string.Empty;
    }
}