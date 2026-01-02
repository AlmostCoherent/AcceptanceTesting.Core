using NorthStandard.Testing.Playwright.Domain.Abstractions;
using System;
using System.Threading.Tasks;
using Xunit;

namespace NorthStandard.Testing.Playwright.XUnit.Fixtures;

/// <summary>
/// xUnit class fixture for managing Playwright page lifecycle for each test.
/// Use this to create a fresh page for each test method.
/// </summary>
public class PlaywrightPageFixture : IAsyncLifetime
{
    private readonly ITestLifecycleManager _lifecycleManager;

    public PlaywrightPageFixture(ITestLifecycleManager lifecycleManager)
    {
        _lifecycleManager = lifecycleManager ?? throw new ArgumentNullException(nameof(lifecycleManager));
    }

    /// <summary>
    /// Creates a new page before each test
    /// </summary>
    public async Task InitializeAsync()
    {
        await _lifecycleManager.BeforeScenarioAsync();
    }

    /// <summary>
    /// Closes the page after each test
    /// </summary>
    public async Task DisposeAsync()
    {
        await _lifecycleManager.AfterScenarioAsync();
    }
}
