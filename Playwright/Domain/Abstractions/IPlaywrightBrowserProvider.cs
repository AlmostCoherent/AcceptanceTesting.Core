using Microsoft.Playwright;
using System.Threading.Tasks;

namespace NorthStandard.Testing.Playwright.Domain.Abstractions
{
    /// <summary>
    /// Abstraction for providing Playwright browsers
    /// </summary>
    public interface IPlaywrightBrowserProvider
    {
        /// <summary>
        /// The launch options the browser was opened with
        /// </summary>
        BrowserTypeLaunchOptions BrowserTypeLaunchOptions { get; }

        /// <summary>
        /// Opens the browser
        /// </summary>
        Task OpenBrowserAsync();

        /// <summary>
        /// Closes the browser
        /// </summary>
        Task CloseBrowserAsync();

        /// <summary>
        /// Provides access to the browser
        /// </summary>
        /// <returns>The browser</returns>
        IBrowser Provide();
    }
}