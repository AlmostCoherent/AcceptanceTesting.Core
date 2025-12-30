using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcceptanceTesting.Api.Acceptance.Engine.Core
{
    public class ApiContextProvider {
        private IPlaywright? playwright;
        public async Task Initialise() {
            playwright = await Playwright.CreateAsync();
        }

        public Task<IAPIRequestContext> GetContextAsync()
        {
            var options = new APIRequestNewContextOptions
            {
                BaseURL = "",
            };
            return playwright!.APIRequest.NewContextAsync(options);
        }
    }
}
