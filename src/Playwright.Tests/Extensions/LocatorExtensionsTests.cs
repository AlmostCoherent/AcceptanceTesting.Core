using FluentAssertions;
using Microsoft.Playwright;
using Moq;
using NorthStandard.Testing.Playwright.Domain.Entities;
using NorthStandard.Testing.Playwright.Extensions;
using Xunit;

namespace NorthStandard.Testing.Playwright.Tests.Extensions
{
    public class LocatorExtensionsTests
    {
        [Fact]
        public void AsButton_WithValidLocator_ReturnsButton()
        {
            // Arrange
            var mockLocator = new Mock<ILocator>();

            // Act
            var result = mockLocator.Object.AsButton();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Button>();
        }

        [Fact]
        public void AsButton_WithNullLocator_ReturnsButtonWithNullLocator()
        {
            // Arrange
            ILocator? nullLocator = null;

            // Act
            var result = nullLocator!.AsButton();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Button>();
        }

        [Fact]
        public void AsButton_CalledMultipleTimes_ReturnsNewInstanceEachTime()
        {
            // Arrange
            var mockLocator = new Mock<ILocator>();

            // Act
            var result1 = mockLocator.Object.AsButton();
            var result2 = mockLocator.Object.AsButton();

            // Assert
            result1.Should().NotBeSameAs(result2);
            result1.Should().BeOfType<Button>();
            result2.Should().BeOfType<Button>();
        }

        [Fact]
        public void AsButton_WithDifferentLocators_ReturnsButtonsWithDifferentLocators()
        {
            // Arrange
            var mockLocator1 = new Mock<ILocator>();
            var mockLocator2 = new Mock<ILocator>();

            // Act
            var button1 = mockLocator1.Object.AsButton();
            var button2 = mockLocator2.Object.AsButton();

            // Assert
            button1.Should().NotBeNull();
            button2.Should().NotBeNull();
            button1.Should().NotBeSameAs(button2);
        }

        [Fact]
        public void AsButton_IsExtensionMethod_CanBeCalledOnILocator()
        {
            // Arrange
            var mockLocator = new Mock<ILocator>();
            ILocator locator = mockLocator.Object;

            // Act & Assert - This test ensures the extension method works on the interface
            var act = () => locator.AsButton();
            act.Should().NotThrow();
            
            var result = act();
            result.Should().BeOfType<Button>();
        }
    }
}