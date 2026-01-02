using NorthStandard.Testing.Playwright.Domain.Abstractions;
using Reqnroll;
using System;
using System.Threading.Tasks;

namespace NorthStandard.Testing.Playwright.Reqnroll.Hooks;

/// <summary>
/// Reqnroll hooks for Playwright lifecycle management
/// </summary>
[Binding]
public class PlaywrightReqnrollHook
{
    private static ITestLifecycleManager? _lifecycleManager;
    private readonly ITestLifecycleManager _instanceLifecycleManager;

    public PlaywrightReqnrollHook(ITestLifecycleManager lifecycleManager)
    {
        _instanceLifecycleManager = lifecycleManager ?? throw new ArgumentNullException(nameof(lifecycleManager));
    }

    /// <summary>
    /// Initializes static lifecycle manager for test run hooks
    /// </summary>
    [BeforeTestRun(Order = 100)]
    public static void InitializeLifecycleManager(ITestLifecycleManager lifecycleManager)
    {
        _lifecycleManager = lifecycleManager;
    }

    /// <summary>
    /// Initializes browser before test run
    /// </summary>
    [BeforeTestRun(Order = 200)]
    public static async Task BeforeTestRun()
    {
        if (_lifecycleManager != null)
        {
            await _lifecycleManager.BeforeTestRunAsync();
        }
    }

    /// <summary>
    /// Cleans up browser after test run
    /// </summary>
    [AfterTestRun(Order = 100)]
    public static async Task AfterTestRun()
    {
        if (_lifecycleManager != null)
        {
            await _lifecycleManager.AfterTestRunAsync();
        }
    }

    /// <summary>
    /// Creates new page before each scenario
    /// </summary>
    [BeforeScenario(Order = 100)]
    public async Task BeforeScenario()
    {
        await _instanceLifecycleManager.BeforeScenarioAsync();
    }

    /// <summary>
    /// Closes page after each scenario
    /// </summary>
    [AfterScenario(Order = 100)]
    public async Task AfterScenario()
    {
        await _instanceLifecycleManager.AfterScenarioAsync();
    }
}
