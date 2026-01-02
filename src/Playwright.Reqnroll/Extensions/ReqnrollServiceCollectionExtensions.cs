using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NorthStandard.Testing.Playwright.Extensions;
using NorthStandard.Testing.Playwright.Infrastructure.Configuration;

namespace NorthStandard.Testing.Playwright.Reqnroll.Extensions;

/// <summary>
/// Extension methods for registering Playwright with Reqnroll
/// </summary>
public static class ReqnrollServiceCollectionExtensions
{
    /// <summary>
    /// Adds Playwright services configured for Reqnroll
    /// </summary>
    public static IServiceCollection AddPlaywrightForReqnroll(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        // Register core Playwright services
        services.AddPlaywrightServices(configuration);
        
        // Reqnroll hooks are auto-discovered, no additional registration needed
        
        return services;
    }
}
