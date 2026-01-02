using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NorthStandard.Testing.Playwright.Application.Services;
using NorthStandard.Testing.Playwright.Domain.Abstractions;
using NorthStandard.Testing.Playwright.Extensions;
using NorthStandard.Testing.Playwright.XUnit.Fixtures;
using NorthStandard.Testing.ScreenPlayFramework.Infrastructure.Extensions;
using System.Net;
using System.Net.Sockets;
using Xunit;

namespace NorthStandard.Testing.Demos.Web.Acceptance;

/// <summary>
/// xUnit collection fixture to manage web server and Playwright for all tests
/// </summary>
public class WebServerFixture : IAsyncLifetime
{
	private WebApplication? _server;
	private int _port;
	private TcpListener? _portReservation;

	public PlaywrightFixture PlaywrightFixture { get; private set; } = null!;
	public UrlBuilder UrlBuilder { get; private set; } = null!;
	public IServiceProvider Services { get; private set; } = null!;

	public async Task InitializeAsync()
	{
		// Reserve port
		_portReservation = new TcpListener(IPAddress.Loopback, 0);
		_portReservation.Start();
		_port = ((IPEndPoint)_portReservation.LocalEndpoint).Port;

		var config = new ConfigurationBuilder()
			.AddJsonFile("appsettings.json")
			.AddEnvironmentVariables()
			.Build();

		// Build services
		var services = new ServiceCollection();
		services.AddLogging(c => c.AddConfiguration(config));
		services.AddSingleton<IConfiguration>(config);
		
		UrlBuilder = new UrlBuilder($"http://localhost:{_port}/");
		services.AddSingleton(UrlBuilder);
		
		// Add Playwright for xUnit
		services.AddPlaywrightServices(config);
		
		// Add ScreenPlay framework
		services.AddCoreScreenPlayFramework();
		services.AddScreenPlayFrameworkFromAssembly(typeof(WebServerFixture).Assembly);

		Services = services.BuildServiceProvider();

		// Create Playwright fixture manually with DI dependencies
		var lifecycleManager = Services.GetRequiredService<ITestLifecycleManager>();
		var pageProvider = Services.GetRequiredService<IPlaywrightPageProvider>();
		PlaywrightFixture = new PlaywrightFixture(lifecycleManager, pageProvider);
		await PlaywrightFixture.InitializeAsync();

		// Start web server
		Console.WriteLine($"Starting Web server on {UrlBuilder.GetBaseUrl()}...");
		
		_portReservation?.Stop();
		_portReservation?.Dispose();
		_portReservation = null;

		string[] args = new[] { $"--urls={UrlBuilder.GetBaseUrl()}" };
		_server = global::Web.Program.CreateWebHostBuilder(args, isTestHost: true);
		await _server.StartAsync();

		Console.WriteLine($"Web server running at {UrlBuilder.GetBaseUrl()}");
	}

	public async Task DisposeAsync()
	{
		try
		{
			await PlaywrightFixture.DisposeAsync();

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
			Console.WriteLine($"Error during cleanup: {ex.Message}");
		}
		finally
		{
			_portReservation?.Stop();
			_portReservation?.Dispose();
			_portReservation = null;

			if (Services is IDisposable disposable)
			{
				disposable.Dispose();
			}
		}
	}
}

[CollectionDefinition("WebServer")]
public class WebServerCollection : ICollectionFixture<WebServerFixture>
{
}
