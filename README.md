# AcceptanceTesting.Core

[![Publish NuGet Packages](https://github.com/AlmostCoherent/AcceptanceTesting.Core/actions/workflows/nuget-publish.yml/badge.svg)](https://github.com/AlmostCoherent/AcceptanceTesting.Core/actions/workflows/nuget-publish.yml)
[![GitVersion](https://img.shields.io/badge/GitVersion-Enabled-blue)](docs/GitVersion.md)

A comprehensive .NET testing framework based on the **ScreenPlay pattern** that provides a clean, maintainable approach to acceptance testing. Built with **Playwright** integration and **Reqnroll** support for Behavior-Driven Development (BDD).

## 🎯 Overview

This framework implements the **ScreenPlay pattern** - a modern approach to test automation that emphasizes:
- **Readability**: Tests read like natural language
- **Maintainability**: Reusable components reduce duplication
- **Scalability**: Clean architecture supports large test suites
- **Flexibility**: Supports both web UI and API testing

### Key Components

The framework is built around three core concepts:

- **🎭 Actors**: Perform individual actions (click, type, navigate)
- **🎼 Orchestrators**: Combine actors into complex workflows (login, checkout)
- **✅ Validators**: Assert expected outcomes and behaviors

## 📦 Packages

This repository provides multiple NuGet packages:

| Package | Description | Use Case |
|---------|-------------|----------|
| `AlmostCoherent.AcceptanceTesting.Core` | Core abstractions and patterns | Base framework for any testing project |
| `NorthStandard.Testing.ScreenPlayFramework` | ScreenPlay pattern implementation | Structured test automation |
| `NorthStandard.Testing.Playwright` | Playwright utilities and extensions | Web UI testing |
| `NorthStandard.Testing.Playwright.Reqnroll` | Reqnroll integration for Playwright | BDD with Gherkin syntax |
| `NorthStandard.Testing.Playwright.XUnit` | xUnit integration with BDD support | xUnit test framework with fluent scenarios |
| `NorthStandard.Testing.Hosting` | Application hosting helpers | Test environment management |

## 🚀 Quick Start

### Installation

```bash
# Install the core framework
dotnet add package AlmostCoherent.AcceptanceTesting.Core

# Add ScreenPlay pattern support
dotnet add package NorthStandard.Testing.ScreenPlayFramework

# For web testing with Playwright
dotnet add package NorthStandard.Testing.Playwright

# For BDD with Reqnroll
dotnet add package NorthStandard.Testing.Playwright.Reqnroll

# For xUnit with fluent BDD scenarios
dotnet add package NorthStandard.Testing.Playwright.XUnit

# For application hosting in tests
dotnet add package NorthStandard.Testing.Hosting
```

### From GitHub Packages

```bash
# Add GitHub Packages source
dotnet nuget add source --username USERNAME --password GITHUB_PAT \
  --name github "https://nuget.pkg.github.com/AlmostCoherent/index.json"

# Install packages
dotnet add package AlmostCoherent.AcceptanceTesting.Core --source github
```

> **Security Note**: For production environments, use [Azure Artifacts Credential Provider](https://github.com/microsoft/artifacts-credprovider) for secure credential management.

### Basic Example

#### ScreenPlay Pattern

```csharp
// Actor - performs individual actions
public class LoginActor : IActor
{
    public async Task EnterCredentials(string username, string password)
    {
        await _page.Locator("#username").FillAsync(username);
        await _page.Locator("#password").FillAsync(password);
    }
    
    public async Task ClickLoginButton()
    {
        await _page.Locator("#login-btn").AsButton().ClickAsync();
    }
}

// Orchestrator - combines actors into workflows  
public class AuthenticationOrchestrator : IOrchestrator
{
    public async Task LoginUser(string username, string password)
    {
        await _loginActor.EnterCredentials(username, password);
        await _loginActor.ClickLoginButton();
    }
}

// Validator - asserts outcomes
public class DashboardValidator : IValidator  
{
    public async Task ShouldShowWelcomeMessage(string expectedUser)
    {
        var welcomeText = await _page.Locator(".welcome").TextContentAsync();
        welcomeText.Should().Contain($"Welcome, {expectedUser}");
    }
}
```

#### xUnit Fluent BDD Scenarios

```csharp
[Fact]
public async Task UserCanLoginSuccessfully()
{
    await Scenario.Create("User login flow", _output)
        .Given("a user with valid credentials", ctx =>
        {
            ctx["username"] = "testuser";
            ctx["password"] = "password123";
        })
        .When("the user logs in", async ctx =>
        {
            await _loginPage.EnterCredentials(
                ctx["username"] as string,
                ctx["password"] as string);
            await _loginPage.ClickLogin();
        })
        .Then("the user should see the dashboard", async ctx =>
        {
            await Expect(_page.Locator(".dashboard"))
                .ToBeVisibleAsync();
        })
        .RunAsync();
}
```

**Output:**
```
Scenario: User login flow
--------------------------------------------------

Given a user with valid credentials
  ✓ Completed in 0ms

 When the user logs in
  ✓ Completed in 245ms

 Then the user should see the dashboard
  ✓ Completed in 89ms

--------------------------------------------------
Scenario completed successfully with 3 step(s)
```

## 🎭 ScreenPlay Framework Deep Dive

The ScreenPlay pattern provides a structured approach to test automation that scales from simple unit tests to complex end-to-end scenarios.

### Architecture Principles

#### 1. **Actors** 🎭
**What they do**: Atomic, reusable actions that interact directly with the application

**Examples**:
- `NavigationActor`: Handle page navigation
- `FormActor`: Fill out and submit forms  
- `ApiActor`: Make HTTP requests
- `DatabaseActor`: Perform data operations

**Benefits**:
- ✅ Single responsibility principle
- ✅ High reusability across tests
- ✅ Easy to unit test in isolation
- ✅ Clear separation of concerns

```csharp
public class NavigationActor : IActor
{
    public async Task GoToHomePage() => await _page.GotoAsync("/");
    public async Task GoToProductPage(int id) => await _page.GotoAsync($"/products/{id}");
    public async Task GoBack() => await _page.GoBackAsync();
}
```

#### 2. **Orchestrators** 🎼
**What they do**: Combine multiple actors to create business workflows

**Examples**:
- `CheckoutOrchestrator`: Complete purchase process
- `RegistrationOrchestrator`: Sign up new users
- `OrderManagementOrchestrator`: Handle order lifecycle

**Benefits**:
- ✅ Business logic abstraction
- ✅ Readable test scenarios
- ✅ Centralized workflow management
- ✅ Easier maintenance when processes change

```csharp
public class CheckoutOrchestrator : IOrchestrator
{
    public async Task CompleteOrderFlow(OrderDetails order)
    {
        await _cartActor.AddItemsToCart(order.Items);
        await _navigationActor.GoToCheckout();
        await _paymentActor.EnterPaymentDetails(order.Payment);
        await _shippingActor.SelectShippingMethod(order.Shipping);
        await _orderActor.ConfirmOrder();
    }
}
```

#### 3. **Validators** ✅
**What they do**: Assert expected outcomes and verify system state

**Examples**:
- `PaymentValidator`: Verify payment processing
- `EmailValidator`: Check notification delivery
- `DatabaseValidator`: Validate data persistence

**Benefits**:
- ✅ Centralized assertion logic
- ✅ Consistent validation patterns
- ✅ Separation of verification from execution
- ✅ Reusable across multiple test scenarios

```csharp
public class OrderValidator : IValidator
{
    public async Task ShouldConfirmOrderPlacement(string orderNumber)
    {
        await Assertions.Expect(_page.Locator(".confirmation"))
            .ToContainTextAsync($"Order {orderNumber} confirmed");
    }
    
    public async Task ShouldReceiveConfirmationEmail(string email)
    {
        var emails = await _emailService.GetEmailsForAddress(email);
        emails.Should().ContainSingle(e => e.Subject.Contains("Order Confirmation"));
    }
}
```

### Integration Patterns

#### Dependency Injection Setup

```csharp
// Configure services for ScreenPlay pattern
services.AddScopedImplementingFromAssembly<IActor>(Assembly.GetExecutingAssembly());
services.AddScopedImplementingFromAssembly<IOrchestrator>(Assembly.GetExecutingAssembly());
services.AddScopedImplementingFromAssembly<IValidator>(Assembly.GetExecutingAssembly());
```

#### BDD Integration with Reqnroll

```csharp
[Given("I am on the login page")]
public async Task GivenIAmOnTheLoginPage()
{
    await _navigationActor.GoToLoginPage();
}

[When("I log in with valid credentials")]  
public async Task WhenILogInWithValidCredentials()
{
    await _authenticationOrchestrator.LoginUser("testuser", "password123");
}

[Then("I should see the dashboard")]
public async Task ThenIShouldSeeTheDashboard()
{
    await _dashboardValidator.ShouldBeDisplayed();
}
```

#### xUnit BDD Scenarios

The framework provides a fluent BDD API for xUnit tests:

**Features:**
- ✅ Natural Given/When/Then/And syntax
- ✅ Step execution tracking with timing
- ✅ Automatic step validation and ordering
- ✅ Context sharing across steps
- ✅ Detailed output logging
- ✅ Rich error messages with step details

```csharp
[Fact]
public async Task CompleteCheckoutProcess()
{
    await Scenario.Create("Complete order checkout", _output)
        .Given("a user is logged in", ctx => 
            ctx["userId"] = "user123")
        .And("has items in cart", async ctx =>
            await _cart.AddItem("product-1"))
        .When("user proceeds to checkout", async ctx =>
            await _checkout.NavigateToCheckout())
        .And("enters payment details", async ctx =>
            await _checkout.EnterPayment("4111111111111111"))
        .And("confirms the order", async ctx =>
            await _checkout.ConfirmOrder())
        .Then("order should be placed successfully", async ctx =>
            await Expect(_page.Locator(".order-confirmation"))
                .ToBeVisibleAsync())
        .And("confirmation email should be sent", async ctx =>
        {
            var emails = await _emailService.GetEmails(ctx["userId"]);
            emails.Should().ContainSingle(e => e.Subject.Contains("Order"));
        })
        .RunAsync();
}
```

## 📚 Demos

This repository includes comprehensive demo applications showcasing different testing approaches:

### 🌐 Web Application Demo with Reqnroll (`demos/dotnet_web/`)

**What it demonstrates**:
- Complete web application with Blazor Server
- Playwright-based UI testing
- ScreenPlay pattern implementation for web testing
- Reqnroll/BDD with Gherkin syntax
- Integration with authentication flows

**Running the demo**:
```bash
cd demos/dotnet_web/Web
dotnet run
# Navigate to https://localhost:7001

# Run acceptance tests with Reqnroll
cd ../Web.Acceptance  
dotnet test
```

### 🎯 Web Application Demo with xUnit (`demos/dotnet_web_xunit/`)

**What it demonstrates**:
- Blazor Server web application
- xUnit test framework integration
- Fluent BDD scenarios with Given/When/Then
- ScreenPlay pattern with xUnit
- Test output logging and step timing

**Running the demo**:
```bash
cd demos/dotnet_web_xunit/Web
dotnet run
# Navigate to http://localhost:5000

# Run xUnit BDD tests
cd ../Web.Acceptance
dotnet test
```

### 🔌 API Application Demo (`demos/dotnet_api/`)

**What it demonstrates**:
- RESTful API
- API testing with HttpClient integration
- ScreenPlay pattern for API testing
- Swagger/OpenAPI documentation

**Running the demo**:
```bash
cd demos/dotnet_api/Api
dotnet run
# API available at https://localhost:7002/swagger

# Run API acceptance tests
cd ../Api.Acceptance
dotnet test
```

### 📋 Demo Test Scenarios

Both demos include comprehensive test suites demonstrating:

| Feature | Reqnroll Demo | xUnit Demo | API Demo |
|---------|---------------|------------|----------|
| **ScreenPlay Pattern** | ✅ Full implementation | ✅ Full implementation | ✅ Full implementation |
| **Test Framework** | Reqnroll/Gherkin | xUnit Fluent BDD | MSTest |
| **Step Validation** | Via Gherkin syntax | Built-in ordering | N/A |
| **Output Logging** | Reqnroll reports | Detailed step timing | Standard output |
| **Context Sharing** | ScenarioContext | Dictionary-based | DI container |
| **Error Messages** | Step descriptions | Rich step context | Standard |
| **Host Management** | WebTestingHostManager | WebTestingHostManager | N/A |

## 🏗️ Building from Source

### Prerequisites

- **.NET 8.0 SDK** or later
- **Node.js** (for Playwright browser installation)

### Build Commands

```bash
# Clone the repository
git clone https://github.com/AlmostCoherent/AcceptanceTesting.Core.git
cd AcceptanceTesting.Core

# Restore dependencies
dotnet restore

# Build the solution  
dotnet build

# Run all tests
dotnet test

# Install Playwright browsers (for web testing)
pwsh src/Playwright.Tests/bin/Debug/net8.0/playwright.ps1 install

# Create NuGet packages
dotnet pack --configuration Release --output ./nupkgs
```

### Project Structure

```
AcceptanceTesting.Core/
├── src/                              # Source packages
│   ├── AcceptanceTesting.Core/           # Core abstractions
│   ├── ScreenPlayFramework/              # ScreenPlay implementation  
│   ├── Playwright/                       # Playwright utilities
│   ├── Playwright.Reqnroll/              # Reqnroll/BDD integration
│   ├── Playwright.XUnit/                 # xUnit with fluent BDD
│   ├── Hosting/                          # Test hosting helpers
│   └── *.Tests/                          # Unit tests
├── demos/                            # Example applications
│   ├── dotnet_web/                       # Web demo with Reqnroll
│   ├── dotnet_web_xunit/                 # Web demo with xUnit BDD
│   └── dotnet_api/                       # API application demo  
├── docs/                             # Documentation
└── .github/workflows/                # CI/CD pipeline
```

## 🔄 CI/CD & Versioning

This project uses **GitVersion** for automated semantic versioning:

- **Main branch**: Production releases (`1.0.0`, `1.0.1`)
- **Develop branch**: Alpha builds (`1.1.0-alpha.1`) 
- **Feature branches**: Feature builds (`1.0.0-my-feature.1`)
- **Release branches**: Beta releases (`1.1.0-beta.1`)

See [GitVersion Documentation](docs/GitVersion.md) for details.

### Automated Pipeline

The GitHub Actions workflow automatically:
- ✅ Builds solution and runs tests
- ✅ Calculates semantic version using GitVersion
- ✅ Creates NuGet packages for all `IsPackable=true` projects
- ✅ Publishes to GitHub Packages on main/release branches
- ✅ Provides detailed build artifacts and reports

## 🤝 Contributing

Contributions are welcome! Please:

1. **Fork** the repository
2. **Create** a feature branch (`git checkout -b feature/amazing-feature`)
3. **Follow** the ScreenPlay pattern for any test-related contributions
4. **Add tests** for new functionality
5. **Ensure** all tests pass (`dotnet test`)
6. **Submit** a Pull Request

### Development Guidelines

- Use the **ScreenPlay pattern** for test automation code
- Follow **SOLID principles** in component design
- Write **clear, self-documenting code**
- Include **comprehensive tests** for new features
- Update **documentation** for API changes

## 📄 License

This project is licensed under the **MIT License** - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- **ScreenPlay Pattern**: Inspired by [Serenity BDD](http://serenity-bdd.info)
- **Playwright**: Microsoft's excellent browser automation library
- **Reqnroll**: Modern BDD framework for .NET
- **GitVersion**: Semantic versioning from Git history
---

**Ready to transform your testing approach?** 🚀

Start with our [demos](#📚-demos), explore the [ScreenPlay framework](#🎭-screenplay-framework-deep-dive), and build maintainable test suites that scale with your applications.