using NorthStandard.Testing.Playwright.Application.Services;
using NorthStandard.Testing.Playwright.Domain.Abstractions;
using NorthStandard.Testing.ScreenPlayFramework.Domain.Abstractions;

namespace NorthStandard.Testing.Demos.Web.Acceptance.Engine.Home.Actors
{
    public class HomePageActor(IPlaywrightPageProvider pageProvider, UrlBuilder urlBuilder) : IActor
	{
		public async Task NavigateToHomePage() {
			await pageProvider.OpenPageInNewBrowserAsync();
			var page = pageProvider.GetPage();
			await page.GotoAsync(urlBuilder.GetBaseUrl());
		}
	}
}
