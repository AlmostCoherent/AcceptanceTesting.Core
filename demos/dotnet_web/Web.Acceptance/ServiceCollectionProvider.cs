using Microsoft.Extensions.DependencyInjection;
using Reqnroll.Microsoft.Extensions.DependencyInjection;

namespace AlmostCoherent.Testing.Demos.Web.Acceptance
{
    public static class ServicesCollectionProvider
    {
        private readonly static IServiceCollection services = new ServiceCollection();

        [ScenarioDependencies(AutoRegisterBindings = true, ScopeLevel = ScopeLevelType.Scenario)]
        public static IServiceCollection CreateServices() => services;
    }
}
