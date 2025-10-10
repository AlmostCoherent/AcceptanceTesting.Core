using System.Threading.Tasks;
using Testing.Acceptance.Core.Abstractions;
using Testing.Acceptance.Core.Infrastructure.Playwright;

namespace Testing.Acceptance.Core.Engine.Core.Actors
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
