using Microsoft.AspNetCore.Components;
using System.Net.NetworkInformation;
using System.Web;

namespace Ecommerce.Pages.Authentication;

public partial class LoginGoogle : ComponentBase
{
    private string? _status;
    protected override async Task OnInitializedAsync()
    {
        var uri = _navigationManager.ToAbsoluteUri(_navigationManager.Uri);
        var code = HttpUtility.ParseQueryString(uri.Query).Get("code");

        if (string.IsNullOrWhiteSpace(code))
        {
            _status = "Google login failed: missing code.";
            return;
        }
    }
}