using AlmostCoherent.Testing.Playwright.Application.Services;
using AlmostCoherent.Testing.Playwright.Domain.Abstractions;
using AlmostCoherent.Testing.ScreenPlayFramework.Domain.Abstractions;

namespace AlmostCoherent.Testing.Demos.Web.Acceptance.Engine.Home.Actors
{
    public class HomePageActor(IPlaywrightPageProvider pageProvider, UrlBuilder urlBuilder) : IActor
	{
		public async Task NavigateToHomePage() {
			var page = pageProvider.GetPage();
			await page.GotoAsync(urlBuilder.GetBaseUrl());
		}
	}
}
