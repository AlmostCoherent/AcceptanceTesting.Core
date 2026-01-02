namespace NorthStandard.Testing.Hosting.Infrastructure.Configuration
{
    /// <summary>
    /// Simple configuration options for testing environment
    /// </summary>
    public class TestingProfileOptions
    {
        public string Environment { get; set; } = "Development";
    }

    /// <summary>
    /// Legacy class - consider using TestingProfileOptions directly with configuration binding
    /// </summary>
    public class TestingProfile : TestingProfileOptions
    {
    }
}
