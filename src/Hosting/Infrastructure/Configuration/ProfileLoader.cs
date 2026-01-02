using Microsoft.Extensions.Configuration;
using System;
using NorthStandard.Testing.Hosting.Infrastructure.Configuration;

namespace NorthStandard.Testing.Hosting.Infrastructure.Configuration
{
    /// <summary>
    /// Legacy profile loader - consider using ConfigurationExtensions.BindConfiguration instead
    /// </summary>
    public class ProfileLoader<T>(IConfiguration configuration) where T : class, new()
    {
        public T LoadProfile(string profileSection)
        {
            // Use the new extension method internally for consistency
            return configuration.BindConfigurationOrDefault<T>(profileSection);
        }
    }
}
