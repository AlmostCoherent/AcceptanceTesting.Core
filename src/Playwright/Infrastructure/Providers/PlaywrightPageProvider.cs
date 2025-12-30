using Microsoft.Playwright;
using NorthStandard.Testing.Playwright.Domain.Abstractions;
using System;
using System.Threading.Tasks;

namespace NorthStandard.Testing.Playwright.Infrastructure.Providers
{
    /// <summary>
    /// Provides a playwright page <see cref="IPage"/>
    /// </summary>
    public class PlaywrightPageProvider : IPlaywrightPageProvider
    {
        private readonly IPlaywrightBrowserProvider browserProvider;
        private IBrowserContext? context;
        private IPage? page;

        /// <summary>
        /// Constructs the <see cref="PlaywrightPageProvider"/>
        /// </summary>
        /// <param name="browserProvider"></param>
        public PlaywrightPageProvider(IPlaywrightBrowserProvider browserProvider)
        {
            this.browserProvider = browserProvider;
        }

        /// <summary>
        /// A unique id for this page provider. Useful for debugging issues where DI seems to be creating multiple instances of the page provider
        /// </summary>
        public Guid Id { get; } = Guid.NewGuid();

        /// <summary>
        /// Opens a page in a new browser
        /// </summary>
        /// <param name="contextOptions">The optional context options that should be used to create the new browser page</param>
        public async Task OpenPageInNewBrowserAsync(BrowserNewContextOptions? contextOptions = null)
        {
            await browserProvider.OpenBrowserAsync();
            if (contextOptions is null)
            {
                page = await browserProvider.Provide().NewPageAsync();
            }
            else
            {
                context = await browserProvider.Provide().NewContextAsync(contextOptions);
                page = await context.NewPageAsync();
            }
            if (browserProvider.BrowserTypeLaunchOptions.Timeout.HasValue)
            {
                page.SetDefaultTimeout(browserProvider.BrowserTypeLaunchOptions.Timeout.Value);
            }
            
            // Cast to concrete implementation to call internal method
            if (browserProvider is PlaywrightBrowserProvider concreteBrowserProvider)
            {
                concreteBrowserProvider.SetPageProvider(this);
            }
        }

        /// <summary>
        /// Allows the tests to set the page to use. This is useful when the page is created by a previous test batch already and
        /// we want the next test batch to use the same page. When tests are run with NCrunch it will use the same process for many 
        /// batches of tests, and if creating the browser and page is slow, we can save time by reusing the page. Ideally we would
        /// be able to remember this page across test runs, but something about the solidtoken dependency injection plugin for specflow
        /// causes the before test run to be given new instances of the PlaywrightPageProvider. When this happens we can use the page created 
        /// for the process so that it gets reused across test batches.
        /// </summary>
        /// <param name="page">The pre-created, already open page that we want to use</param>
        public void UsePage(IPage page)
        {
            this.page = page;
        }

        /// <summary>
        /// Provide the playwright page <see cref="IPage"/>
        /// </summary>
        /// <returns>
        /// The playwright page <see cref="IPage"/> :
        /// </returns>
        public IPage GetPage()
        {
            return page ?? throw new InvalidOperationException($"{nameof(OpenPageInNewBrowserAsync)} needs to be called before {nameof(GetPage)}");
        }

        /// <summary>
        /// Closes the page and any associated context. this should be called before closing the browser
        /// </summary>
        /// <returns></returns>
        public async Task ClosePageAsync()
        {
            if (context is not null)
            {
                await context.CloseAsync();
            }
            if (page is not null)
            {
                await page.CloseAsync();
            }
        }
    }
}
