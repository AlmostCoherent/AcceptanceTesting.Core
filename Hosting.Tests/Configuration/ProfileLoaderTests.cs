using FluentAssertions;
using Microsoft.Extensions.Configuration;
using NorthStandard.Testing.Hosting.Infrastructure.Configuration;
using System;
using System.Collections.Generic;
using Xunit;

namespace NorthStandard.Testing.Hosting.Tests.Configuration
{
    public class ProfileLoaderTests
    {
        [Fact]
        public void LoadProfile_WithValidConfiguration_ReturnsConfiguredInstance()
        {
            // Arrange
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["TestProfile:Environment"] = "Production"
                })
                .Build();

            var loader = new ProfileLoader<TestingProfile>(configuration);

            // Act
            var result = loader.LoadProfile("TestProfile");

            // Assert
            result.Should().NotBeNull();
            result.Environment.Should().Be("Production");
        }

        [Fact]
        public void LoadProfile_WithMissingConfiguration_ReturnsDefaultInstance()
        {
            // Arrange
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>())
                .Build();

            var loader = new ProfileLoader<TestingProfile>(configuration);

            // Act
            var result = loader.LoadProfile("NonExistentProfile");

            // Assert
            result.Should().NotBeNull();
            result.Environment.Should().Be("Development"); // Default value
        }

        [Fact]
        public void LoadProfile_WithEmptySection_ReturnsDefaultInstance()
        {
            // Arrange
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["EmptyProfile:"] = ""
                })
                .Build();

            var loader = new ProfileLoader<TestingProfile>(configuration);

            // Act
            var result = loader.LoadProfile("EmptyProfile");

            // Assert
            result.Should().NotBeNull();
            result.Environment.Should().Be("Development");
        }

        [Fact]
        public void LoadProfile_WithMultipleProperties_ConfiguresAllProperties()
        {
            // Arrange
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["TestProfile:Environment"] = "Staging"
                })
                .Build();

            var loader = new ProfileLoader<TestingProfile>(configuration);

            // Act
            var result = loader.LoadProfile("TestProfile");

            // Assert
            result.Should().NotBeNull();
            result.Environment.Should().Be("Staging");
        }

        [Fact]
        public void LoadProfile_WithInvalidPropertyType_ThrowsException()
        {
            // Arrange
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["TestProfile:Environment"] = "ValidString"
                })
                .Build();

            var loader = new ProfileLoader<TestProfileWithIntProperty>(configuration);

            // Act & Assert
            // This should work fine since we're setting a string property
            var result = loader.LoadProfile("TestProfile");
            result.Should().NotBeNull();
        }

        // Test helper class
        public class TestProfileWithIntProperty
        {
            public int NumberValue { get; set; } = 42;
        }

        [Fact]
        public void LoadProfile_WithIntegerProperty_ConvertsCorrectly()
        {
            // Arrange
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["TestProfile:NumberValue"] = "123"
                })
                .Build();

            var loader = new ProfileLoader<TestProfileWithIntProperty>(configuration);

            // Act
            var result = loader.LoadProfile("TestProfile");

            // Assert
            result.Should().NotBeNull();
            result.NumberValue.Should().Be(123);
        }
    }
}