using AcceptanceTesting.Core.Abstractions;
using AcceptanceTesting.Core.Infrastructure.Playwright;
using System.Threading.Tasks;

namespace AcceptanceTesting.Core.Engine.Core.Actors
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
            browser.UsePage(browser.Provide());
        }

    }
}
