using Microsoft.Extensions.DependencyInjection;
using Reqnroll.Microsoft.Extensions.DependencyInjection;

namespace NorthStandard.Testing.Demos.Api.Acceptance
{
    public static class ServicesCollectionProvider
    {
        private readonly static IServiceCollection services = new ServiceCollection();

        [ScenarioDependencies(AutoRegisterBindings = true, ScopeLevel = ScopeLevelType.Scenario)]
        public static IServiceCollection CreateServices() => services;
    }
}
