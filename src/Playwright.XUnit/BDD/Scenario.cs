using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NorthStandard.Testing.Playwright.XUnit.BDD;

/// <summary>
/// Fluent API for building BDD-style test scenarios
/// </summary>
public class Scenario
{
    private enum StepType
    {
        None,
        Given,
        When,
        Then
    }

    private readonly Dictionary<string, object> _context = new();
    private readonly List<Func<Dictionary<string, object>, Task>> _steps = new();
    private StepType _lastStepType = StepType.None;
    private StepType _currentPhase = StepType.None;

    private Scenario() { }

    /// <summary>
    /// Creates a new BDD scenario with the specified description
    /// </summary>
    /// <param name="description">A description of the scenario being tested</param>
    /// <returns>A new Scenario instance</returns>
    public static Scenario Create(string description) => new();

    private void ValidateStepOrder(StepType newStepType)
    {
        switch (newStepType)
        {
            case StepType.Given:
                // Given can only be at the start or after another Given
                if (_currentPhase != StepType.None && _currentPhase != StepType.Given)
                {
                    throw new InvalidOperationException("Given must come before When and Then steps");
                }
                break;
            case StepType.When:
                // When can be at the start, or after Given/When
                if (_currentPhase != StepType.None && _currentPhase != StepType.Given && _currentPhase != StepType.When)
                {
                    throw new InvalidOperationException("When cannot follow Then");
                }
                break;
            case StepType.Then:
                // Then must come after When
                if (_currentPhase != StepType.When && _currentPhase != StepType.Then)
                {
                    throw new InvalidOperationException("Then must follow When");
                }
                break;
        }

        if (newStepType != StepType.None)
        {
            _currentPhase = newStepType;
        }
    }

    /// <summary>
    /// Adds a Given step that sets up the initial context for the scenario (async version)
    /// </summary>
    /// <param name="description">A description of what is being set up</param>
    /// <param name="action">The async action to execute for this step</param>
    /// <returns>The scenario instance for fluent chaining</returns>
    public Scenario Given(string description, Func<Dictionary<string, object>, Task> action)
    {
        ValidateStepOrder(StepType.Given);
        _steps.Add(action);
        _lastStepType = StepType.Given;
        return this;
    }

    /// <summary>
    /// Adds a Given step that sets up the initial context for the scenario (sync version)
    /// </summary>
    /// <param name="description">A description of what is being set up</param>
    /// <param name="action">The action to execute for this step</param>
    /// <returns>The scenario instance for fluent chaining</returns>
    public Scenario Given(string description, Action<Dictionary<string, object>> action)
    {
        ValidateStepOrder(StepType.Given);
        _steps.Add(ctx => { action(ctx); return Task.CompletedTask; });
        _lastStepType = StepType.Given;
        return this;
    }

    /// <summary>
    /// Adds a When step that performs an action or triggers an event (async version)
    /// </summary>
    /// <param name="description">A description of the action being performed</param>
    /// <param name="action">The async action to execute for this step</param>
    /// <returns>The scenario instance for fluent chaining</returns>
    public Scenario When(string description, Func<Dictionary<string, object>, Task> action)
    {
        ValidateStepOrder(StepType.When);
        _steps.Add(action);
        _lastStepType = StepType.When;
        return this;
    }

    /// <summary>
    /// Adds a When step that performs an action or triggers an event (sync version)
    /// </summary>
    /// <param name="description">A description of the action being performed</param>
    /// <param name="action">The action to execute for this step</param>
    /// <returns>The scenario instance for fluent chaining</returns>
    public Scenario When(string description, Action<Dictionary<string, object>> action)
    {
        ValidateStepOrder(StepType.When);
        _steps.Add(ctx => { action(ctx); return Task.CompletedTask; });
        _lastStepType = StepType.When;
        return this;
    }

    /// <summary>
    /// Adds a Then step that verifies the expected outcome or assertion (async version)
    /// </summary>
    /// <param name="description">A description of the expected outcome</param>
    /// <param name="action">The async action to execute for this step</param>
    /// <returns>The scenario instance for fluent chaining</returns>
    public Scenario Then(string description, Func<Dictionary<string, object>, Task> action)
    {
        ValidateStepOrder(StepType.Then);
        _steps.Add(action);
        _lastStepType = StepType.Then;
        return this;
    }

    /// <summary>
    /// Adds a Then step that verifies the expected outcome or assertion (sync version)
    /// </summary>
    /// <param name="description">A description of the expected outcome</param>
    /// <param name="action">The action to execute for this step</param>
    /// <returns>The scenario instance for fluent chaining</returns>
    public Scenario Then(string description, Action<Dictionary<string, object>> action)
    {
        ValidateStepOrder(StepType.Then);
        _steps.Add(ctx => { action(ctx); return Task.CompletedTask; });
        _lastStepType = StepType.Then;
        return this;
    }

    /// <summary>
    /// Adds an And step that continues the type of the previous step (Given/When/Then) (async version)
    /// </summary>
    /// <param name="description">A description of the additional step</param>
    /// <param name="action">The async action to execute for this step</param>
    /// <returns>The scenario instance for fluent chaining</returns>
    /// <exception cref="InvalidOperationException">Thrown when And is called without a preceding Given, When, or Then step</exception>
    public Scenario And(string description, Func<Dictionary<string, object>, Task> action)
    {
        if (_lastStepType == StepType.None)
        {
            throw new InvalidOperationException("And must follow a Given, When, or Then step");
        }
        _steps.Add(action);
        // Keep the same _lastStepType so subsequent And calls continue the same type
        return this;
    }

    /// <summary>
    /// Adds an And step that continues the type of the previous step (Given/When/Then) (sync version)
    /// </summary>
    /// <param name="description">A description of the additional step</param>
    /// <param name="action">The action to execute for this step</param>
    /// <returns>The scenario instance for fluent chaining</returns>
    /// <exception cref="InvalidOperationException">Thrown when And is called without a preceding Given, When, or Then step</exception>
    public Scenario And(string description, Action<Dictionary<string, object>> action)
    {
        if (_lastStepType == StepType.None)
        {
            throw new InvalidOperationException("And must follow a Given, When, or Then step");
        }
        _steps.Add(ctx => { action(ctx); return Task.CompletedTask; });
        // Keep the same _lastStepType so subsequent And calls continue the same type
        return this;
    }

    /// <summary>
    /// Executes all the steps in the scenario sequentially and clears the context when complete
    /// </summary>
    /// <returns>A task representing the asynchronous operation</returns>
    public async Task RunAsync()
    {
        foreach (var step in _steps)
        {
            await step(_context);
        }
        _context.Clear();
        _steps.Clear();
        _lastStepType = StepType.None;
        _currentPhase = StepType.None;
    }
}
