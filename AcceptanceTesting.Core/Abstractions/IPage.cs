namespace Testing.Acceptance.Core.Abstractions
{
    /// <summary>
    /// Marker interface for a testing engine page.
    /// Marking your class with this will allow auto Ioc registration when you call <see cref="IServiceCollectionExtensions.AddOptimisTestingEngineComponentsForAssembly(Microsoft.Extensions.DependencyInjection.IServiceCollection, System.Reflection.Assembly)"/>
    /// </summary>
    /// <remarks>
    /// A page is an object that contains methods for interacting with a specific page in the application under test.
    /// </remarks>
    public interface IPage
    {
    }
}
