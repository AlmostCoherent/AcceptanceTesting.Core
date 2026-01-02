# NorthStandard.Testing.Playwright.XUnit

xUnit integration package for NorthStandard.Testing.Playwright. This package provides fixtures and lifecycle management for running Playwright tests with xUnit.

## Installation

```bash
dotnet add package NorthStandard.Testing.Playwright.XUnit
```

## Usage

### Basic Setup with Class Fixture

Use `PlaywrightPageFixture` to create a fresh page for each test class:

```csharp
using NorthStandard.Testing.Playwright.Domain.Abstractions;
using NorthStandard.Testing.Playwright.XUnit.Fixtures;
using Xunit;

public class MyTests : IClassFixture<PlaywrightPageFixture>
{
    private readonly IPlaywrightPageProvider _pageProvider;

    public MyTests(PlaywrightPageFixture fixture)
    {
        _pageProvider = fixture.PageProvider;
    }

    [Fact]
    public async Task Should_Navigate_To_Page()
    {
        var page = _pageProvider.GetPage();
        await page.GotoAsync("https://example.com");
        
        var title = await page.TitleAsync();
        Assert.Equal("Example Domain", title);
    }
}
```

### Collection Fixture for Shared Browser

Use `PlaywrightBrowserFixture` to share a browser across multiple test classes:

```csharp
using NorthStandard.Testing.Playwright.XUnit.Fixtures;
using Xunit;

// Define collection
[CollectionDefinition("Browser")]
public class BrowserCollection : ICollectionFixture<PlaywrightBrowserFixture>
{
}

// Use collection in test class
[Collection("Browser")]
public class FirstTests
{
    private readonly IPlaywrightPageProvider _pageProvider;

    public FirstTests(IPlaywrightPageProvider pageProvider)
    {
        _pageProvider = pageProvider;
    }

    [Fact]
    public async Task Test1()
    {
        var page = _pageProvider.GetPage();
        await page.GotoAsync("https://example.com");
        // Test logic...
    }
}

[Collection("Browser")]
public class SecondTests
{
    private readonly IPlaywrightPageProvider _pageProvider;

    public SecondTests(IPlaywrightPageProvider pageProvider)
    {
        _pageProvider = pageProvider;
    }

    [Fact]
    public async Task Test2()
    {
        var page = _pageProvider.GetPage();
        await page.GotoAsync("https://example.com");
        // Test logic...
    }
}
```

### Complete Isolation with PlaywrightFixture

Use `PlaywrightFixture` for complete isolation with browser and page per test class:

```csharp
using NorthStandard.Testing.Playwright.XUnit.Fixtures;
using Xunit;

public class IsolatedTests : IClassFixture<PlaywrightFixture>
{
    private readonly PlaywrightFixture _fixture;

    public IsolatedTests(PlaywrightFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Should_Be_Isolated()
    {
        var page = _fixture.PageProvider.GetPage();
        await page.GotoAsync("https://example.com");
        // Each test class gets its own browser and page
    }
}
```

### Configuration

Configure Playwright in your test project setup or constructor:

```csharp
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NorthStandard.Testing.Playwright.XUnit.Extensions;

public class TestStartup
{
    public void ConfigureServices(IServiceCollection services)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        services.AddPlaywrightForXUnit(configuration);
    }
}
```

Add configuration to your `appsettings.json`:

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

## Available Fixtures

| Fixture | Lifecycle | Use Case |
|---------|-----------|----------|
| `PlaywrightBrowserFixture` | Browser per collection | Share browser across test classes |
| `PlaywrightPageFixture` | Page per test class | Fresh page for each test class |
| `PlaywrightFixture` | Browser + Page per test class | Complete isolation per test class |

## Dependencies

- NorthStandard.Testing.Playwright (core package)
- xUnit 2.9.0+
- .NET 8.0+

## Related Packages

- **NorthStandard.Testing.Playwright**: Core Playwright testing utilities (framework-agnostic)
- **NorthStandard.Testing.Playwright.Reqnroll**: Reqnroll/BDD integration
