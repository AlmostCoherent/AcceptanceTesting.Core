using AcceptanceTesting.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcceptanceTesting.Core.Engine.Core.Contexts
{
    public class CoreWebContext : IContext
    {
        /// <summary>
        /// Holds the URL of the web application. 
        /// </summary>
        public string WebBaseUrl { get; set; } = string.Empty;
    }
}
