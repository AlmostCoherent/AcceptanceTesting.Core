using FluentAssertions;
using NorthStandard.Testing.ScreenPlayFramework.Domain.Abstractions;
using Xunit;

namespace NorthStandard.Testing.ScreenPlayFramework.Tests.Domain.Abstractions
{
    // Test implementations for interface validation
    public class TestActor : IActor { }
    public class TestContext : IContext { }
    public class TestOrchestrator : IOrchestrator { }
    public class TestValidator : IValidator { }
    public class TestPage : IPage { }

    public class DomainAbstractionsTests
    {
        [Fact]
        public void IActor_CanBeImplemented()
        {
            // Act
            var actor = new TestActor();

            // Assert
            actor.Should().NotBeNull();
            actor.Should().BeAssignableTo<IActor>();
        }

        [Fact]
        public void IContext_CanBeImplemented()
        {
            // Act
            var context = new TestContext();

            // Assert
            context.Should().NotBeNull();
            context.Should().BeAssignableTo<IContext>();
        }

        [Fact]
        public void IOrchestrator_CanBeImplemented()
        {
            // Act
            var orchestrator = new TestOrchestrator();

            // Assert
            orchestrator.Should().NotBeNull();
            orchestrator.Should().BeAssignableTo<IOrchestrator>();
        }

        [Fact]
        public void IValidator_CanBeImplemented()
        {
            // Act
            var validator = new TestValidator();

            // Assert
            validator.Should().NotBeNull();
            validator.Should().BeAssignableTo<IValidator>();
        }

        [Fact]
        public void IPage_CanBeImplemented()
        {
            // Act
            var page = new TestPage();

            // Assert
            page.Should().NotBeNull();
            page.Should().BeAssignableTo<IPage>();
        }

        [Fact]
        public void AllAbstractions_AreInCorrectNamespace()
        {
            // Assert
            typeof(IActor).Namespace.Should().Be("NorthStandard.Testing.ScreenPlayFramework.Domain.Abstractions");
            typeof(IContext).Namespace.Should().Be("NorthStandard.Testing.ScreenPlayFramework.Domain.Abstractions");
            typeof(IOrchestrator).Namespace.Should().Be("NorthStandard.Testing.ScreenPlayFramework.Domain.Abstractions");
            typeof(IValidator).Namespace.Should().Be("NorthStandard.Testing.ScreenPlayFramework.Domain.Abstractions");
            typeof(IPage).Namespace.Should().Be("NorthStandard.Testing.ScreenPlayFramework.Domain.Abstractions");
        }

        [Fact]
        public void MultipleImplementations_CanImplementSameInterface()
        {
            // Arrange & Act
            var actor1 = new TestActor();
            var actor2 = new TestActor();

            // Assert
            actor1.Should().BeOfType<TestActor>();
            actor2.Should().BeOfType<TestActor>();
            actor1.Should().NotBeSameAs(actor2);
            actor1.Should().BeAssignableTo<IActor>();
            actor2.Should().BeAssignableTo<IActor>();
        }

        [Fact]
        public void Interfaces_DoNotHaveMembers()
        {
            // These interfaces are marker interfaces and should not define members
            
            // Assert
            typeof(IActor).GetMethods().Should().HaveCount(0, "IActor should be a marker interface");
            typeof(IContext).GetMethods().Should().HaveCount(0, "IContext should be a marker interface");
            typeof(IOrchestrator).GetMethods().Should().HaveCount(0, "IOrchestrator should be a marker interface");
            typeof(IValidator).GetMethods().Should().HaveCount(0, "IValidator should be a marker interface");
            typeof(IPage).GetMethods().Should().HaveCount(0, "IPage should be a marker interface");
            
            typeof(IActor).GetProperties().Should().HaveCount(0, "IActor should be a marker interface");
            typeof(IContext).GetProperties().Should().HaveCount(0, "IContext should be a marker interface");
            typeof(IOrchestrator).GetProperties().Should().HaveCount(0, "IOrchestrator should be a marker interface");
            typeof(IValidator).GetProperties().Should().HaveCount(0, "IValidator should be a marker interface");
            typeof(IPage).GetProperties().Should().HaveCount(0, "IPage should be a marker interface");
        }
    }
}