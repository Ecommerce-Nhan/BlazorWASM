using Blazored.LocalStorage;
using Ecommerce.Infrastructure.Helpers;
using ECommerce.Infrastructure.Authentication;
using ECommerce.Infrastructure.Managers;
using ECommerce.Infrastructure.Managers.Preferences;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor;
using MudBlazor.Services;
using SharedLibrary.Constants.Permission;
using System.Globalization;
using System.Reflection;
using Toolbelt.Blazor.Extensions.DependencyInjection;

namespace Ecommerce.Extensions;

public static class WebAssemblyHostBuilderExtensions
{
    private const string ClientName = "ECommerce.API";
    public static WebAssemblyHostBuilder AddRootComponents(this WebAssemblyHostBuilder builder)
    {
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        return builder;
    }

    public static WebAssemblyHostBuilder AddClientServices(this WebAssemblyHostBuilder builder)
    {
        builder.Services
                .AddLocalization(options =>
                {
                    options.ResourcesPath = "Resources";
                })
               .AddAuthorizationCore(options =>
               {
                   RegisterPermissionClaims(options);
               })
               .AddBlazoredLocalStorage()
               .AddMudServices(configuration =>
               {
                   configuration.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopRight;
                   configuration.SnackbarConfiguration.HideTransitionDuration = 100;
                   configuration.SnackbarConfiguration.ShowTransitionDuration = 100;
                   configuration.SnackbarConfiguration.VisibleStateDuration = 3000;
                   configuration.SnackbarConfiguration.ShowCloseIcon = false;
               })
               .AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies())
               .AddScoped<ECommerceStateProvider>()
               .AddScoped<ClientPreferenceManager>()
               .AddScoped<AuthenticationStateProvider, ECommerceStateProvider>()
               .AddManagers()
               .AddTransient<AuthenticationHeaderHandler>()
               .AddScoped(sp => sp
                   .GetRequiredService<IHttpClientFactory>()
                   .CreateClient(ClientName).EnableIntercept(sp))
               .AddHttpClient(ClientName, client =>
               {
                   client.DefaultRequestHeaders.AcceptLanguage.Clear();
                   client.DefaultRequestHeaders.AcceptLanguage.ParseAdd(CultureInfo.DefaultThreadCurrentCulture?.TwoLetterISOLanguageName);
                   client.BaseAddress = new Uri("https://localhost:7000");
               })
               .AddHttpMessageHandler<AuthenticationHeaderHandler>();
        builder.Services.AddHttpClientInterceptor();
        builder.Services.AddSingleton<LoadingStateContainer>();

        return builder;
    }

    public static IServiceCollection AddManagers(this IServiceCollection services)
    {
        var managers = typeof(IManager);

        var types = managers
            .Assembly
            .GetExportedTypes()
            .Where(t => t.IsClass && !t.IsAbstract)
            .Select(t => new
            {
                Service = t.GetInterface($"I{t.Name}"),
                Implementation = t
            })
            .Where(t => t.Service != null);

        foreach (var type in types)
        {
            if (managers.IsAssignableFrom(type.Service))
            {
                services.AddTransient(type.Service, type.Implementation);
            }
        }

        return services;
    }

    private static void RegisterPermissionClaims(AuthorizationOptions options)
    {
        foreach (var prop in typeof(Permissions).GetNestedTypes().SelectMany(c => c.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)))
        {
            var propertyValue = prop.GetValue(null);
            if (propertyValue is string propertyValueString)
            {
                options.AddPolicy(propertyValueString, policy => policy.RequireClaim(ApplicationClaimTypes.Permission, propertyValueString));
            }
        }
    }
}