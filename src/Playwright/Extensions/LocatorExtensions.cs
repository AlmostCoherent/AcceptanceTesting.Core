using Microsoft.Playwright;
using NorthStandard.Testing.Playwright.Domain.Entities;

namespace NorthStandard.Testing.Playwright.Extensions
{
  public static class LocatorExtensions
  {
    public static Button AsButton(this ILocator locator)
    {
      return new Button(locator);
    }
  }
}
