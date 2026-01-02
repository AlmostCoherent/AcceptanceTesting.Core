using System.Collections.Generic;

namespace NorthStandard.Testing.Playwright.XUnit.Extensions;

/// <summary>
/// Extensions for scenario context
/// </summary>
public static class ScenarioContextExtensions
{
    public static void Set<T>(this Dictionary<string, object> context, string key, T value) where T : notnull
    {
        context[key] = value;
    }

    public static T Get<T>(this Dictionary<string, object> context, string key)
    {
        return (T)context[key];
    }
}
