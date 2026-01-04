using System.Threading.Tasks;

namespace NorthStandard.Testing.Hosting.Domain.Abstractions
{
    /// <summary>
    /// Manages web application hosting lifecycle for testing scenarios
    /// </summary>
    public interface IWebTestingHostManager
    {
        /// <summary>
        /// Gets the base URL of the web application (either local or remote)
        /// </summary>
        string BaseUrl { get; }

        /// <summary>
        /// Initializes the host manager, preparing for startup
        /// </summary>
        void Initialize();

        /// <summary>
        /// Starts the web server if configured for local hosting
        /// </summary>
        Task StartAsync();

        /// <summary>
        /// Stops the web server if running
        /// </summary>
        Task StopAsync();
    }
}
