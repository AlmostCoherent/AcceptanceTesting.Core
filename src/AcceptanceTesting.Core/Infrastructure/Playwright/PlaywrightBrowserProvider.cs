using Microsoft.Playwright;
using System;
using System.Threading.Tasks;

namespace AcceptanceTesting.Core.Infrastructure.Playwright
{
    /// <summary>
    /// Opens, closes and provides the playwright browser
    /// </summary>
    public class PlaywrightBrowserProvider
    {
        private IBrowser? browser;
        private PlaywrightPageProvider? pageProvider;

        /// <summary>
        /// The launch options the browser was opened with
        /// </summary>
        public BrowserTypeLaunchOptions BrowserTypeLaunchOptions { get; }

        /// <summary>
        /// Constructs <see cref="PlaywrightBrowserProvider"/>
        /// </summary>
        /// <param name="browserTypeLaunchOptions"></param>
        public PlaywrightBrowserProvider(BrowserTypeLaunchOptions browserTypeLaunchOptions)
        {
            BrowserTypeLaunchOptions = browserTypeLaunchOptions;
        }

        /// <summary>
        /// Opens the browser
        /// </summary>
        public async Task OpenBrowserAsync()
        {
            if (browser != null && browser.IsConnected)
            {
                return;
            }

            var playwright = await Microsoft.Playwright.Playwright.CreateAsync();
            browser = await playwright.Chromium.LaunchAsync(BrowserTypeLaunchOptions);
        }

        /// <summary>
        /// Provides the currently open playwright browser <see cref="IBrowser"/>
        /// </summary>
        /// <returns>The <see cref="IBrowser"/> currently open</returns>
        public IBrowser Provide()
        {
            return browser ?? throw new InvalidOperationException($"{nameof(OpenBrowserAsync)} needs to be called before {nameof(Provide)}");
        }

        /// <summary>
        /// Closes the browser
        /// </summary>
        /// <returns></returns>
        public async Task CloseBrowserAsync()
        {
            if (pageProvider is not null)
            {
                await pageProvider.ClosePage();
            }
            if (browser is not null)
            {
                await browser.CloseAsync();
            }
        }

        /// <summary>
        /// Sets the reference to the <see cref="PlaywrightPageProvider"/> so that it can be closed when the browser is closed.
        /// This ensures any additional contexts created by the <see cref="PlaywrightPageProvider"/> are closed before the browser is closed, as per the documentation.
        /// </summary>
        /// <param name="playwrightPageProvider"></param>
        internal void SetPageProvider(PlaywrightPageProvider playwrightPageProvider)
        {
            pageProvider = playwrightPageProvider;
        }
    }
}
