using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NorthStandard.Testing.ScreenPlayFramework.Infrastructure.Extensions;
using System.Linq;
using System.Reflection;
using Xunit;

namespace NorthStandard.Testing.ScreenPlayFramework.Tests.Infrastructure.Extensions
{
    // Simple test interfaces for testing purposes
    public interface ISimpleTestInterface { }
    public class SimpleTestImplementation : ISimpleTestInterface { }
    public class AnotherSimpleTestImplementation : ISimpleTestInterface { }

    public class SimpleServiceCollectionExtensionsTests
    {
        private readonly IServiceCollection _services;
        private readonly Assembly _testAssembly;

        public SimpleServiceCollectionExtensionsTests()
        {
            _services = new ServiceCollection();
            _testAssembly = Assembly.GetExecutingAssembly();
        }

        [Fact]
        public void AddScopedImplementingFromAssembly_WithSimpleInterface_RegistersImplementations()
        {
            // Act
            _services.AddScopedImplementingFromAssembly<ISimpleTestInterface>(_testAssembly);

            // Assert
            var registeredTypes = _services
                .Where(s => s.ServiceType.IsAssignableTo(typeof(ISimpleTestInterface)))
                .Select(s => s.ServiceType)
                .ToList();

            registeredTypes.Should().Contain(typeof(SimpleTestImplementation));
            registeredTypes.Should().Contain(typeof(AnotherSimpleTestImplementation));
            registeredTypes.Should().NotContain(typeof(ISimpleTestInterface));
        }

        [Fact]
        public void AddScopedImplementingFromAssembly_ReturnsServiceCollection()
        {
            // Act
            var result = _services.AddScopedImplementingFromAssembly<ISimpleTestInterface>(_testAssembly);

            // Assert
            result.Should().BeSameAs(_services);
        }

        [Fact]
        public void AddScopedImplementingFromAssembly_RegistersAsScoped()
        {
            // Act
            _services.AddScopedImplementingFromAssembly<ISimpleTestInterface>(_testAssembly);

            // Assert
            var scopedServices = _services
                .Where(s => s.ServiceType.IsAssignableTo(typeof(ISimpleTestInterface)))
                .ToList();

            scopedServices.Should().AllSatisfy(s => s.Lifetime.Should().Be(ServiceLifetime.Scoped));
        }
    }
}