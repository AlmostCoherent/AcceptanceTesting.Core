using AlmostCoherent.Testing.ScreenPlayFramework.Domain.Abstractions;
using AlmostCoherent.Testing.Playwright.Infrastructure.Providers;
using AlmostCoherent.Testing.Playwright.Application.Services;
using Microsoft.Playwright;
using System;
using System.Threading.Tasks;

namespace AlmostCoherent.Testing.ScreenPlayFramework.Infrastructure.Web.Actors
{
    public class NavigationActor(IPlaywrightPageProvider pageProvider, UrlBuilder urlBuilder) : IActor
    {
    public async Task NavigateToUrl(string url)
    {
      var page = pageProvider.GetPage();

      await page.GotoAsync(
          urlBuilder.GetUrl(url),
          new PageGotoOptions
          {
            WaitUntil = WaitUntilState.DOMContentLoaded
          });
    }
  }
}
