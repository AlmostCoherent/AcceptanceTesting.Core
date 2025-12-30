using Microsoft.Playwright;
using NorthStandard.Testing.Playwright.Domain.Abstractions;
using NorthStandard.Testing.Playwright.Infrastructure.Configuration;
using Reqnroll;
using System;
using System.IO;
using System.Threading.Tasks;

namespace NorthStandard.Testing.Playwright.Infrastructure.Hooks
{
    [Binding]
    public class PlaywrightHook
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly IPlaywrightPageProvider _pageProvider;
        private readonly PlaywrightConfiguration _configuration;

        public PlaywrightHook(
            ScenarioContext scenarioContext, 
            IPlaywrightPageProvider pageProvider,
            PlaywrightConfiguration? configuration)
        {
            _scenarioContext = scenarioContext;
            _pageProvider = pageProvider;
            _configuration = configuration ?? new PlaywrightConfiguration();
        }

        [BeforeScenario]
        public async Task BeforeScenarioStartTracing()
        {
            // Skip if profile is disabled or tracing is disabled
            if (!_configuration.IsEnabled || !_configuration.EnableTracing)
                return;

            var page = _pageProvider.GetPage();
            var context = page?.Context;
            
            if (context != null)
            {
                await context.Tracing.StartAsync(new()
                {
                    Screenshots = _configuration.TracingOptions.Screenshots,
                    Snapshots = _configuration.TracingOptions.Snapshots,
                    Sources = _configuration.TracingOptions.Sources
                });
            }
        }

        [AfterScenario(Order = 0)]
        public async Task AfterScenarioHandleTestResults()
        {
            // Skip if profile is disabled
            if (!_configuration.IsEnabled)
                return;

            var page = _pageProvider.GetPage();
            var context = page?.Context;

            if (page != null && context != null)
            {
                if (_scenarioContext.TestError != null)
                {
                    await CaptureFailureArtifacts(page, context);
                }
                else if (_configuration.EnableTracing)
                {
                    // Only stop tracing if it was started
                    await context.Tracing.StopAsync();
                }
            }
        }

        private async Task CaptureFailureArtifacts(IPage page, IBrowserContext context)
        {
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var scenarioName = SanitizeFileName(_scenarioContext.ScenarioInfo.Title);

            // Screenshot (if enabled in profile)
            if (_configuration.CaptureScreenshots)
            {
                await CaptureScreenshot(page, scenarioName, timestamp);
            }

            // Trace (if tracing was enabled)
            if (_configuration.EnableTracing && context != null)
            {
                await CaptureTrace(context, scenarioName, timestamp);
            }
        }

        private async Task CaptureScreenshot(IPage page, string scenarioName, string timestamp)
        {
            var screenshotsDir = Path.Combine(_configuration.ArtifactsPath, "Screenshots");
            Directory.CreateDirectory(screenshotsDir);
            var screenshotPath = Path.Combine(screenshotsDir, $"{scenarioName}_{timestamp}.png");
            
            await page.ScreenshotAsync(new() 
            { 
                Path = screenshotPath, 
                FullPage = _configuration.FullPageScreenshots 
            });
            
            Console.WriteLine($"Screenshot saved: {screenshotPath}");
        }

        private async Task CaptureTrace(IBrowserContext context, string scenarioName, string timestamp)
        {
            var tracesDir = Path.Combine(_configuration.ArtifactsPath, "Traces");
            Directory.CreateDirectory(tracesDir);
            var tracePath = Path.Combine(tracesDir, $"{scenarioName}_{timestamp}.zip");
            
            await context.Tracing.StopAsync(new() { Path = tracePath });
            Console.WriteLine($"Trace saved: {tracePath}");
        }

        private static string SanitizeFileName(string fileName)
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            var sanitized = fileName;
            
            foreach (var invalidChar in invalidChars)
            {
                sanitized = sanitized.Replace(invalidChar, '_');
            }
            
            return sanitized.Replace(" ", "_");
        }
    }
}