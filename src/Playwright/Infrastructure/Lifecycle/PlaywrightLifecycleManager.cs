using NorthStandard.Testing.Playwright.Domain.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NorthStandard.Testing.Playwright.Infrastructure.Lifecycle;

/// <summary>
/// Default implementation of Playwright lifecycle management
/// </summary>
public class PlaywrightLifecycleManager : ITestLifecycleManager
{
    private readonly IPlaywrightBrowserProvider _browserProvider;
    private readonly IPlaywrightPageProvider _pageProvider;

    public PlaywrightLifecycleManager(
        IPlaywrightBrowserProvider browserProvider,
        IPlaywrightPageProvider pageProvider)
    {
        _browserProvider = browserProvider ?? throw new ArgumentNullException(nameof(browserProvider));
        _pageProvider = pageProvider ?? throw new ArgumentNullException(nameof(pageProvider));
    }

    public async Task BeforeTestRunAsync(CancellationToken cancellationToken = default)
    {
        await _browserProvider.OpenBrowserAsync();
    }

    public async Task AfterTestRunAsync(CancellationToken cancellationToken = default)
    {
        await _browserProvider.CloseBrowserAsync();
    }

    public async Task BeforeScenarioAsync(CancellationToken cancellationToken = default)
    {
        await _pageProvider.OpenPageInNewBrowserAsync();
    }

    public async Task AfterScenarioAsync(CancellationToken cancellationToken = default)
    {
        await _pageProvider.ClosePageAsync();
    }
}
