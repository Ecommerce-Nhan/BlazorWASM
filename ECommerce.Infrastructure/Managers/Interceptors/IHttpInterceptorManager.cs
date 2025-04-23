using Toolbelt.Blazor;

namespace ECommerce.Infrastructure.Managers.Interceptors;

public interface IHttpInterceptorManager : IManager
{
    void RegisterEvent();

    Task InterceptBeforeHttpAsync(object sender, HttpClientInterceptorEventArgs e);

    void DisposeEvent();
}