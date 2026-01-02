# NorthStandard.Testing.Playwright.Reqnroll

Reqnroll integration package for NorthStandard.Testing.Playwright. This package provides hooks and lifecycle management for running Playwright tests with Reqnroll (BDD framework).

## Installation

```bash
dotnet add package NorthStandard.Testing.Playwright.Reqnroll
```

## Usage

### 1. Configure Services

In your Reqnroll test project, register Playwright services in your DI container:

```csharp
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NorthStandard.Testing.Playwright.Reqnroll.Extensions;

[Binding]
public class Hooks
{
    [BeforeTestRun]
    public static void BeforeTestRun(IServiceCollection services, IConfiguration configuration)
    {
        services.AddPlaywrightForReqnroll(configuration);
    }
}
```

### 2. Configure Playwright Settings

Add Playwright configuration to your `appsettings.json`:

```json
{
  "Playwright": {
    "BrowserType": "Chromium",
    "Headless": false,
    "SlowMo": 0,
    "Timeout": 30000
  }
}
```

### 3. Use in Step Definitions

```csharp
using NorthStandard.Testing.Playwright.Domain.Abstractions;

[Binding]
public class StepDefinitions
{
    private readonly IPlaywrightPageProvider _pageProvider;

    public StepDefinitions(IPlaywrightPageProvider pageProvider)
    {
        _pageProvider = pageProvider;
    }

    [Given(@"I navigate to ""(.*)""")]
    public async Task GivenINavigateTo(string url)
    {
        var page = _pageProvider.GetPage();
        await page.GotoAsync(url);
    }
}
```

## How It Works

This package automatically manages the Playwright lifecycle:

- **BeforeTestRun**: Initializes the browser
- **AfterTestRun**: Closes and cleans up the browser
- **BeforeScenario**: Creates a new page for each scenario
- **AfterScenario**: Closes the page after each scenario

The hooks are automatically discovered by Reqnroll and executed at the appropriate times.

## Dependencies

- NorthStandard.Testing.Playwright (core package)
- Reqnroll 3.3.0+
- .NET 8.0+

## Related Packages

- **NorthStandard.Testing.Playwright**: Core Playwright testing utilities (framework-agnostic)
