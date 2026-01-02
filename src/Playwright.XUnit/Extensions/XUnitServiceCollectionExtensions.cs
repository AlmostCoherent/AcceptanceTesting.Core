using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NorthStandard.Testing.Playwright.Extensions;

namespace NorthStandard.Testing.Playwright.XUnit.Extensions;

/// <summary>
/// Extension methods for registering Playwright with xUnit
/// </summary>
public static class XUnitServiceCollectionExtensions
{
    /// <summary>
    /// Adds Playwright services configured for xUnit
    /// </summary>
    public static IServiceCollection AddPlaywrightForXUnit(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        // Register core Playwright services
        services.AddPlaywrightServices(configuration);
        return services;
    }
}
