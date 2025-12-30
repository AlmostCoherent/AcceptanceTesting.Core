using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NorthStandard.Testing.Playwright.Application.Services;
using NorthStandard.Testing.ScreenPlayFramework.Domain.Abstractions;
using NorthStandard.Testing.ScreenPlayFramework.Infrastructure.Api.Contexts;

namespace NorthStandard.Testing.ScreenPlayFramework.Infrastructure.Api.Actors
{
    public class ApiActor(UrlBuilder urlBuilder, ApiContext apiContextProvider, ApiRequestContext apiRequestContext, ILogger<ApiActor> logger) : IActor
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        /// <summary>
        /// Sends a GET request and deserializes the response.
        /// </summary>
        public async Task<TResponse> GetAsync<TResponse>(string relativeUrl) where TResponse : new()
        {
            var client = apiContextProvider.GetTestClient();
            string requestUrl = urlBuilder.GetUrl(relativeUrl);

            logger.LogInformation("GET {Url}", requestUrl);

            HttpResponseMessage response = await client.GetAsync(requestUrl);
            return await HandleResponse<TResponse>(response);
        }

        /// <summary>
        /// Sends a GET request and deserializes the response.
        /// </summary>
        public async Task<TResponse> PostAsync<TRequest, TResponse>(string relativeUrl, TRequest request) where TResponse : new()
        {
            var client = apiContextProvider.GetTestClient();
            string requestUrl = urlBuilder.GetUrl(relativeUrl);

            logger.LogInformation("POST {Url}", requestUrl);

            try
            {
                string jsonData = JsonSerializer.Serialize(request);
                using var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(requestUrl, content);
                return await HandleResponse<TResponse>(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while making POST request to {Url}", requestUrl);
                throw;
            }
        }


        /// <summary>
        /// Handles the HTTP response, logs the result, and deserializes the content.
        /// </summary>
        private async Task<TResponse> HandleResponse<TResponse>(HttpResponseMessage response) where TResponse : new()
        {
            apiRequestContext.StatusCodeResult = new StatusCodeResult((int)response.StatusCode);

            string responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                logger.LogInformation("API request success. Status Code: {StatusCode}", (int)response.StatusCode);
                logger.LogDebug("Response content: {Content}", responseContent);

                var result = JsonSerializer.Deserialize<TResponse>(responseContent, JsonOptions) ?? new TResponse();
                return result;
            }
            else
            {
                logger.LogWarning("API request failed. Status Code: {StatusCode}", (int)response.StatusCode);
                logger.LogWarning("Error response content: {Content}", responseContent);

                return new TResponse();
            }
        }
    }
}
