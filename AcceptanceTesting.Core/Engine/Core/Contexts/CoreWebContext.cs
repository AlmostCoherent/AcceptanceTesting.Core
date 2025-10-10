using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testing.Acceptance.Core.Abstractions;

namespace Testing.Acceptance.Core.Engine.Core.Contexts
{
    public class CoreWebContext : IContext
    {
        /// <summary>
        /// Holds the URL of the web application. 
        /// </summary>
        public string WebBaseUrl { get; set; } = string.Empty;
    }
}
