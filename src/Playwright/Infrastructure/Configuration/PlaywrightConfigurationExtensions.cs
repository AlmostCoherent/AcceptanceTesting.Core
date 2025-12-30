using Microsoft.Extensions.Configuration;

namespace NorthStandard.Testing.Playwright.Infrastructure.Configuration
{
    public static class PlaywrightConfigurationExtensions
    {
        /// <summary>
        /// Binds Playwright configuration from appsettings with fallback to defaults
        /// </summary>
        public static PlaywrightConfiguration GetPlaywrightConfiguration(this IConfiguration configuration, string section = "Playwright")
        {
            var config = new PlaywrightConfiguration();
            configuration.GetSection(section).Bind(config);
            return config;
        }
    }
}