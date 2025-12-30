using Microsoft.Playwright;
using System.Threading.Tasks;

namespace NorthStandard.Testing.ScreenPlayFramework.Infrastructure.Validation
{
    public abstract class PlaywrightValidatorBase
    {
        protected async Task ShouldBeVisibleAndEnabled(ILocator locator)
        {
            await Assertions.Expect(locator).ToBeVisibleAsync();
      await Assertions.Expect(locator).ToBeEnabledAsync();
    }

    protected async Task ShouldHaveText(ILocator locator, string expected)
    {
      await Assertions.Expect(locator).ToHaveTextAsync(expected);
    }
    protected async Task ShouldContainText(ILocator locator, string expected)
    {
      await Assertions.Expect(locator).ToContainTextAsync(expected);
    }

    protected async Task ShouldBeEmptyInput(ILocator locator)
    {
      await Assertions.Expect(locator).ToHaveValueAsync(string.Empty);
    }
  }
}
