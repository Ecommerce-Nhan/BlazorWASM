using Microsoft.AspNetCore.Components;
using MudBlazor;
using SharedLibrary.Requests.Identity;

namespace Ecommerce.Pages.Authentication;

public partial class Login : ComponentBase
{
    [Inject] private ILogger<Login> _logger { get; set; } = default!;
    private TokenRequest _tokenModel = new();
    protected override async Task OnInitializedAsync()
    {
        var state = await _stateProvider.GetAuthenticationStateAsync();
        if (state.User.Identity?.IsAuthenticated == true)
        {
            _navigationManager.NavigateTo("/");
        }
    }

    private async Task SubmitAsync()
    {
        try
        {
            var result = await _authenticationManager.Login(_tokenModel);
            if (!result.Succeeded)
            {
                foreach (var message in result.Errors ?? [])
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
            else
            {
                _navigationManager.Refresh();
            }
        }
        catch (Exception ex)
        {
            _snackBar.Add($"Server error.", Severity.Error);
            _logger.LogError($"Login failed {ex.Message}");
        }
    }

    private void FillAdministratorCredentials()
    {
        _tokenModel.Email = "superadmin@gmail.com";
        _tokenModel.Password = "123Pa$$word!";
    }

    private void LoginWithGoogle()
    {
        var googleOAuthUrl =
            "https://accounts.google.com/o/oauth2/v2/auth" +
            "?client_id=988669802858-5gb5ogt2h31tf9978481g91lggrhvkds.apps.googleusercontent.com" +
            "&redirect_uri=https://ecommerce.tranthanhnhan.click" +
            "&response_type=code" +
            "&scope=openid";

        _navigationManager.NavigateTo(googleOAuthUrl, true);
    }
}