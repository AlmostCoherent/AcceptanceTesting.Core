using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace AlmostCoherent.Testing.Playwright.Extensions;

/// <summary>
/// Extension methods and utilities for building test configurations in Playwright projects.
/// Works with any test framework (Reqnroll, xUnit, MSTest, etc.)
/// </summary>
public static class ConfigurationExtensions
{
    /// <summary>
    /// Builds a configuration from appsettings.json in the output directory with environment-specific overrides.
    /// Automatically detects the environment from DOTNET_ENVIRONMENT or ASPNETCORE_ENVIRONMENT variables.
    /// Falls back to base appsettings.json if no environment is defined.
    /// </summary>
    /// <remarks>
    /// Expects appsettings.json to be copied to the bin output directory via .csproj configuration:
    /// <code>
    /// &lt;ItemGroup&gt;
    ///   &lt;None Update="appsettings.json;appsettings.Development.json"&gt;
    ///     &lt;CopyToOutputDirectory&gt;PreserveNewest&lt;/CopyToOutputDirectory&gt;
    ///   &lt;/None&gt;
    /// &lt;/ItemGroup&gt;
    /// </code>
    /// </remarks>
    /// <returns>An IConfiguration instance loaded from appsettings.json with optional environment-specific overrides</returns>
    public static IConfiguration BuildTestConfiguration()
    {
        // Detect environment from DOTNET_ENVIRONMENT or ASPNETCORE_ENVIRONMENT
        var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") 
            ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        var appsettingsPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
        
        var builder = new ConfigurationBuilder()
            .AddJsonFile(appsettingsPath, optional: false);
        
        // Load environment-specific config if environment is defined and file exists
        if (!string.IsNullOrEmpty(environment))
        {
            var environmentPath = Path.Combine(AppContext.BaseDirectory, $"appsettings.{environment}.json");
            if (File.Exists(environmentPath))
            {
                builder.AddJsonFile(environmentPath, optional: true);
            }
        }
        
        return builder
            .AddEnvironmentVariables()
            .Build();
    }

    /// <summary>
    /// Builds a configuration from appsettings.json in the output directory with specific environment overrides.
    /// </summary>
    /// <param name="environment">Environment name (e.g., "Development", "Production"). Loads appsettings.{environment}.json if it exists</param>
    /// <returns>An IConfiguration instance with environment-specific settings</returns>
    public static IConfiguration BuildTestConfiguration(string environment)
    {
        var appsettingsPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
        var environmentPath = Path.Combine(AppContext.BaseDirectory, $"appsettings.{environment}.json");
        
        var builder = new ConfigurationBuilder()
            .AddJsonFile(appsettingsPath, optional: false);
        
        // Load environment-specific config if it exists
        if (File.Exists(environmentPath))
        {
            builder.AddJsonFile(environmentPath, optional: true);
        }
        
        return builder
            .AddEnvironmentVariables()
            .Build();
    }
}
