using AlmostCoherent.Testing.Playwright.Application.Services;
using AlmostCoherent.Testing.Playwright.Domain.Abstractions;
using AlmostCoherent.Testing.ScreenPlayFramework.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlmostCoherent.Testing.Demos.Web.Acceptance.Engine.Home.Actors
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
