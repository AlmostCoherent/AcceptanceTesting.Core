using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Xunit.Abstractions;

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

    private class StepInfo
    {
        public StepType Type { get; set; }
        public string Description { get; set; } = string.Empty;
        public Func<Dictionary<string, object>, Task> Action { get; set; } = null!;
    }

    private readonly Dictionary<string, object> _context = new();
    private readonly List<StepInfo> _steps = new();
    private readonly ITestOutputHelper? _output;
    private readonly string _scenarioDescription;
    private StepType _lastStepType = StepType.None;
    private StepType _currentPhase = StepType.None;

    private Scenario(string description, ITestOutputHelper? output)
    {
        _scenarioDescription = description;
        _output = output;
    }

    /// <summary>
    /// Creates a new BDD scenario with the specified description
    /// </summary>
    /// <param name="description">A description of the scenario being tested</param>
    /// <param name="output">Optional test output helper for logging (xUnit)</param>
    /// <returns>A new Scenario instance</returns>
    public static Scenario Create(string description, ITestOutputHelper? output = null) 
        => new(description, output);

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
        _steps.Add(new StepInfo
        {
            Type = StepType.Given,
            Description = description,
            Action = action
        });
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
        _steps.Add(new StepInfo
        {
            Type = StepType.Given,
            Description = description,
            Action = ctx => { action(ctx); return Task.CompletedTask; }
        });
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
        _steps.Add(new StepInfo
        {
            Type = StepType.When,
            Description = description,
            Action = action
        });
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
        _steps.Add(new StepInfo
        {
            Type = StepType.When,
            Description = description,
            Action = ctx => { action(ctx); return Task.CompletedTask; }
        });
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
        _steps.Add(new StepInfo
        {
            Type = StepType.Then,
            Description = description,
            Action = action
        });
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
        _steps.Add(new StepInfo
        {
            Type = StepType.Then,
            Description = description,
            Action = ctx => { action(ctx); return Task.CompletedTask; }
        });
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
        _steps.Add(new StepInfo
        {
            Type = _lastStepType,
            Description = description,
            Action = action
        });
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
        _steps.Add(new StepInfo
        {
            Type = _lastStepType,
            Description = description,
            Action = ctx => { action(ctx); return Task.CompletedTask; }
        });
        // Keep the same _lastStepType so subsequent And calls continue the same type
        return this;
    }

    /// <summary>
    /// Executes all the steps in the scenario sequentially and clears the context when complete
    /// </summary>
    /// <returns>A task representing the asynchronous operation</returns>
    public async Task RunAsync()
    {
        try
        {
            _output?.WriteLine($"\nScenario: {_scenarioDescription}");
            _output?.WriteLine(new string('-', 50));

            for (int i = 0; i < _steps.Count; i++)
            {
                var step = _steps[i];
                var stepPrefix = GetStepPrefix(step.Type, i);
                var stepDescription = $"{stepPrefix} {step.Description}";

                try
                {
                    _output?.WriteLine($"\n{stepDescription}");
                    
                    var sw = Stopwatch.StartNew();
                    await step.Action(_context);
                    sw.Stop();
                    
                    _output?.WriteLine($"  Completed in {sw.ElapsedMilliseconds}ms");
                }
                catch (Exception ex)
                {
                    _output?.WriteLine($"  Failed");
                    throw new InvalidOperationException(
                        $"Scenario '{_scenarioDescription}' failed at step: {stepDescription}", 
                        ex);
                }
            }

            _output?.WriteLine($"\n{new string('-', 50)}");
            _output?.WriteLine($"Scenario completed successfully with {_steps.Count} step(s)\n");
        }
        finally
        {
            // Always cleanup, even if steps fail
            _context.Clear();
            _steps.Clear();
            _lastStepType = StepType.None;
            _currentPhase = StepType.None;
        }
    }

    private string GetStepPrefix(StepType stepType, int stepIndex)
    {
        // Use "And" for subsequent steps of the same type
        if (stepIndex > 0 && _steps[stepIndex - 1].Type == stepType)
        {
            return "  And";
        }

        return stepType switch
        {
            StepType.Given => "Given",
            StepType.When => " When",
            StepType.Then => " Then",
            _ => string.Empty
        };
    }
}
