using NorthStandard.Testing.Playwright.Domain.Abstractions;
using System;
using System.Threading.Tasks;
using Xunit;

namespace NorthStandard.Testing.Playwright.XUnit.Fixtures;

/// <summary>
/// xUnit collection fixture for managing Playwright browser lifecycle across test collection.
/// Use this to share a single browser instance across multiple test classes.
/// </summary>
public class PlaywrightBrowserFixture : IAsyncLifetime
{
    private readonly ITestLifecycleManager _lifecycleManager;

    public PlaywrightBrowserFixture(ITestLifecycleManager lifecycleManager)
    {
        _lifecycleManager = lifecycleManager ?? throw new ArgumentNullException(nameof(lifecycleManager));
    }

    /// <summary>
    /// Initializes the browser before the test collection runs
    /// </summary>
    public async Task InitializeAsync()
    {
        await _lifecycleManager.BeforeTestRunAsync();
    }

    /// <summary>
    /// Cleans up the browser after the test collection completes
    /// </summary>
    public async Task DisposeAsync()
    {
        await _lifecycleManager.AfterTestRunAsync();
    }
}
