using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;
using NorthStandard.Testing.Playwright.Domain.Abstractions;
using NorthStandard.Testing.Playwright.Infrastructure.Configuration;
using NorthStandard.Testing.Playwright.Infrastructure.Lifecycle;
using NorthStandard.Testing.Playwright.Infrastructure.Providers;

namespace NorthStandard.Testing.Playwright.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers Playwright services with the provided configuration
    /// </summary>
    public static IServiceCollection AddPlaywrightServices(
    this IServiceCollection services,
    IConfiguration configuration)
    {
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