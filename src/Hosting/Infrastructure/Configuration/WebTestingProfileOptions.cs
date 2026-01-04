namespace NorthStandard.Testing.Hosting.Infrastructure.Configuration
{
    public class WebTestingProfileOptions
    {
        /// <summary>
        /// If this is true, the test framework will start a local instance of the application for testing.
        /// The test run will be significantly slower. If false, tests will connect to an existing instance at BaseUrl.
        /// </summary>
        public bool UseLocalAppInstance { get; set; } = true;
        /// <summary>
        /// If UseLocalAppInstance is false, this is the base URL of the application to test against.
        /// </summary>
        public string BaseUrl { get; set; } = "https://localhost:5001";
    }
}
