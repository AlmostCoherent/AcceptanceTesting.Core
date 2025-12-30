namespace AcceptanceTesting.Core.Abstractions
{
    /// <summary>
    /// Marker interface for a testing engine validator.
    /// Marking your class with this will allow auto Ioc registration when you call <see cref="IServiceCollectionExtensions.AddOptimisTestingEngineComponentsForAssembly(Microsoft.Extensions.DependencyInjection.IServiceCollection, System.Reflection.Assembly)"/>
    /// </summary>
    /// <remarks>
    /// A validatoris an object that does validation in your testing engine, ie called by "Then" clause in BDD terms
    /// </remarks>
    public interface IValidator
    {
    }
}
