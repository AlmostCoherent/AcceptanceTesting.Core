using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NorthStandard.Testing.Hosting.Domain.Abstractions;
using System;

namespace NorthStandard.Testing.Hosting.Infrastructure.Angular
{
    public class AngularTestingProfile : IAppProfile
    {
        public string Name => "AngularTesting";
        /// <summary>
        /// This path is from the repository root to the Angular application folder where a command to start the application should be run.
        /// <example>
        /// <code>
        /// Example: "src/web"
        /// </code>
        /// </example>
        /// If left blank, the application will be started from the repository root.
        /// </summary>
        public string BaseAppFilePath { get; } = string.Empty;
        private const string BaseAppFileConfigPath = "Profiles:AngularTesting:BaseAppFileConfigPath";
        /// <summary>
        /// The command to run the Angular application, e.g. "npm start" or "ng serve". This command will be executed in the folder specified by BaseAppFilePath.
        /// </summary>
        public string ApplicationRunCommand { get; }
        private const string ApplicationRunCommandConfigPath = "Profiles:AngularTesting:ApplicationRunCommand";

        public AngularTestingProfile(IConfiguration config)
        {
            BaseAppFilePath = config[BaseAppFileConfigPath] ?? throw new ArgumentNullException(nameof(BaseAppFilePath));
            ApplicationRunCommand = config[ApplicationRunCommandConfigPath] ?? throw new ArgumentNullException(nameof(ApplicationRunCommandConfigPath));
        }

        public void ConfigureServices(IServiceCollection services, IConfiguration config)
        {
        }
    }
}