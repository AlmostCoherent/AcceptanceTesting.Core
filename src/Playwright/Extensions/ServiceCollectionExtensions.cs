using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;
using AlmostCoherent.Testing.Playwright.Domain.Abstractions;
using AlmostCoherent.Testing.Playwright.Infrastructure.Configuration;
using AlmostCoherent.Testing.Playwright.Infrastructure.Lifecycle;
using AlmostCoherent.Testing.Playwright.Infrastructure.Providers;

namespace AlmostCoherent.Testing.Playwright.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers Playwright services using default configuration loading.
    /// Automatically builds configuration from appsettings.json in the output directory.
    /// </summary>
    /// <remarks>
    /// Configuration is loaded automatically from appsettings.json with environment-specific overrides.
    /// The environment is detected from DOTNET_ENVIRONMENT or ASPNETCORE_ENVIRONMENT variables.
    /// </remarks>
    public static IServiceCollection AddPlaywrightServices(this IServiceCollection services)
    {
        var configuration = ConfigurationExtensions.BuildTestConfiguration();
        services.AddSingleton(configuration);
        
        var playwrightConfig = configuration.GetPlaywrightConfiguration();
        services.AddSingleton(playwrightConfig);
        
        // Register BrowserTypeLaunchOptions
        services.AddSingleton(new BrowserTypeLaunchOptions
        {
            Headless = playwrightConfig.EnableHeadlessBrowser,
            Timeout = playwrightConfig.WaitTimeOut
        });
        
        services.AddSingleton<IPlaywrightBrowserProvider, PlaywrightBrowserProvider>();
        services.AddSingleton<IPlaywrightPageProvider, PlaywrightPageProvider>();
        services.AddSingleton<ITestLifecycleManager, PlaywrightLifecycleManager>();
        return services;
    }
}