using NorthStandard.Testing.ScreenPlayFramework.Domain.Abstractions;

namespace NorthStandard.Testing.ScreenPlayFramework.Application.Core.Contexts
{
    public class CoreWebContext : IContext
    {
        /// <summary>
        /// Holds the URL of the web application. 
        /// </summary>
        public string WebBaseUrl { get; set; } = string.Empty;
    }
}
