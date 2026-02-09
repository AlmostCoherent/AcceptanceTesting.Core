using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NorthStandard.Testing.Playwright.Application.Services;
using NorthStandard.Testing.Playwright.Reqnroll.Extensions;
using NorthStandard.Testing.ScreenPlayFramework.Infrastructure.Extensions;
using Reqnroll;
using System.Net;
using System.Net.Sockets;
namespace NorthStandard.Testing.Demos.Api.Acceptance
{
    [Binding]
    public class Bootstrap
    {
        private static IHost? server;
        private static int port;

        [BeforeTestRun(Order = 0)]
        public static void BeforeTestRunSetupServices()
        {
            port = GetFreeTcpPort(); // Get a free port dynamically

            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            var loggerFactory = LoggerFactory.Create(c => c.AddConfiguration(config));
            var log = loggerFactory.CreateLogger<Bootstrap>();

            log.LogInformation("\r\nStarting test run\r\n");

            ServicesCollectionProvider
                .CreateServices()
                .AddLogging()
                .AddSingleton(config)
                .AddSingleton(new UrlBuilder($"https://localhost:{port}/")) // ✅ Inject URL with dynamic port
                .AddPlaywrightForReqnroll(config)
                .AddCoreScreenPlayFramework()
                .AddScreenPlayFrameworkFromAssembly(typeof(Bootstrap).Assembly);
        }

        [BeforeTestRun(Order = 1)]
        public static async Task BeforeTestRunConfigureAndRunApp(UrlBuilder urlBuilder)
        {
            Console.WriteLine($"Starting API server on {urlBuilder.GetBaseUrl()}...");
            string[] args = new[] { $"--urls={urlBuilder.GetBaseUrl()}" };
            // Use the factory provider pattern to create the host without exposing the method directly
            server = ApiHostFactory.CreateHost(args);
            await server.StartAsync();

            Console.WriteLine($"API server running at {urlBuilder.GetBaseUrl()}/");
        }

        [AfterTestRun]
        public static async Task AfterTestRunTestTeardownConfigurationAndApp()
        {
            if (server != null)
            {
                Console.WriteLine("Stopping API server...");
                await server.StopAsync();
                Console.WriteLine("API server stopped.");
            }
        }

        private static int GetFreeTcpPort()
        {
            using var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            int port = ((IPEndPoint)listener.LocalEndpoint).Port;
            return port;
        }
    }
}
