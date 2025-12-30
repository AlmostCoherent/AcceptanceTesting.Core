using Microsoft.Playwright;
using System.Threading.Tasks;

namespace NorthStandard.Testing.Playwright.Domain.Entities
{
  public sealed class Button(ILocator locator)
  {
    private readonly ILocator _locator = locator;

    /// <summary>
    /// Clicks the button and waits until network activity has settled.
    /// </summary>
    public async Task ClickAndWaitForNetworkIdleAsync(IPage page)
    {
      var waitForNetworkIdle = page.WaitForLoadStateAsync(LoadState.NetworkIdle);

      await _locator.ClickAsync();

      await waitForNetworkIdle;
    }

    /// <summary>
    /// Clicks the button with no expected side-effects.
    /// </summary>
    public Task ClickAsync() => _locator.ClickAsync();
  }
}
