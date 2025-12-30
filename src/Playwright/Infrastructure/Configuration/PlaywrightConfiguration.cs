namespace NorthStandard.Testing.Playwright.Infrastructure.Configuration
{
    public class PlaywrightConfiguration
    {
        public bool IsEnabled { get; set; } = true;
        public bool EnableHeadlessBrowser { get; set; }
        public int WaitTimeOut { get; set; } = 10000;
        public bool EnableTracing { get; set; }
        public bool CaptureScreenshots { get; set; }
        public bool FullPageScreenshots { get; set; }
        public string ArtifactsPath { get; set; } = "TestResults";
        public TracingOptions TracingOptions { get; set; } = new();
    }

    public class TracingOptions
    {
        public bool Screenshots { get; set; }
        public bool Snapshots { get; set; }
        public bool Sources { get; set; }
    }
}