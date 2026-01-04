using System.Threading.Tasks;
using NorthStandard.Testing.Playwright.XUnit.BDD;
using Xunit;
using Xunit.Abstractions;

namespace NorthStandard.Testing.Playwright.XUnit.Tests.BDD;

public class ScenarioOutputExampleTests
{
    private readonly ITestOutputHelper _output;

    public ScenarioOutputExampleTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public async Task DemonstrateScenarioOutputAndTiming()
    {
        // Arrange & Act & Assert
        await Scenario.Create("User login flow with detailed output", _output)
            .Given("a user with valid credentials", ctx =>
            {
                ctx["username"] = "testuser";
                ctx["password"] = "password123";
            })
            .And("the user is on the login page", ctx =>
            {
                ctx["currentPage"] = "login";
            })
            .When("the user enters their credentials", async ctx =>
            {
                // Simulate some work
                await Task.Delay(50);
                ctx["credentialsEntered"] = true;
            })
            .And("clicks the login button", async ctx =>
            {
                await Task.Delay(30);
                ctx["loginClicked"] = true;
            })
            .Then("the user should be logged in", ctx =>
            {
                ctx["isLoggedIn"] = true;
            })
            .And("redirected to the dashboard", ctx =>
            {
                ctx["currentPage"] = "dashboard";
            })
            .And("see their username displayed", ctx =>
            {
                var username = ctx["username"] as string ?? string.Empty;
                ctx["displayedUsername"] = username;
            })
            .RunAsync();
    }

    [Fact]
    public async Task DemonstrateErrorHandling()
    {
        try
        {
            await Scenario.Create("Scenario with error in middle step", _output)
                .Given("initial setup", ctx => ctx["value"] = 1)
                .When("first action", ctx => ctx["value"] = 2)
                .And("action that fails", ctx => throw new System.Exception("Simulated failure"))
                .Then("should not reach here", ctx => ctx["value"] = 3)
                .RunAsync();
        }
        catch (System.InvalidOperationException ex)
        {
            // The error message should include the step description
            Assert.Contains("action that fails", ex.Message);
            Assert.Contains("Scenario with error in middle step", ex.Message);
        }
    }
}
