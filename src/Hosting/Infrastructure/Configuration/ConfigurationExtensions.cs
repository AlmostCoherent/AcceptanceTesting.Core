using Microsoft.Extensions.Configuration;

namespace NorthStandard.Testing.Hosting.Infrastructure.Configuration
{
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Binds configuration to a strongly-typed object using Microsoft's built-in configuration binding
        /// </summary>
        public static T BindConfiguration<T>(this IConfiguration configuration, string section) where T : new()
        {
            var instance = new T();
            configuration.GetSection(section).Bind(instance);
            return instance;
        }

        /// <summary>
        /// Binds configuration with fallback defaults if section doesn't exist
        /// </summary>
        public static T BindConfigurationOrDefault<T>(this IConfiguration configuration, string section) where T : new()
        {
            var configSection = configuration.GetSection(section);
            if (!configSection.Exists())
            {
                return new T();
            }
            
            var instance = new T();
            configSection.Bind(instance);
            return instance;
        }
    }
}