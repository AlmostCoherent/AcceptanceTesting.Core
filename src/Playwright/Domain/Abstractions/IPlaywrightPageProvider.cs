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
        /// Allows the tests to set the page to use. This is useful when the page is created by a previous test batch already and
        /// we want the next test batch to use the same page. When tests are run with NCrunch it will use the same process for many 
        /// batches of tests, and if creating the browser and page is slow, we can save time by reusing the page. Ideally we would
        /// be able to remember this page across test runs, but something about the solidtoken dependency injection plugin for specflow
        /// causes the before test run to be given new instances of the PlaywrightPageProvider. When this happens we can use the page created 
        /// for the process so that it gets reused across test batches.
        /// </summary>
        /// <param name="page">The pre-created, already open page that we want to use</param>
        void UsePage(IPage page);

        /// <summary>
        /// Closes the page and disposes of the browser
        /// </summary>
        Task ClosePageAsync();
    }
}