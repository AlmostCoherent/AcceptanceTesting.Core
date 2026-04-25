namespace AcceptanceTesting.Core
{
    /// <summary>
    /// Builds a url from a given base url and a relative one
    /// </summary>
    public class UrlBuilder
    {
        private string baseUrl;

        /// <summary>
        /// Construct the builder
        /// </summary>
        /// <param name="baseUrl">The given base url</param>
        public UrlBuilder(string baseUrl)
        {
            this.baseUrl = baseUrl;
        }

        /// <summary>
        /// Returns the base urll for the application in test.
        /// </summary>
        /// <returns>The base url</returns>
        public string GetBaseUrl() => $"{baseUrl}";

        /// <summary>
        /// Constructs a full url from the base url stored on construction followed by the given relative url
        /// </summary>
        /// <param name="relativeUrl">The relative url to proceed the base url</param>
        /// <returns>The full combined url</returns>
        public string GetUrl(string relativeUrl) => $"{baseUrl}{relativeUrl}";
    }
}