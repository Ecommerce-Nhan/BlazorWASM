using ECommerce.Infrastructure.Managers.Identity.Authentication;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Net;
using System.Net.Http.Headers;
using Toolbelt.Blazor;

namespace ECommerce.Infrastructure.Managers.Interceptors;

public class HttpInterceptorManager : IHttpInterceptorManager
{
    private readonly ISnackbar _snackbar;
    private readonly IAuthenticationManager _authenticationManager;
    private readonly HttpClientInterceptor _interceptor;
    private readonly NavigationManager _navigationManager;

    public HttpInterceptorManager(ISnackbar snackbar,
        IAuthenticationManager authenticationManager,
        HttpClientInterceptor interceptor,
        NavigationManager navigationManager)
    {
        _snackbar = snackbar;
        _interceptor = interceptor;
        _authenticationManager = authenticationManager;
        _navigationManager = navigationManager;
    }

    public void RegisterEvent()
    {
        _interceptor.BeforeSendAsync += InterceptBeforeHttpAsync;
        _interceptor.AfterSendAsync += InterceptAfterHttpAsync;
    }


    public async Task InterceptBeforeHttpAsync(object sender, HttpClientInterceptorEventArgs e)
    {
        var absPath = e.Request.RequestUri!.AbsolutePath;
        if (!absPath.Contains("token") && !absPath.Contains("accounts"))
        {
            try
            {
                var token = await _authenticationManager.TryRefreshToken();
                if (!string.IsNullOrEmpty(token))
                {
                    e.Request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await _authenticationManager.Logout();
                _navigationManager.NavigateTo("/");
            }
        }
    }

    public async Task InterceptAfterHttpAsync(object sender, HttpClientInterceptorEventArgs e)
    {
        var statusCode = e.Response?.StatusCode;

        if (statusCode.HasValue && ((int)statusCode.Value >= 400))
        {
            if (statusCode == HttpStatusCode.Unauthorized)
            {
                _snackbar.Add("Please login first.", Severity.Error);
                await _authenticationManager.Logout();
            }
            else
            {
                _snackbar.Add("Server error.", Severity.Error);
            }
            _navigationManager.NavigateTo("/");
        }
    }

    public void DisposeEvent()
    {
        _interceptor.BeforeSendAsync -= InterceptBeforeHttpAsync;
        _interceptor.AfterSendAsync -= InterceptAfterHttpAsync;
    }
}