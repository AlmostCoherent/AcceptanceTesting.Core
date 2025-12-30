using NorthStandard.Testing.ScreenPlayFramework.Domain.Abstractions;
using NorthStandard.Testing.Playwright.Infrastructure.Providers;
using NorthStandard.Testing.Playwright.Application.Services;
using Microsoft.Playwright;
using System;
using System.Threading.Tasks;

namespace NorthStandard.Testing.ScreenPlayFramework.Infrastructure.Web.Actors
{
    public class NavigationActor(PlaywrightPageProvider pageProvider, UrlBuilder urlBuilder) : IActor
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
