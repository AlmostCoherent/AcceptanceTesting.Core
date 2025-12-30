using FluentAssertions;
using NorthStandard.Testing.Hosting.Infrastructure.Configuration;
using Xunit;

namespace NorthStandard.Testing.Hosting.Tests.Configuration
{
    public class TestingProfileTests
    {
        [Fact]
        public void Constructor_SetsDefaultEnvironment()
        {
            // Act
            var profile = new TestingProfile();

            // Assert
            profile.Environment.Should().Be("Development");
        }

        [Fact]
        public void Environment_CanBeSet()
        {
            // Arrange
            var profile = new TestingProfile();

            // Act
            profile.Environment = "Production";

            // Assert
            profile.Environment.Should().Be("Production");
        }

        [Fact]
        public void Environment_CanBeSetToNull()
        {
            // Arrange
            var profile = new TestingProfile();

            // Act
            profile.Environment = null!;

            // Assert
            profile.Environment.Should().BeNull();
        }

        [Fact]
        public void Environment_CanBeSetToEmptyString()
        {
            // Arrange
            var profile = new TestingProfile();

            // Act
            profile.Environment = "";

            // Assert
            profile.Environment.Should().Be("");
        }
    }
}