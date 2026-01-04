using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NorthStandard.Testing.Hosting.Domain.Abstractions;
using NorthStandard.Testing.Hosting.Infrastructure;
using NorthStandard.Testing.Hosting.Infrastructure.Configuration;
using NorthStandard.Testing.Playwright.Application.Services;
using NorthStandard.Testing.Playwright.Domain.Abstractions;
using NorthStandard.Testing.Playwright.Extensions;
using NorthStandard.Testing.Playwright.XUnit.Fixtures;
using NorthStandard.Testing.ScreenPlayFramework.Infrastructure.Extensions;
using Xunit;

namespace NorthStandard.Testing.Demos.Web.Acceptance;

/// <summary>
/// xUnit collection fixture to manage web server and Playwright for all tests
/// </summary>
public class WebServerFixture : IAsyncLifetime
{
    private IWebTestingHostManager? _hostManager;

    public PlaywrightFixture PlaywrightFixture { get; private set; } = null!;
    public UrlBuilder UrlBuilder { get; private set; } = null!;
    public IServiceProvider Services { get; private set; } = null!;
    public WebTestingProfile Profile { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        var config = BuildConfiguration();
        Profile = new WebTestingProfile(config);
        
        _hostManager = new WebTestingHostManager(
            Profile.Options,
            args => global::Web.Program.CreateWebHostBuilder(args, isTestHost: true));
        
        _hostManager.Initialize();
        
        UrlBuilder = new UrlBuilder(_hostManager.BaseUrl);
        Services = BuildServiceProvider(config);
        
        PlaywrightFixture = await CreatePlaywrightFixture();
        
        await _hostManager.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await DisposePlaywrightFixture();
        
        if (_hostManager != null)
        {
            await _hostManager.StopAsync();
        }
        
        DisposeServices();
    }

    private static IConfiguration BuildConfiguration()
    {
        return new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true)
            .AddEnvironmentVariables()
            .Build();
    }

    private IServiceProvider BuildServiceProvider(IConfiguration config)
    {
        var services = new ServiceCollection();
        
        services.AddLogging(c => c.AddConfiguration(config));
        services.AddSingleton(config);
        services.AddSingleton(UrlBuilder);
        
        services.AddPlaywrightServices(config);
        services.AddCoreScreenPlayFramework();
        services.AddScreenPlayFrameworkFromAssembly(typeof(WebServerFixture).Assembly);
        
        Profile.ConfigureServices(services, config);

        return services.BuildServiceProvider();
    }

    private async Task<PlaywrightFixture> CreatePlaywrightFixture()
    {
        var lifecycleManager = Services.GetRequiredService<ITestLifecycleManager>();
        var pageProvider = Services.GetRequiredService<IPlaywrightPageProvider>();
        
        var fixture = new PlaywrightFixture(lifecycleManager, pageProvider);
        await fixture.InitializeAsync();
        
        return fixture;
    }

    private async Task DisposePlaywrightFixture()
    {
        try
        {
            if (PlaywrightFixture != null)
            {
                await PlaywrightFixture.DisposeAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error disposing Playwright fixture: {ex.Message}");
        }
    }

    private void DisposeServices()
    {
        if (Services is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}

[CollectionDefinition("WebServer")]
public class WebServerCollection : ICollectionFixture<WebServerFixture>
{
}