namespace NorthStandard.Testing.ScreenPlayFramework.Domain.Abstractions
{
    /// <summary>
    /// Marker interface for a testing engine actor.
    /// Marking your class with this will allow auto Ioc registration when you call <see cref="IServiceCollectionExtensions.AddOptimisTestingEngineComponentsForAssembly(Microsoft.Extensions.DependencyInjection.IServiceCollection, System.Reflection.Assembly)"/>
    /// </summary>
    /// <remarks>
    /// An actor is an object that does an action in your testing engine, ie either called by a "Given" or a "When" clause in BDD terms
    /// </remarks>
    public interface IActor
    {
    }
}
