using FluentAssertions;
using NorthStandard.Testing.Playwright.Infrastructure.Configuration;
using Xunit;

namespace NorthStandard.Testing.Playwright.Tests.Infrastructure.Configuration
{
    public class PlaywrightConfigurationTests
    {
        [Fact]
        public void Constructor_SetsDefaultValues()
        {
            // Act
            var config = new PlaywrightConfiguration();

            // Assert
            config.EnableCaptureForFailingTests.Should().BeTrue();
            config.EnableHeadlessBrowser.Should().BeFalse(); // Default value
            config.WaitTimeOut.Should().Be(10000);
            config.EnableTracing.Should().BeFalse(); // Default value
            config.CaptureScreenshots.Should().BeFalse(); // Default value
            config.FullPageScreenshots.Should().BeFalse(); // Default value
            config.ArtifactsPath.Should().Be("TestResults");
            config.TracingOptions.Should().NotBeNull();
        }

        [Fact]
        public void IsEnabled_CanBeSet()
        {
            // Arrange
            var config = new PlaywrightConfiguration();

            // Act
            config.EnableCaptureForFailingTests = false;

            // Assert
            config.EnableCaptureForFailingTests.Should().BeFalse();
        }

        [Fact]
        public void EnableHeadlessBrowser_CanBeSet()
        {
            // Arrange
            var config = new PlaywrightConfiguration();

            // Act
            config.EnableHeadlessBrowser = true;

            // Assert
            config.EnableHeadlessBrowser.Should().BeTrue();
        }

        [Fact]
        public void WaitTimeOut_CanBeSet()
        {
            // Arrange
            var config = new PlaywrightConfiguration();

            // Act
            config.WaitTimeOut = 5000;

            // Assert
            config.WaitTimeOut.Should().Be(5000);
        }

        [Fact]
        public void EnableTracing_CanBeSet()
        {
            // Arrange
            var config = new PlaywrightConfiguration();

            // Act
            config.EnableTracing = true;

            // Assert
            config.EnableTracing.Should().BeTrue();
        }

        [Fact]
        public void CaptureScreenshots_CanBeSet()
        {
            // Arrange
            var config = new PlaywrightConfiguration();

            // Act
            config.CaptureScreenshots = true;

            // Assert
            config.CaptureScreenshots.Should().BeTrue();
        }

        [Fact]
        public void FullPageScreenshots_CanBeSet()
        {
            // Arrange
            var config = new PlaywrightConfiguration();

            // Act
            config.FullPageScreenshots = true;

            // Assert
            config.FullPageScreenshots.Should().BeTrue();
        }

        [Fact]
        public void ArtifactsPath_CanBeSet()
        {
            // Arrange
            var config = new PlaywrightConfiguration();

            // Act
            config.ArtifactsPath = "CustomPath";

            // Assert
            config.ArtifactsPath.Should().Be("CustomPath");
        }

        [Fact]
        public void TracingOptions_CanBeSet()
        {
            // Arrange
            var config = new PlaywrightConfiguration();
            var newOptions = new TracingOptions();

            // Act
            config.TracingOptions = newOptions;

            // Assert
            config.TracingOptions.Should().BeSameAs(newOptions);
        }

        [Fact]
        public void AllProperties_CanBeSetToValidValues()
        {
            // Arrange
            var config = new PlaywrightConfiguration();

            // Act
            config.EnableCaptureForFailingTests = true;
            config.EnableHeadlessBrowser = true;
            config.WaitTimeOut = 15000;
            config.EnableTracing = true;
            config.CaptureScreenshots = true;
            config.FullPageScreenshots = true;
            config.ArtifactsPath = "/custom/path";
            config.TracingOptions = new TracingOptions
            {
                Screenshots = false,
                Snapshots = false,
                Sources = false
            };

            // Assert
            config.EnableCaptureForFailingTests.Should().BeTrue();
            config.EnableHeadlessBrowser.Should().BeTrue();
            config.WaitTimeOut.Should().Be(15000);
            config.EnableTracing.Should().BeTrue();
            config.CaptureScreenshots.Should().BeTrue();
            config.FullPageScreenshots.Should().BeTrue();
            config.ArtifactsPath.Should().Be("/custom/path");
            config.TracingOptions.Screenshots.Should().BeFalse();
            config.TracingOptions.Snapshots.Should().BeFalse();
            config.TracingOptions.Sources.Should().BeFalse();
        }
    }

    public class TracingOptionsTests
    {
        [Fact]
        public void Constructor_SetsDefaultValues()
        {
            // Act
            var options = new TracingOptions();

            // Assert
            options.Screenshots.Should().BeFalse(); // Default value
            options.Snapshots.Should().BeFalse(); // Default value
            options.Sources.Should().BeFalse(); // Default value
        }

        [Fact]
        public void Screenshots_CanBeSet()
        {
            // Arrange
            var options = new TracingOptions();

            // Act
            options.Screenshots = true;

            // Assert
            options.Screenshots.Should().BeTrue();
        }

        [Fact]
        public void Snapshots_CanBeSet()
        {
            // Arrange
            var options = new TracingOptions();

            // Act
            options.Snapshots = true;

            // Assert
            options.Snapshots.Should().BeTrue();
        }

        [Fact]
        public void Sources_CanBeSet()
        {
            // Arrange
            var options = new TracingOptions();

            // Act
            options.Sources = true;

            // Assert
            options.Sources.Should().BeTrue();
        }

        [Fact]
        public void AllProperties_CanBeSetIndependently()
        {
            // Arrange
            var options = new TracingOptions();

            // Act
            options.Screenshots = true;
            options.Snapshots = false;
            options.Sources = true;

            // Assert
            options.Screenshots.Should().BeTrue();
            options.Snapshots.Should().BeFalse();
            options.Sources.Should().BeTrue();
        }
    }
}