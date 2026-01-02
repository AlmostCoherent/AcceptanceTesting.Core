using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NorthStandard.Testing.Playwright.Application.Services;
using NorthStandard.Testing.Playwright.Reqnroll.Extensions;
using NorthStandard.Testing.ScreenPlayFramework.Infrastructure.Extensions;
using Reqnroll;
using System.Net;
using System.Net.Sockets;

namespace NorthStandard.Testing.Demos.Web.Acceptance
{
	[Binding]
	public class Bootstrap
	{
		private static WebApplication? server;
		private static int port;
		private static TcpListener? portReservation;

		[BeforeTestRun(Order = 0)]
		public static void BeforeTestRunSetupServices()
		{
			// Reserve port for NCrunch-safe parallel execution
			portReservation = new TcpListener(IPAddress.Loopback, 0);
			portReservation.Start();
			port = ((IPEndPoint)portReservation.LocalEndpoint).Port;

			var config = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json")
				.AddEnvironmentVariables()
				.Build();

			var loggerFactory = LoggerFactory.Create(c => c.AddConfiguration(config));
			var log = loggerFactory.CreateLogger<Bootstrap>();

			log.LogInformation("\r\nStarting Web acceptance test run\r\n");

			ServicesCollectionProvider
				.CreateServices()
				.AddLogging()
				.AddSingleton<IConfiguration>(config)
				.AddSingleton(new UrlBuilder($"http://localhost:{port}/"))
				.AddPlaywrightForReqnroll(config)
				.AddCoreScreenPlayFramework()
				.AddScreenPlayFrameworkFromAssembly(typeof(Bootstrap).Assembly);
		}

		[BeforeTestRun(Order = 1)]
		public static async Task BeforeTestRunConfigureAndRunApp(UrlBuilder urlBuilder)
		{
			Console.WriteLine($"Starting Web server on {urlBuilder.GetBaseUrl()}...");

			// Release port reservation BEFORE starting server so it can bind
			portReservation?.Stop();
			portReservation?.Dispose();
			portReservation = null;

			string[] args = new[] { $"--urls={urlBuilder.GetBaseUrl()}" };

			server = global::Web.Program.CreateWebHostBuilder(args, isTestHost: true);
			await server.StartAsync();
		}

		[AfterTestRun(Order = 0)]
		public static async Task AfterTestRunTestTeardownConfigurationAndApp()
		{
			try
			{
				if (server != null)
				{
					Console.WriteLine("Stopping Web server...");
					await server.StopAsync();
					await server.DisposeAsync();
					server = null;
					Console.WriteLine("Web server stopped.");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error stopping server: {ex.Message}");
			}
			finally
			{
				// Cleanup port reservation if something went wrong
				portReservation?.Stop();
				portReservation?.Dispose();
				portReservation = null;
			}
		}
	}
}
