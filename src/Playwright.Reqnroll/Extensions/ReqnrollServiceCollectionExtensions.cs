using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AlmostCoherent.Testing.Playwright.Extensions;
using AlmostCoherent.Testing.Playwright.Infrastructure.Configuration;

namespace AlmostCoherent.Testing.Playwright.Reqnroll.Extensions;

/// <summary>
/// Extension methods for registering Playwright with Reqnroll
/// </summary>
public static class ReqnrollServiceCollectionExtensions
{
    /// <summary>
    /// Adds Playwright services configured for Reqnroll with automatic configuration loading.
    /// </summary>
    /// <remarks>
    /// Automatically loads configuration from appsettings.json in the output directory.
    /// Configuration is loaded with environment-specific overrides based on DOTNET_ENVIRONMENT or ASPNETCORE_ENVIRONMENT.
    /// </remarks>
    public static IServiceCollection AddPlaywrightForReqnroll(this IServiceCollection services)
    {
        // Register core Playwright services with automatic configuration
        services.AddPlaywrightServices();
        
        // Reqnroll hooks are auto-discovered, no additional registration needed
        
        return services;
    }
}}
