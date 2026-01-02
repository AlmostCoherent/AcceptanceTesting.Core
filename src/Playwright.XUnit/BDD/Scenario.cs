using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NorthStandard.Testing.Playwright.XUnit.BDD;

/// <summary>
/// Fluent API for building BDD-style test scenarios
/// </summary>
public class Scenario
{
    private readonly Dictionary<string, object> _context = new();
    private readonly List<Func<Dictionary<string, object>, Task>> _steps = new();

    private Scenario() { }

    public static Scenario Create(string description) => new();

    public Scenario Given(string description, Func<Dictionary<string, object>, Task> action)
    {
        _steps.Add(action);
        return this;
    }

    public Scenario Given(string description, Action<Dictionary<string, object>> action)
    {
        _steps.Add(ctx => { action(ctx); return Task.CompletedTask; });
        return this;
    }

    public Scenario When(string description, Func<Dictionary<string, object>, Task> action)
    {
        _steps.Add(action);
        return this;
    }

    public Scenario When(string description, Action<Dictionary<string, object>> action)
    {
        _steps.Add(ctx => { action(ctx); return Task.CompletedTask; });
        return this;
    }

    public Scenario Then(string description, Func<Dictionary<string, object>, Task> action)
    {
        _steps.Add(action);
        return this;
    }

    public Scenario Then(string description, Action<Dictionary<string, object>> action)
    {
        _steps.Add(ctx => { action(ctx); return Task.CompletedTask; });
        return this;
    }

    public async Task RunAsync()
    {
        foreach (var step in _steps)
        {
            await step(_context);
        }
        _context.Clear();
    }
}
