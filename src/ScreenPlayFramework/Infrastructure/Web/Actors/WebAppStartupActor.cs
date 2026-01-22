using NorthStandard.Testing.Playwright.Infrastructure.Providers;
using System.Threading.Tasks;
using NorthStandard.Testing.Playwright.Domain.Abstractions;
using NorthStandard.Testing.ScreenPlayFramework.Domain.Abstractions;

namespace NorthStandard.Testing.ScreenPlayFramework.Infrastructure.Web.Actors
{
    public class WebAppStartupActor : IActor
    {
        private readonly IPlaywrightPageProvider pageProvider;

        public WebAppStartupActor(IPlaywrightPageProvider pageProvider)
        {
            this.pageProvider = pageProvider;
        }

        public async Task StartWebApp()
        {
            await pageProvider.OpenPageInNewBrowserAsync();
            pageProvider.UsePage(pageProvider.GetPage());
        }

    }
}
