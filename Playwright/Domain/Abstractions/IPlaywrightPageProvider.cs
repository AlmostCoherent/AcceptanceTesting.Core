using Microsoft.Playwright;
using System;
using System.Threading.Tasks;

namespace NorthStandard.Testing.Playwright.Domain.Abstractions
{
    /// <summary>
    /// Abstraction for providing Playwright pages
    /// </summary>
    public interface IPlaywrightPageProvider
    {
        /// <summary>
        /// A unique id for this page provider. Useful for debugging issues where DI seems to be creating multiple instances of the page provider
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Opens a page in a new browser
        /// </summary>
        /// <param name="contextOptions">The optional context options that should be used to create the new browser page</param>
        Task OpenPageInNewBrowserAsync(BrowserNewContextOptions? contextOptions = null);

        /// <summary>
        /// Returns the page that has been created by the page provider
        /// </summary>
        /// <returns>The page</returns>
        IPage GetPage();

        /// <summary>
        /// Closes the page and disposes of the browser
        /// </summary>
        Task ClosePageAsync();
    }
}