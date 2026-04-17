# Acceptance Testing Framework - Copilot Instructions

> **For use across external projects integrating the AcceptanceTesting.Core framework**

These instructions provide detailed guidance for creating acceptance tests using the ScreenPlay pattern with Playwright. Instructions differ significantly based on whether you're using **xUnit** or **Reqnroll** (BDD).

---

## Core Framework Overview

This framework is built on three core layers:

1. **ScreenPlayFramework** - Pattern-based test architecture with Actors, Tasks, and Questions
2. **Playwright Integration** - Web and API automation via `Playwright.XUnit` or `Playwright.Reqnroll`
3. **AcceptanceTesting.Core** - Shared abstractions, contexts, and utilities

---

## Table of Contents
- [Common Patterns (All Tests)](#common-patterns-all-tests)
- [xUnit-Specific Guidance](#xunit-specific-guidance)
- [Reqnroll-Specific Guidance](#reqnroll-specific-guidance)
- [Architecture Decisions](#architecture-decisions)

---

## Common Patterns (All Tests)

### Project Structure

All acceptance test projects follow this organization:

```
[ProjectName].Acceptance/
??? Engine/                          # Test logic layer
?   ??? [Feature]/
?   ?   ??? Actors/
?   ?   ?   ??? [Feature]Actor.cs    # User actions and workflows
?   ?   ??? Validators/              # For xUnit only - state verification
?   ?   ?   ??? [Feature]Validator.cs
?   ?   ??? Questions/               # For Reqnroll only - state queries
?   ?   ?   ??? [Feature]Question.cs
?   ?   ??? Contexts/                # Feature-specific state (if needed)
?   ?       ??? [Feature]Context.cs
?   ??? Core/
?       ??? ApiStartupActor.cs       # Initialize API client (if applicable)
?       ??? WebAppStartupActor.cs    # Initialize web app (if applicable)
??? Specs/
?   ??? Tests/                       # xUnit test classes only
?   ?   ??? [Feature]Tests.cs
?   ??? Features/                    # Reqnroll feature files only
?   ?   ??? [Feature].feature
?   ??? Steps/                       # Reqnroll step definitions only
?       ??? [Feature]StepDefinitions.cs
??? appsettings.json                 # Test configuration
??? [ProjectName]Fixture.cs          # Collection fixture (xUnit) or Bootstrap hooks (Reqnroll)
```

### The ScreenPlay Pattern

All tests use the **ScreenPlay Pattern**, which models testing as interactions between Actors and the application:

#### Core Concepts

- **Actor**: Represents a user or API client; encapsulates actions and workflows
- **Validator** (xUnit): Asserts expected application state and throws on failure
- **Question** (Reqnroll): Queries application state, leaving assertion to the caller
- **Context**: Holds scenario-specific state shared between actors and validators/questions

#### Example Structure

```csharp
// Actor: Encapsulates user actions and workflows
public class HomePageActor : IActor
{
    private readonly IPlaywrightPageProvider _pageProvider;
    private readonly UrlBuilder _urlBuilder;
    
    public async Task NavigateToHomePage()
    {
        var page = _pageProvider.GetPage();
        await page.GotoAsync(_urlBuilder.GetBaseUrl());
    }
    
    public async Task ClickButton(string buttonSelector)
    {
        var page = _pageProvider.GetPage();
        await page.ClickAsync(buttonSelector);
    }
}

// Validator (xUnit): Performs assertions and state verification
public class HomePageValidator : IValidator
{
    private readonly IPlaywrightPageProvider _pageProvider;
    
    public async Task ValidatePageTitle(string expectedTitle)
    {
        var page = _pageProvider.GetPage();
        var heading = page.Locator("h1");
        var text = await heading.InnerTextAsync();
        Assert.Equal(expectedTitle, text);
    }

    public async Task<bool> IsElementVisible(string selector)
    {
        var page = _pageProvider.GetPage();
        return await page.Locator(selector).IsVisibleAsync();
    }
}

// Question (Reqnroll): Queries state without asserting
public class HomePageQuestion
{
    private readonly IPlaywrightPageProvider _pageProvider;
    
    public async Task<string> GetPageTitle()
    {
        var page = _pageProvider.GetPage();
        var heading = page.Locator("h1");
        return await heading.InnerTextAsync();
    }
}
```

### Service Registration

All test projects must register their Actors, Validators, and Questions with dependency injection:

```csharp
// Extension method - follow this pattern
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHomePageActors(this IServiceCollection services)
    {
        services.AddScoped<HomePageActor>();
        services.AddScoped<HomePageValidator>();
        // For Reqnroll, also register Questions
        services.AddScoped<HomePageQuestion>();
        
        return services;
    }
}

// In your fixture/bootstrap
services.AddPlaywrightServices(config);
services.AddCoreScreenPlayFramework();
services.AddHomePageActors();
```

### Playwright Page & Browser Lifecycle

The framework abstracts Playwright management through:

- **IPlaywrightPageProvider**: Get the current test page
- **IPlaywrightBrowserProvider**: Get the current browser (rarely used directly)
- **ITestLifecycleManager**: Handles browser/page setup and teardown

Inject `IPlaywrightPageProvider` into your actors and validators to access the current page:

```csharp
public class MyActor(IPlaywrightPageProvider pageProvider)
{
    public async Task DoSomething()
    {
        var page = pageProvider.GetPage();
        await page.ClickAsync("button");
    }
}
```

### Configuration via appsettings.json

```json
{
  "Playwright": {
    "BrowserType": "chromium",
    "Headless": true,
    "SlowMo": 0,
    "Timeout": 30000
  },
  "TestServer": {
    "Port": 0,
    "EnvironmentName": "Testing"
  }
}
```

### Assertion Best Practices

- **Use Playwright's built-in waiting**: Let Playwright's locator poll for elements automatically
- **Avoid sleeps**: Never use `Thread.Sleep()` or `Task.Delay()` in tests
- **Scope assertions**: Check visibility and enabled state in addition to content
- **Be idempotent**: Tests should not depend on execution order or previous state
- **Use xUnit Asserts**: `Assert.Equal()`, `Assert.True()`, `Assert.NotNull()`, etc.

---

## xUnit-Specific Guidance

### ?? CAVEAT: xUnit is for **traditional test scenarios**, not BDD feature files

**Use xUnit if:**
- You need fine-grained test control with `[Fact]` and `[Theory]`
- Your tests don't map to business-readable BDD features
- You want Traits for test categorization and filtering
- Test per class isolation with fixtures (not shared across classes)

**Use Reqnroll if:**
- You need BDD feature files with Given/When/Then
- Business stakeholders need to read test scenarios
- You want gherkin language support

### Test Class Setup

xUnit test classes use constructor injection with IAsyncLifetime-based fixtures:

```csharp
[Collection("WebServer")]  // References WebServerCollection
public class HomeTests
{
    private readonly WebServerFixture _fixture;
    private readonly HomePageActor _actor;
    private readonly HomePageValidator _validator;

    public HomeTests(WebServerFixture fixture)
    {
        _fixture = fixture;
        _actor = _fixture.Services.GetService(typeof(HomePageActor)) as HomePageActor 
            ?? throw new InvalidOperationException("HomePageActor not registered");
        _validator = _fixture.Services.GetService(typeof(HomePageValidator)) as HomePageValidator
            ?? throw new InvalidOperationException("HomePageValidator not registered");
    }

    [Fact]
    [Trait("Category", "Home")]
    [Trait("Priority", "High")]
    public async Task User_Can_View_Home_Page()
    {
        await _actor.NavigateToHomePage();
        await _validator.ValidatePageTitle("Welcome");
    }
}
```

### Collection Fixtures (Shared Web Server)

Use a **Collection Fixture** to share a single server instance across all tests in the collection:

```csharp
public class WebServerFixture : IAsyncLifetime
{
    public PlaywrightFixture PlaywrightFixture { get; private set; } = null!;
    public IServiceProvider Services { get; private set; } = null!;
    public UrlBuilder UrlBuilder { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        // 1. Build configuration
        var config = BuildConfiguration();
        
        // 2. Start web server
        var hostManager = new WebTestingHostManager(config, args => 
            global::Web.Program.CreateWebHostBuilder(args, isTestHost: true));
        hostManager.Initialize();
        UrlBuilder = new UrlBuilder(hostManager.BaseUrl);
        
        // 3. Build DI container
        Services = BuildServiceProvider(config);
        
        // 4. Initialize Playwright
        var lifecycleManager = Services.GetRequiredService<ITestLifecycleManager>();
        var pageProvider = Services.GetRequiredService<IPlaywrightPageProvider>();
        PlaywrightFixture = new PlaywrightFixture(lifecycleManager, pageProvider);
        await PlaywrightFixture.InitializeAsync();
        
        // 5. Start server
        await hostManager.StartAsync();
    }

    public async Task DisposeAsync()
    {
        // Cleanup order: reverse of initialization
        await PlaywrightFixture?.DisposeAsync();
        await hostManager?.StopAsync();
        if (Services is IDisposable d) d.Dispose();
    }
}

[CollectionDefinition("WebServer")]
public class WebServerCollection : ICollectionFixture<WebServerFixture>
{
}
```

### Scenario Helper (Optional BDD-Style in xUnit)

For a more BDD feel while using xUnit, use the `Scenario` helper from Playwright.XUnit:

```csharp
[Fact]
public async Task User_Can_View_Home_Page()
{
    await Scenario.Create("User views the home page")
        .When("I navigate to the home page", async ctx =>
        {
            await _actor.NavigateToHomePage();
        })
        .Then("I should see the home page", async ctx =>
        {
            await _validator.ValidatePageTitle("Welcome");
        })
        .RunAsync();
}
```

**Note**: This is **optional**. You can use traditional Arrange/Act/Assert in xUnit as well:

```csharp
[Fact]
public async Task User_Can_View_Home_Page()
{
    // Arrange
    var actor = _fixture.Services.GetService(typeof(HomePageActor)) as HomePageActor!;
    var validator = _fixture.Services.GetService(typeof(HomePageValidator)) as HomePageValidator!;
    
    // Act
    await actor.NavigateToHomePage();
    
    // Assert
    await validator.ValidatePageTitle("Welcome");
}
```

### Validators in xUnit

In **xUnit**, use **Validators** (not Questions) for assertions:

```csharp
public class HomePageValidator : IValidator
{
    private readonly IPlaywrightPageProvider _pageProvider;

    public async Task ValidatePageTitle(string expectedTitle)
    {
        var page = _pageProvider.GetPage();
        var heading = page.Locator("h1");
        var text = await heading.InnerTextAsync();
        Assert.Equal(expectedTitle, text);
    }

    public async Task<bool> IsContentVisible(string selector)
    {
        var page = _pageProvider.GetPage();
        return await page.Locator(selector).IsVisibleAsync();
    }
}
```

Call validators from tests:
```csharp
await _validator.ValidatePageTitle("Welcome");
Assert.True(await _validator.IsContentVisible(".content"));
```

### NuGet Dependencies (xUnit)

```xml
<ItemGroup>
    <PackageReference Include="Playwright.XUnit" Version="..." />
    <PackageReference Include="ScreenPlayFramework" Version="..." />
    <PackageReference Include="AcceptanceTesting.Core" Version="..." />
    <PackageReference Include="xunit" Version="..." />
    <PackageReference Include="xunit.runner.visualstudio" Version="..." />
</ItemGroup>
```

### Test Execution (xUnit)

```bash
# Run all tests
dotnet test

# Run by trait
dotnet test --filter "Category=Home"

# Run specific test
dotnet test --filter "Name=User_Can_View_Home_Page"

# Run in parallel
dotnet test --parallel
```

---

## Reqnroll-Specific Guidance

### ?? CAVEAT: Reqnroll is for **BDD-driven scenarios**, not traditional unit tests

**Use Reqnroll if:**
- You're practicing BDD and have Gherkin feature files
- Non-technical stakeholders need to review test scenarios
- You want to map scenarios to business requirements
- Each scenario is independent (unlike xUnit where fixtures can be shared across a class)

**Use xUnit if:**
- You need traditional test structure with fine-grained control
- You don't have BDD scenarios
- You want per-class isolation with shared fixtures

### Feature Files (Gherkin)

Write human-readable scenarios in `.feature` files:

```gherkin
Feature: Weather Forecast API
  As a client
  I want to retrieve weather forecasts
  So that I can plan my activities

  Scenario: Get all weather forecasts
    Given I have defined some weather forecasts
    When I make a request to the weatherforecast api
    Then I should see a 200 OK response
    And the response contains valid forecast data
```

**Structure:**
- **Feature**: High-level description
- **Scenario**: Individual test case
- **Given**: Setup/preconditions
- **When**: Action/trigger
- **Then**: Assertion/verification

### Step Definitions

Map Gherkin steps to C# code using Actors and Questions:

```csharp
[Binding]
public class WeatherForecastStepDefinitions
{
    private readonly WeatherForecastsActor _actor;
    private readonly WeatherForecastQuestion _question;

    public WeatherForecastStepDefinitions(WeatherForecastsActor actor, WeatherForecastQuestion question)
    {
        _actor = actor;
        _question = question;
    }

    [Given("I have defined some weather forecasts")]
    public async Task GivenWeatherForecastsExist()
    {
        await _actor.SetupWeatherForecasts();
    }

    [When("I make a request to the weatherforecast api")]
    public async Task WhenMakingRequest()
    {
        await _actor.RequestWeatherForecasts();
    }

    [Then("I should see a {int} OK response")]
    public async Task ThenResponseIsOk(int statusCode)
    {
        var actualStatus = await _question.GetResponseStatus();
        Assert.Equal(statusCode, actualStatus);
    }

    [Then("the response contains valid forecast data")]
    public async Task ThenResponseContainsData()
    {
        var data = await _question.GetForecastData();
        Assert.NotEmpty(data);
    }
}
```

**Key points:**
- **Binding**: Class attribute to register with Reqnroll  
- **Step attributes**: Given/When/Then with optional parameter placeholders `{int}`, `{string}`, etc.
- **Dependency Injection**: Reqnroll injects actors and questions via constructor

### Bootstrap & Service Registration (Reqnroll)

Reqnroll uses **Hook** methods (not fixtures) to initialize services:

```csharp
[Binding]
public class Bootstrap
{
    private static IHost? _server;
    private static int _port;

    [BeforeTestRun(Order = 0)]
    public static void BeforeTestRunSetupServices()
    {
        // 1. Get a free port dynamically
        _port = GetFreeTcpPort();

        // 2. Build configuration
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .Build();

        // 3. Register services in the DI container
        // Reqnroll will inject these into step definitions and hooks
        var services = new ServiceCollection()
            .AddLogging()
            .AddSingleton(config)
            .AddSingleton(new UrlBuilder($"https://localhost:{_port}/"))
            .AddPlaywrightForReqnroll(config)
            .AddCoreScreenPlayFramework()
            .AddScreenPlayFrameworkFromAssembly(typeof(Bootstrap).Assembly);
    }

    [BeforeTestRun(Order = 1)]
    public static async Task BeforeTestRunStartServer(UrlBuilder urlBuilder)
    {
        // 4. Start the application server
        Console.WriteLine($"Starting server on {urlBuilder.GetBaseUrl()}...");
        _server = Program.CreateApiHostBuilder(new[] { $"--urls={urlBuilder.GetBaseUrl()}" });
        await _server.StartAsync();
    }

    [AfterTestRun]
    public static async Task AfterTestRunStopServer()
    {
        // 5. Stop the application server
        if (_server != null)
        {
            await _server.StopAsync();
        }
    }

    private static int GetFreeTcpPort()
    {
        using var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        int port = ((IPEndPoint)listener.LocalEndpoint).Port;
        return port;
    }
}
```

**Key differences from xUnit:**
- Use `[BeforeTestRun]` / `[AfterTestRun]` hooks instead of collection fixtures
- Services are registered once and shared across scenarios
- Each scenario gets a fresh page (via `PlaywrightReqnrollHook`)

### Playwright Lifecycle in Reqnroll

The `PlaywrightReqnrollHook` automatically manages browser and page. Register it in your hooks:

```csharp
[Binding]
public class PlaywrightHooks
{
    private static ITestLifecycleManager? _lifecycleManager;

    [BeforeTestRun(Order = 100)]
    public static void InitializePlaywright(ITestLifecycleManager lifecycleManager)
    {
        _lifecycleManager = lifecycleManager;
    }

    [BeforeTestRun(Order = 200)]
    public static async Task BeforeTestRunInitBrowser()
    {
        if (_lifecycleManager != null)
        {
            await _lifecycleManager.BeforeTestRunAsync();
        }
    }

    [BeforeScenario]
    public async Task BeforeScenarioCreatePage(ITestLifecycleManager lifecycleManager)
    {
        await lifecycleManager.BeforeScenarioAsync();
    }

    [AfterScenario]
    public async Task AfterScenarioClosePage(ITestLifecycleManager lifecycleManager)
    {
        await lifecycleManager.AfterScenarioAsync();
    }

    [AfterTestRun]
    public static async Task AfterTestRunDisposeBrowser()
    {
        if (_lifecycleManager != null)
        {
            await _lifecycleManager.AfterTestRunAsync();
        }
    }
}
```

### Questions in Reqnroll

In **Reqnroll**, use **Questions** to query state (don't assert directly in questions):

```csharp
public class WeatherForecastQuestion
{
    private readonly ApiContext _context;

    public WeatherForecastQuestion(ApiContext context)
    {
        _context = context;
    }

    public async Task<int> GetResponseStatus()
    {
        return _context.LastResponse?.StatusCode ?? 0;
    }

    public async Task<List<WeatherForecast>> GetForecastData()
    {
        return _context.LastForecastData ?? new();
    }
}
```

Then **assert in step definitions**:

```csharp
[Then("I should see a {int} response")]
public async Task ThenResponseStatus(int expectedStatus)
{
    var actualStatus = await _question.GetResponseStatus();
    Assert.Equal(expectedStatus, actualStatus);
}
```

### Context Sharing (Reqnroll)

Use custom context objects to share state between steps:

```csharp
public class ApiContext
{
    public HttpResponseMessage? LastResponse { get; set; }
    public List<WeatherForecast> LastForecastData { get; set; } = new();
}

[Binding]
public class WeatherForecastStepDefinitions
{
    private readonly WeatherForecastsActor _actor;
    private readonly ApiContext _context;

    public WeatherForecastStepDefinitions(WeatherForecastsActor actor, ApiContext context)
    {
        _actor = actor;
        _context = context;
    }

    [When("I make a request")]
    public async Task WhenMakingRequest()
    {
        _context.LastResponse = await _actor.MakeRequest();
    }

    [Then("I should see forecast data")]
    public void ThenVerifyData()
    {
        Assert.NotNull(_context.LastResponse);
    }
}
```

Reqnroll automatically manages the lifetime of scoped services (like `ApiContext`) per scenario.

### NuGet Dependencies (Reqnroll)

```xml
<ItemGroup>
    <PackageReference Include="Reqnroll" Version="..." />
    <PackageReference Include="Reqnroll.NUnit" Version="..." /> <!-- or MSTest -->
    <PackageReference Include="Playwright.Reqnroll" Version="..." />
    <PackageReference Include="ScreenPlayFramework" Version="..." />
    <PackageReference Include="AcceptanceTesting.Core" Version="..." />
</ItemGroup>
```

### Test Execution (Reqnroll)

```bash
# Run all scenarios
dotnet test

# Run specific feature
dotnet test -- --features="./Specs/Features/WeatherForecast.feature"

# Run with tags
dotnet test -- --tags="@smoke"

# Generate report
dotnet test -- --format=cucumber
```

### reqnroll.json Configuration

```json
{
  "language": "en",
  "unitTestProvider": "nunit",
  "runtime": {
    "stopAtFirstError": false,
    "missingOrPendingStepsOutcome": "Inconclusive"
  },
  "trace": {
    "traceSuccessfulSteps": true,
    "traceTimings": true
  }
}
```

---

## Architecture Decisions

### Actor Responsibilities

**Actors should encapsulate:**
- Page navigation and interactions
- Form filling and button clicks
- API requests and data operations
- Any action that changes application state

**Examples:**
```csharp
public class LoginActor : IActor
{
    public async Task NavigateToLoginPage() { }
    public async Task EnterEmail(string email) { }
    public async Task EnterPassword(string password) { }
    public async Task ClickLoginButton() { }
}
```

### When to Create a Validator (xUnit)

Create a validator when:
- You need to verify application state
- Multiple tests check similar outcomes
- Assertions are complex or multi-step
- You want to centralize assertion logic

```csharp
public class LoginPageValidator : IValidator
{
    public async Task ValidateLoginSuccess()
    {
        // Complex assertion logic here
        Assert.NotNull(loggedInUser);
    }
}
```

### When to Create a Question (Reqnroll)

Create a question when:
- You need to query application state
- The value is used across multiple steps
- You want to separate queries from assertions

```csharp
public class LoginQuestion
{
    public async Task<User?> GetLoggedInUser() { }
    public async Task<bool> IsLoginFormVisible() { }
}
```

### When to Create a Context

Create a context when:
- You need to share data between steps/actors
- Testing a multi-step workflow (login ? browse ? purchase)
- Different actors need access to the same state

```csharp
public class ShoppingContext
{
    public List<Product> CartItems { get; set; } = new();
    public decimal TotalPrice { get; set; }
    public Order? PlacedOrder { get; set; }
}
```

### Page Objects vs. Inline Selectors

**Use inline selectors** for simple, one-off interactions:
```csharp
await page.FillAsync("input[name='email']", "user@test.com");
```

**Use Page Objects** when:
- Multiple tests use the same page
- Selectors are complex or brittle
- You want to centralize UI locators

```csharp
public class LoginPage
{
    private readonly IPage _page;
    
    public ILocator EmailInput => _page.Locator("input[name='email']");
    public ILocator PasswordInput => _page.Locator("input[name='password']");
    public ILocator SubmitButton => _page.Locator("button[type='submit']");
}

public class LoginActor : IActor
{
    private readonly LoginPage _page;
    
    public async Task EnterCredentials(string email, string password)
    {
        await _page.EmailInput.FillAsync(email);
        await _page.PasswordInput.FillAsync(password);
        await _page.SubmitButton.ClickAsync();
    }
}
```

---

## Quick Checklist for New Test Projects

- [ ] Create `[ProjectName].Acceptance` project with .NET 8 target
- [ ] Add NuGet refs: `Playwright.XUnit` OR `Playwright.Reqnroll`, `ScreenPlayFramework`, `AcceptanceTesting.Core`
- [ ] Create folder structure: `Engine/`, `Specs/`
- [ ] Create test fixture (xUnit) or bootstrap hooks (Reqnroll)
- [ ] Create first Actor for a feature
- [ ] Register actor in DI (via `ServiceCollectionExtensions` or bootstrap)
- [ ] Create first Validator (xUnit) or Question (Reqnroll)
- [ ] Write first test (xUnit) or scenario (Reqnroll)
- [ ] Run tests: `dotnet test`
- [ ] Verify async/await usage throughout
- [ ] Verify Playwright waits are used (no sleeps)

---

## References

- **ScreenPlay Pattern**: [Documented in framework](src/ScreenPlayFramework)
- **xUnit Example**: [demos/dotnet_web_xunit/Web.Acceptance](demos/dotnet_web_xunit/Web.Acceptance)
- **Reqnroll Example**: [demos/dotnet_api/Api.Acceptance](demos/dotnet_api/Api.Acceptance)
- **Playwright Docs**: https://playwright.dev/dotnet/
- **Reqnroll Docs**: https://reqnroll.net/
