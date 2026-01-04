using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NorthStandard.Testing.Hosting.Domain.Abstractions;
using NorthStandard.Testing.Hosting.Infrastructure;
using NorthStandard.Testing.Hosting.Infrastructure.Configuration;
using NorthStandard.Testing.Playwright.Application.Services;
using NorthStandard.Testing.Playwright.Reqnroll.Extensions;
using NorthStandard.Testing.ScreenPlayFramework.Infrastructure.Extensions;
using Reqnroll;

namespace NorthStandard.Testing.Demos.Web.Acceptance
{
    [Binding]
    public class Bootstrap
    {
        private static IWebTestingHostManager? hostManager;
        private static WebTestingProfile? profile;

        [BeforeTestRun(Order = 0)]
        public static void BeforeTestRunSetupServices()
        {
            var config = BuildConfiguration();
            profile = new WebTestingProfile(config);

            hostManager = new WebTestingHostManager(
                profile.Options,
                args => global::Web.Program.CreateWebHostBuilder(args, isTestHost: true));

            hostManager.Initialize();

            var urlBuilder = new UrlBuilder(hostManager.BaseUrl);

            var loggerFactory = LoggerFactory.Create(c => c.AddConfiguration(config));
            var log = loggerFactory.CreateLogger<Bootstrap>();

            log.LogInformation("\r\nStarting Web acceptance test run\r\n");

            var services = ServicesCollectionProvider
                .CreateServices()
                .AddLogging()
                .AddSingleton(config)
                .AddSingleton(urlBuilder)
                .AddPlaywrightForReqnroll(config)
                .AddCoreScreenPlayFramework()
                .AddScreenPlayFrameworkFromAssembly(typeof(Bootstrap).Assembly);

            profile.ConfigureServices(services, config);
        }

        [BeforeTestRun(Order = 1)]
        public static async Task BeforeTestRunConfigureAndRunApp()
        {
            await hostManager!.StartAsync();
        }

        [AfterTestRun(Order = 0)]
        public static async Task AfterTestRunTestTeardownConfigurationAndApp()
        {
            if (hostManager != null)
            {
                await hostManager.StopAsync();
            }
        }

        private static IConfiguration BuildConfiguration()
        {
            return new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
        }
    }
}