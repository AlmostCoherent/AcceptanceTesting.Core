using NorthStandard.Testing.ScreenPlayFramework.Domain.Abstractions;
using NorthStandard.Testing.Playwright.Infrastructure.Providers;
using System.Threading.Tasks;

namespace NorthStandard.Testing.ScreenPlayFramework.Infrastructure.Web.Actors
{
    public class WebAppStartupActor : IActor
    {
        private readonly PlaywrightPageProvider browser;

        public WebAppStartupActor(PlaywrightPageProvider browser)
        {
            this.browser = browser;
        }

        public async Task StartWebApp()
        {
            await browser.OpenPageInNewBrowserAsync();
            browser.UsePage(browser.GetPage());
        }

    }
}
