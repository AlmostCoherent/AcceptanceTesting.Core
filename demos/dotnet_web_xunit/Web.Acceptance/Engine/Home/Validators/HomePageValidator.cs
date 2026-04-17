using AlmostCoherent.Testing.Playwright.Domain.Abstractions;
using AlmostCoherent.Testing.ScreenPlayFramework.Domain.Abstractions;

namespace AlmostCoherent.Testing.Demos.Web.Acceptance.Engine.Home.Validators
{
	public class HomePageValidator(IPlaywrightPageProvider pageProvider) : IValidator
    {
        public async Task ValidateHomePageTitle()
        {
			var page = pageProvider.GetPage();
			var title = page.Locator("h1");
			var text = await title.InnerTextAsync();
			text.Equals("Hello");
		}
	}
}
