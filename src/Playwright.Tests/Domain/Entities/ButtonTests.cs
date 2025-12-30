using FluentAssertions;
using Microsoft.Playwright;
using Moq;
using NorthStandard.Testing.Playwright.Domain.Entities;
using System;
using System.Threading.Tasks;
using Xunit;

namespace NorthStandard.Testing.Playwright.Tests.Domain.Entities
{
    public class ButtonTests
    {
        [Fact]
        public async Task ClickAsync_CallsLocatorClickAsync()
        {
            // Arrange
            var mockLocator = new Mock<ILocator>();
            var button = new Button(mockLocator.Object);

            // Act
            await button.ClickAsync();

            // Assert
            mockLocator.Verify(l => l.ClickAsync(It.IsAny<LocatorClickOptions>()), Times.Once);
        }

        [Fact]
        public async Task ClickAndWaitForNetworkIdleAsync_CallsLocatorClickAndWaitsForNetworkIdle()
        {
            // Arrange
            var mockLocator = new Mock<ILocator>();
            var mockPage = new Mock<IPage>();
            var button = new Button(mockLocator.Object);

            // Setup page to return a completed task for WaitForLoadStateAsync
            mockPage.Setup(p => p.WaitForLoadStateAsync(LoadState.NetworkIdle, It.IsAny<PageWaitForLoadStateOptions>()))
                   .Returns(Task.CompletedTask);

            // Act
            await button.ClickAndWaitForNetworkIdleAsync(mockPage.Object);

            // Assert
            mockLocator.Verify(l => l.ClickAsync(It.IsAny<LocatorClickOptions>()), Times.Once);
            mockPage.Verify(p => p.WaitForLoadStateAsync(LoadState.NetworkIdle, It.IsAny<PageWaitForLoadStateOptions>()), Times.Once);
        }

        [Fact]
        public async Task ClickAndWaitForNetworkIdleAsync_StartsWaitThenClicksThenWaits()
        {
            // Arrange
            var mockLocator = new Mock<ILocator>();
            var mockPage = new Mock<IPage>();
            var button = new Button(mockLocator.Object);

            var waitStarted = false;
            var clickCalled = false;

            // Setup to track that wait is started before click
            mockPage.Setup(p => p.WaitForLoadStateAsync(LoadState.NetworkIdle, It.IsAny<PageWaitForLoadStateOptions>()))
                   .Callback(() => {
                       waitStarted = true;
                   })
                   .Returns(Task.CompletedTask);

            mockLocator.Setup(l => l.ClickAsync(It.IsAny<LocatorClickOptions>()))
                     .Callback(() => {
                         waitStarted.Should().BeTrue("Wait should be started before click is executed");
                         clickCalled = true;
                     })
                     .Returns(Task.CompletedTask);

            // Act
            await button.ClickAndWaitForNetworkIdleAsync(mockPage.Object);

            // Assert
            waitStarted.Should().BeTrue();
            clickCalled.Should().BeTrue();
        }

        [Fact]
        public void Constructor_WithValidLocator_DoesNotThrow()
        {
            // Arrange
            var mockLocator = new Mock<ILocator>();

            // Act & Assert
            var act = () => new Button(mockLocator.Object);
            act.Should().NotThrow();
        }

        [Fact]
        public void Constructor_WithNullLocator_DoesNotThrow()
        {
            // Act & Assert
            var act = () => new Button(null!);
            act.Should().NotThrow();
        }

        [Fact]
        public async Task ClickAsync_WithNullLocator_ThrowsNullReferenceException()
        {
            // Arrange
            var button = new Button(null!);

            // Act & Assert
            var act = async () => await button.ClickAsync();
            await act.Should().ThrowAsync<NullReferenceException>();
        }

        [Fact]
        public async Task ClickAndWaitForNetworkIdleAsync_WithNullLocator_ThrowsNullReferenceException()
        {
            // Arrange
            var mockPage = new Mock<IPage>();
            var button = new Button(null!);

            // Act & Assert
            var act = async () => await button.ClickAndWaitForNetworkIdleAsync(mockPage.Object);
            await act.Should().ThrowAsync<NullReferenceException>();
        }

        [Fact]
        public async Task ClickAndWaitForNetworkIdleAsync_WithNullPage_ThrowsNullReferenceException()
        {
            // Arrange
            var mockLocator = new Mock<ILocator>();
            var button = new Button(mockLocator.Object);

            // Act & Assert
            var act = async () => await button.ClickAndWaitForNetworkIdleAsync(null!);
            await act.Should().ThrowAsync<NullReferenceException>();
        }

        [Fact]
        public async Task ClickAndWaitForNetworkIdleAsync_WhenLocatorClickThrows_PropagatesException()
        {
            // Arrange
            var mockLocator = new Mock<ILocator>();
            var mockPage = new Mock<IPage>();
            var button = new Button(mockLocator.Object);

            var expectedException = new InvalidOperationException("Click failed");
            mockLocator.Setup(l => l.ClickAsync(It.IsAny<LocatorClickOptions>()))
                     .ThrowsAsync(expectedException);

            // Act & Assert
            var act = async () => await button.ClickAndWaitForNetworkIdleAsync(mockPage.Object);
            await act.Should().ThrowAsync<InvalidOperationException>()
                     .WithMessage("Click failed");
        }

        [Fact]
        public async Task ClickAndWaitForNetworkIdleAsync_WhenNetworkIdleWaitThrows_PropagatesException()
        {
            // Arrange
            var mockLocator = new Mock<ILocator>();
            var mockPage = new Mock<IPage>();
            var button = new Button(mockLocator.Object);

            var expectedException = new TimeoutException("Network idle timeout");
            mockPage.Setup(p => p.WaitForLoadStateAsync(LoadState.NetworkIdle, It.IsAny<PageWaitForLoadStateOptions>()))
                   .ThrowsAsync(expectedException);

            // Act & Assert
            var act = async () => await button.ClickAndWaitForNetworkIdleAsync(mockPage.Object);
            await act.Should().ThrowAsync<TimeoutException>()
                     .WithMessage("Network idle timeout");
        }
    }
}