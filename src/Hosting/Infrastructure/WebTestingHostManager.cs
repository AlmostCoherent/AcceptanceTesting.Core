using Microsoft.AspNetCore.Builder;
using NorthStandard.Testing.Hosting.Domain.Abstractions;
using NorthStandard.Testing.Hosting.Infrastructure.Configuration;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace NorthStandard.Testing.Hosting.Infrastructure
{
    /// <summary>
    /// Manages web application hosting for testing, handling both local and remote URLs
    /// </summary>
    public class WebTestingHostManager : IWebTestingHostManager, IAsyncDisposable
    {
        private WebApplication? _server;
        private TcpListener? _portReservation;
        private readonly WebTestingProfileOptions _options;
        private readonly Func<string[], WebApplication> _serverFactory;

        public string BaseUrl { get; private set; } = string.Empty;

        public WebTestingHostManager(
            WebTestingProfileOptions options,
            Func<string[], WebApplication> serverFactory)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _serverFactory = serverFactory ?? throw new ArgumentNullException(nameof(serverFactory));
        }

        /// <summary>
        /// Initializes the host manager, either reserving a port for local hosting or using remote URL
        /// </summary>
        public void Initialize()
        {
            if (_options.UseLocalAppInstance)
            {
                var port = ReservePort();
                BaseUrl = $"http://localhost:{port}/";
                Console.WriteLine($"Reserved local port: {port}");
            }
            else
            {
                BaseUrl = _options.BaseUrl;
                Console.WriteLine($"Configured for remote URL: {BaseUrl}");
            }
        }

        /// <summary>
        /// Starts the web server if UseLocalAppInstance is true
        /// </summary>
        public async Task StartAsync()
        {
            if (!_options.UseLocalAppInstance)
            {
                Console.WriteLine($"Using remote URL: {BaseUrl}");
                return;
            }

            Console.WriteLine($"Starting Web server on {BaseUrl}...");

            ReleasePortReservation();

            string[] args = new[] { $"--urls={BaseUrl}" };
            _server = _serverFactory(args);
            await _server.StartAsync();

            Console.WriteLine($"Web server running at {BaseUrl}");
        }

        /// <summary>
        /// Stops the web server if it's running
        /// </summary>
        public async Task StopAsync()
        {
            try
            {
                if (_server != null)
                {
                    Console.WriteLine("Stopping Web server...");
                    await _server.StopAsync();
                    await _server.DisposeAsync();
                    _server = null;
                    Console.WriteLine("Web server stopped.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error stopping server: {ex.Message}");
            }
        }

        public async ValueTask DisposeAsync()
        {
            await StopAsync();
            DisposePortReservation();
        }

        private int ReservePort()
        {
            _portReservation = new TcpListener(IPAddress.Loopback, 0);
            _portReservation.Start();
            return ((IPEndPoint)_portReservation.LocalEndpoint).Port;
        }

        private void ReleasePortReservation()
        {
            _portReservation?.Stop();
            _portReservation?.Dispose();
            _portReservation = null;
        }

        private void DisposePortReservation()
        {
            _portReservation?.Stop();
            _portReservation?.Dispose();
            _portReservation = null;
        }
    }
}
