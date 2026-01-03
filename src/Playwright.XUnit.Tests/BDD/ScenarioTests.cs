using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NorthStandard.Testing.Playwright.XUnit.BDD;
using Xunit;

namespace NorthStandard.Testing.Playwright.XUnit.Tests.BDD;

public class ScenarioTests
{
    [Fact]
    public void Create_ReturnsNewScenarioInstance()
    {
        // Act
        var scenario = Scenario.Create("Test scenario");

        // Assert
        scenario.Should().NotBeNull();
    }

    [Fact]
    public async Task Given_AddsStepAndExecutes()
    {
        // Arrange
        var executed = false;
        var scenario = Scenario.Create("Test");

        // Act
        scenario.Given("initial state", ctx => { executed = true; });
        await scenario.RunAsync();

        // Assert
        executed.Should().BeTrue();
    }

    [Fact]
    public async Task Given_AsyncVersion_AddsStepAndExecutes()
    {
        // Arrange
        var executed = false;
        var scenario = Scenario.Create("Test");

        // Act
        scenario.Given("initial state", async ctx => { executed = true; await Task.CompletedTask; });
        await scenario.RunAsync();

        // Assert
        executed.Should().BeTrue();
    }

    [Fact]
    public async Task When_AddsStepAndExecutes()
    {
        // Arrange
        var executed = false;
        var scenario = Scenario.Create("Test");

        // Act
        scenario
            .Given("initial state", ctx => { })
            .When("action occurs", ctx => { executed = true; });
        await scenario.RunAsync();

        // Assert
        executed.Should().BeTrue();
    }

    [Fact]
    public async Task Then_AddsStepAndExecutes()
    {
        // Arrange
        var executed = false;
        var scenario = Scenario.Create("Test");

        // Act
        scenario
            .Given("initial state", ctx => { })
            .When("action occurs", ctx => { })
            .Then("expected result", ctx => { executed = true; });
        await scenario.RunAsync();

        // Assert
        executed.Should().BeTrue();
    }

    [Fact]
    public async Task And_AfterGiven_ExecutesAsGivenStep()
    {
        // Arrange
        var executionCount = 0;
        var scenario = Scenario.Create("Test");

        // Act
        scenario
            .Given("first setup", ctx => { executionCount++; })
            .And("second setup", ctx => { executionCount++; })
            .When("action", ctx => { })
            .Then("result", ctx => { });
        await scenario.RunAsync();

        // Assert
        executionCount.Should().Be(2);
    }

    [Fact]
    public async Task And_AfterWhen_ExecutesAsWhenStep()
    {
        // Arrange
        var executionCount = 0;
        var scenario = Scenario.Create("Test");

        // Act
        scenario
            .Given("setup", ctx => { })
            .When("first action", ctx => { executionCount++; })
            .And("second action", ctx => { executionCount++; })
            .Then("result", ctx => { });
        await scenario.RunAsync();

        // Assert
        executionCount.Should().Be(2);
    }

    [Fact]
    public async Task And_AfterThen_ExecutesAsThenStep()
    {
        // Arrange
        var executionCount = 0;
        var scenario = Scenario.Create("Test");

        // Act
        scenario
            .Given("setup", ctx => { })
            .When("action", ctx => { })
            .Then("first assertion", ctx => { executionCount++; })
            .And("second assertion", ctx => { executionCount++; });
        await scenario.RunAsync();

        // Assert
        executionCount.Should().Be(2);
    }

    [Fact]
    public void And_WithoutPrecedingStep_ThrowsInvalidOperationException()
    {
        // Arrange
        var scenario = Scenario.Create("Test");

        // Act
        Action act = () => scenario.And("invalid step", ctx => { });

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("And must follow a Given, When, or Then step");
    }

    [Fact]
    public async Task When_AsFirstStep_ExecutesSuccessfully()
    {
        // Arrange
        var executed = false;
        var scenario = Scenario.Create("Test");

        // Act
        scenario
            .When("action without setup", ctx => { executed = true; })
            .Then("result", ctx => { });
        await scenario.RunAsync();

        // Assert
        executed.Should().BeTrue();
    }

    [Fact]
    public void Then_WithoutWhen_ThrowsInvalidOperationException()
    {
        // Arrange
        var scenario = Scenario.Create("Test");

        // Act
        Action act = () => scenario
            .Given("setup", ctx => { })
            .Then("assertion without action", ctx => { });

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Then must follow When");
    }

    [Fact]
    public void Given_AfterWhen_ThrowsInvalidOperationException()
    {
        // Arrange
        var scenario = Scenario.Create("Test");

        // Act
        Action act = () => scenario
            .Given("first setup", ctx => { })
            .When("action", ctx => { })
            .Given("setup after action", ctx => { });

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Given must come before When and Then steps");
    }

