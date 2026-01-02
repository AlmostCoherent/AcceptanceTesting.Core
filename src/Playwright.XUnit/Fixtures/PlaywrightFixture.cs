using NorthStandard.Testing.Playwright.Domain.Abstractions;
using System;
using System.Threading.Tasks;
using Xunit;

namespace NorthStandard.Testing.Playwright.XUnit.Fixtures;

/// <summary>
/// xUnit class fixture combining browser and page lifecycle management.
/// Use this for complete test isolation with a fresh browser and page for each test class.
/// </summary>
public class PlaywrightFixture : IAsyncLifetime
{
    private readonly ITestLifecycleManager _lifecycleManager;

    public IPlaywrightPageProvider PageProvider { get; }

    public PlaywrightFixture(
        ITestLifecycleManager lifecycleManager,
        IPlaywrightPageProvider pageProvider)
    {
        _lifecycleManager = lifecycleManager ?? throw new ArgumentNullException(nameof(lifecycleManager));
        PageProvider = pageProvider ?? throw new ArgumentNullException(nameof(pageProvider));
    }

    /// <summary>
    /// Initializes browser and creates a page before the test class runs
    /// </summary>
    public async Task InitializeAsync()
    {
        await _lifecycleManager.BeforeTestRunAsync();
        await _lifecycleManager.BeforeScenarioAsync();
    }

    /// <summary>
    /// Cleans up page and browser after the test class completes
    /// </summary>
    public async Task DisposeAsync()
    {
        await _lifecycleManager.AfterScenarioAsync();
        await _lifecycleManager.AfterTestRunAsync();
    }
}
