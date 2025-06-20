using Blazored.LocalStorage;
using SharedLibrary.Constants.Storage;
using System.Net.Http.Headers;

namespace ECommerce.Infrastructure.Authentication;

public class AuthenticationHeaderHandler : DelegatingHandler
{
    private readonly ILocalStorageService localStorage;

    public AuthenticationHeaderHandler(ILocalStorageService localStorage)
        => this.localStorage = localStorage;

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (request.Headers?.Authorization?.Scheme != "Bearer")
        {
            var savedToken = await this.localStorage.GetItemAsync<string>(StorageConstants.Local.AccessToken);

            if (request.Headers is not null && !string.IsNullOrWhiteSpace(savedToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", savedToken);
            }
        }

        return await base.SendAsync(request, cancellationToken);
    }
}