using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NorthStandard.Testing.Hosting.Infrastructure.Configuration;
using System.Collections.Generic;
using Xunit;

namespace NorthStandard.Testing.Hosting.Tests.Configuration
{
    public class WebTestingProfileTests
    {
        [Fact]
        public void Constructor_WithDefaultConfiguration_SetsDefaultValues()
        {
            // Arrange
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>())
                .Build();

            // Act
            var profile = new WebTestingProfile(configuration);

            // Assert
            profile.Name.Should().Be("WebTesting");
            profile.Options.BaseUrl.Should().Be("https://localhost:5001");
            profile.Options.UseLocalAppInstance.Should().BeTrue();
        }

        [Fact]
        public void Constructor_WithCustomConfiguration_SetsCustomValues()
        {
            // Arrange
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Profiles:WebTesting:BaseUrl"] = "https://custom.example.com",
                    ["Profiles:WebTesting:UseLocalAppInstance"] = "false"
                })
                .Build();

            // Act
            var profile = new WebTestingProfile(configuration);

            // Assert
            profile.Name.Should().Be("WebTesting");
            profile.Options.BaseUrl.Should().Be("https://custom.example.com");
            profile.Options.UseLocalAppInstance.Should().BeFalse();
        }

        [Fact]
        public void Constructor_WithPartialConfiguration_UsesMixOfDefaultAndCustom()
        {
            // Arrange
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Profiles:WebTesting:BaseUrl"] = "https://partial.example.com"
                    // UseLocalAppInstance not specified, should default to true
                })
                .Build();

            // Act
            var profile = new WebTestingProfile(configuration);

            // Assert
            profile.Options.BaseUrl.Should().Be("https://partial.example.com");
            profile.Options.UseLocalAppInstance.Should().BeTrue();
        }

        [Fact]
        public void ConfigureServices_WithValidInputs_DoesNotThrow()
        {
            // Arrange
            var configuration = new ConfigurationBuilder().Build();
            var profile = new WebTestingProfile(configuration);
            var services = new ServiceCollection();

            // Act & Assert
            var act = () => profile.ConfigureServices(services, configuration);
            act.Should().NotThrow();
        }

        [Fact]
        public void Name_Property_ReturnsCorrectValue()
        {
            // Arrange
            var configuration = new ConfigurationBuilder().Build();
            var profile = new WebTestingProfile(configuration);

            // Act & Assert
            profile.Name.Should().Be("WebTesting");
        }
    }
}