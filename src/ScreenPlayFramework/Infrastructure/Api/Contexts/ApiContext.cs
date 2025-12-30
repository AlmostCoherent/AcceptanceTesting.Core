using NorthStandard.Testing.Playwright.Application.Services;
using NorthStandard.Testing.ScreenPlayFramework.Domain.Abstractions;
using System;
using System.Net.Http;

namespace NorthStandard.Testing.ScreenPlayFramework.Infrastructure.Api.Contexts
{
    public class ApiContext(UrlBuilder urlBuilder) : IDisposable, IContext
    {
        public HttpClient? Client { get; private set; }
        private HttpMessageHandler? handler;

        public HttpClient GetTestClient()
        {
            if (Client == null)
            {
                // Create handler that bypasses SSL certificate validation for self-signed certs in tests
                handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                };

                Client = new HttpClient(handler)
                {
                    BaseAddress = new Uri(urlBuilder.GetBaseUrl())
                };
            }
            return Client;
        }

        public void Dispose()
        {
            Client?.Dispose();
            handler?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
