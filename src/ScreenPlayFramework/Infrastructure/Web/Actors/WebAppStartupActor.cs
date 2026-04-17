using AlmostCoherent.Testing.ScreenPlayFramework.Domain.Abstractions;
using System.Threading.Tasks;
using AlmostCoherent.Testing.Playwright.Domain.Abstractions;

namespace AlmostCoherent.Testing.ScreenPlayFramework.Infrastructure.Web.Actors
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
