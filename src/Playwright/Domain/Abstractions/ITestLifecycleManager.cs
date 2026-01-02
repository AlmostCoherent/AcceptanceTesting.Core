using System.Threading;
using System.Threading.Tasks;

namespace NorthStandard.Testing.Playwright.Domain.Abstractions;

/// <summary>
/// Manages Playwright lifecycle for test execution
/// </summary>
public interface ITestLifecycleManager
{
    /// <summary>
    /// Initializes the browser before test run
    /// </summary>
    Task BeforeTestRunAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Cleans up the browser after test run
    /// </summary>
    Task AfterTestRunAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Creates a new page before each scenario/test
    /// </summary>
    Task BeforeScenarioAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Closes the page after each scenario/test
    /// </summary>
    Task AfterScenarioAsync(CancellationToken cancellationToken = default);
}
