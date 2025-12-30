using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NorthStandard.Testing.Hosting.Domain.Abstractions
{
    public interface IAppProfile {
        string Name { get; }
        void ConfigureServices(IServiceCollection services, IConfiguration config);
    }
}
