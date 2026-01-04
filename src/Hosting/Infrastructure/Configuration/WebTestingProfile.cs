using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NorthStandard.Testing.Hosting.Domain.Abstractions;

namespace NorthStandard.Testing.Hosting.Infrastructure.Configuration
{

    public class WebTestingProfile : IAppProfile
    {
        public string Name => "WebTesting";
        public WebTestingProfileOptions Options { get; }

        public WebTestingProfile(IConfiguration config)
        {
            Options = config.BindConfigurationOrDefault<WebTestingProfileOptions>("Profiles:WebTesting");
        }

        public void ConfigureServices(IServiceCollection services, IConfiguration config)
        {
            // Configuration logic here if needed
        }
    }
}
