using Microsoft.Playwright;
using AlmostCoherent.Testing.Playwright.Domain.Entities;

namespace AlmostCoherent.Testing.Playwright.Extensions
{
  public static class LocatorExtensions
  {
    public static Button AsButton(this ILocator locator)
    {
      return new Button(locator);
    }
  }
}
