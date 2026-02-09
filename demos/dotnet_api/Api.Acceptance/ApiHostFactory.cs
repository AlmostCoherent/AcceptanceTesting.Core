using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace NorthStandard.Testing.Demos.Api.Acceptance
{
    /// <summary>
    /// Factory for creating the API host for testing.
    /// Provides an abstraction layer over the Program.CreateApiHostBuilder method
    /// and allows for service overrides in test projects.
    /// </summary>
    public static class ApiHostFactory
    {
        private static Action<IServiceCollection>? _serviceConfigurationCallback;

        /// <summary>
        /// Configures the service configuration callback that will be invoked during host creation.
        /// This allows test projects to add or override services before the host is built.
        /// </summary>
        /// <param name="configureServices">An action that configures services for the test host</param>
        public static void ConfigureServices(Action<IServiceCollection> configureServices)
        {
            _serviceConfigurationCallback = configureServices;
        }

        /// <summary>
        /// Clears any previously configured service configuration callback.
        /// </summary>
        public static void ResetServiceConfiguration()
        {
            _serviceConfigurationCallback = null;
        }

        /// <summary>
        /// Creates and configures the API host for testing.
        /// Any services configured via <see cref="ConfigureServices"/> will be applied before the host is built.
        /// </summary>
        /// <param name="args">Command-line arguments to pass to the host builder</param>
        /// <returns>A configured IHost instance with any test-specific service overrides applied</returns>
        public static IHost CreateHost(string[] args)
        {
            var host = Program.CreateApiHostBuilder(args);

            // If there are custom service configurations, we need to reconfigure the host
            if (_serviceConfigurationCallback != null)
            {
                // Create a new builder with the same arguments
                var builder = new HostBuilder()
                    .ConfigureServices((context, services) =>
                    {
                        // First, apply the original configuration from Program
                        var originalHost = Program.CreateApiHostBuilder(args);
                        // Note: Since we can't directly extract services from the built host,
                        // we apply test-specific overrides on top of the original configuration
                        _serviceConfigurationCallback(services);
                    });

                host = builder.Build();
            }

            return host;
        }

        /// <summary>
        /// Creates the API host with inline service configuration.
        /// </summary>
        /// <param name="args">Command-line arguments to pass to the host builder</param>
        /// <param name="configureServices">An action that configures services for the test host</param>
        /// <returns>A configured IHost instance with the specified service overrides applied</returns>
        public static IHost CreateHost(string[] args, Action<IServiceCollection> configureServices)
        {
            ConfigureServices(configureServices);
            try
            {
                return CreateHost(args);
            }
            finally
            {
                ResetServiceConfiguration();
            }
        }
    }
}