    [Fact]
    public void Given_AfterThen_ThrowsInvalidOperationException()
    {
        // Arrange
        var scenario = Scenario.Create("Test");

        // Act
        Action act = () => scenario
            .Given("first setup", ctx => { })
            .When("action", ctx => { })
            .Then("assertion", ctx => { })
            .Given("setup after assertion", ctx => { });

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Given must come before When and Then steps");
    }

    [Fact]
    public void When_AfterThen_ThrowsInvalidOperationException()
    {
        // Arrange
        var scenario = Scenario.Create("Test");

        // Act
        Action act = () => scenario
            .Given("setup", ctx => { })
            .When("first action", ctx => { })
            .Then("assertion", ctx => { })
            .When("action after assertion", ctx => { });

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("When cannot follow Then");
    }

    [Fact]
    public async Task Context_IsSharedAcrossSteps()
    {
        // Arrange
        var scenario = Scenario.Create("Test");
        var capturedValue = string.Empty;

        // Act
        scenario
            .Given("value is set", ctx => ctx["key"] = "test-value")
            .When("value is read", ctx => capturedValue = ctx["key"] as string ?? string.Empty)
            .Then("value is verified", ctx => { });
        await scenario.RunAsync();

        // Assert
        capturedValue.Should().Be("test-value");
    }

    [Fact]
    public async Task RunAsync_ExecutesStepsInOrder()
    {
        // Arrange
        var executionOrder = new List<string>();
        var scenario = Scenario.Create("Test");

        // Act
        scenario
            .Given("first", ctx => executionOrder.Add("Given1"))
            .And("second", ctx => executionOrder.Add("Given2"))
            .When("action", ctx => executionOrder.Add("When"))
            .And("another action", ctx => executionOrder.Add("When2"))
            .Then("assertion", ctx => executionOrder.Add("Then"))
            .And("another assertion", ctx => executionOrder.Add("Then2"));
        await scenario.RunAsync();

        // Assert
        executionOrder.Should().Equal("Given1", "Given2", "When", "When2", "Then", "Then2");
    }

    [Fact]
    public async Task RunAsync_ClearsContextAfterExecution()
    {
        // Arrange
        var scenario = Scenario.Create("Test");
        var contextWasEmpty = false;

        // Act
        scenario
            .Given("value is set", ctx => ctx["key"] = "value")
            .When("value exists", ctx => ctx.ContainsKey("key").Should().BeTrue())
            .Then("done", ctx => { });
        await scenario.RunAsync();

        // Try to use context in a new run (it should be cleared)
        scenario
            .Given("check empty", ctx => contextWasEmpty = !ctx.ContainsKey("key"))
            .When("action", ctx => { })
            .Then("done", ctx => { });
        await scenario.RunAsync();

        // Assert
        contextWasEmpty.Should().BeTrue();
    }

    [Fact]
    public async Task MultipleGiven_CanBeChained()
    {
        // Arrange
        var count = 0;
        var scenario = Scenario.Create("Test");

        // Act
        scenario
            .Given("first", ctx => count++)
            .Given("second", ctx => count++)
            .Given("third", ctx => count++)
            .When("action", ctx => { })
            .Then("result", ctx => { });
        await scenario.RunAsync();

        // Assert
        count.Should().Be(3);
    }

    [Fact]
    public async Task MultipleWhen_CanBeChained()
    {
        // Arrange
        var count = 0;
        var scenario = Scenario.Create("Test");

        // Act
        scenario
            .Given("setup", ctx => { })
            .When("first", ctx => count++)
            .When("second", ctx => count++)
            .When("third", ctx => count++)
            .Then("result", ctx => { });
        await scenario.RunAsync();

        // Assert
        count.Should().Be(3);
    }

    [Fact]
    public async Task MultipleThen_CanBeChained()
    {
        // Arrange
        var count = 0;
        var scenario = Scenario.Create("Test");

        // Act
        scenario
            .Given("setup", ctx => { })
            .When("action", ctx => { })
            .Then("first", ctx => count++)
            .Then("second", ctx => count++)
            .Then("third", ctx => count++);
        await scenario.RunAsync();

        // Assert
        count.Should().Be(3);
    }

    [Fact]
    public async Task WhenThen_WithoutGiven_ExecutesSuccessfully()
    {
        // Arrange
        var whenExecuted = false;
        var thenExecuted = false;
        var scenario = Scenario.Create("Test");

        // Act
        scenario
            .When("action occurs", ctx => { whenExecuted = true; })
            .Then("expected result", ctx => { thenExecuted = true; });
        await scenario.RunAsync();

        // Assert
        whenExecuted.Should().BeTrue();
        thenExecuted.Should().BeTrue();
    }

    [Fact]
    public void Then_WithoutWhen_StillThrowsInvalidOperationException()
    {
        // Arrange
        var scenario = Scenario.Create("Test");

        // Act
        Action act = () => scenario.Then("assertion without action", ctx => { });

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Then must follow When");
    }
}
