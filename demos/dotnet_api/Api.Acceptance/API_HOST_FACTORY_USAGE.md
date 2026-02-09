# ApiHostFactory Usage Guide

The `ApiHostFactory` provides a flexible way to create the API host for testing while allowing test projects to override or add services.

## Basic Usage

### Without Service Overrides

```csharp
[BeforeTestRun(Order = 1)]
public static async Task BeforeTestRunConfigureAndRunApp(UrlBuilder urlBuilder)
{
    string[] args = new[] { $"--urls={urlBuilder.GetBaseUrl()}" };
    server = ApiHostFactory.CreateHost(args);
    await server.StartAsync();
}
```

### With Service Overrides (Inline)

Override services directly when creating the host:

```csharp
[BeforeTestRun(Order = 1)]
public static async Task BeforeTestRunConfigureAndRunApp(UrlBuilder urlBuilder)
{
    string[] args = new[] { $"--urls={urlBuilder.GetBaseUrl()}" };
    
    server = ApiHostFactory.CreateHost(args, services =>
    {
        // Replace the real database service with a mock
        services.AddScoped<IDatabaseService, MockDatabaseService>();
        
        // Add test-specific logging
        services.AddLogging(builder => builder.AddConsole());
    });
    
    await server.StartAsync();
}
```

### With Service Overrides (Pre-Configuration)

Configure services separately, then create the host:

```csharp
[BeforeTestRun(Order = 0)]
public static void ConfigureTestServices()
{
    ApiHostFactory.ConfigureServices(services =>
    {
        // Mock external API calls
        services.AddScoped<IExternalApiClient, MockExternalApiClient>();
        
        // Use in-memory database instead of real one
        services.AddScoped<IDbContext, InMemoryDbContext>();
    });
}

[BeforeTestRun(Order = 1)]
public static async Task BeforeTestRunConfigureAndRunApp(UrlBuilder urlBuilder)
{
    string[] args = new[] { $"--urls={urlBuilder.GetBaseUrl()}" };
    server = ApiHostFactory.CreateHost(args);
    await server.StartAsync();
}

[AfterTestRun]
public static async Task AfterTestRunCleanup()
{
    // Clean up the service configuration
    ApiHostFactory.ResetServiceConfiguration();
    
    if (server != null)
    {
        await server.StopAsync();
    }
}
```

## Key Points

1. **No Method Exposure**: The `Program.CreateApiHostBuilder` method remains internal to the API project
2. **Flexible Configuration**: Test projects can override any service without modifying the API project
3. **Clean Abstraction**: The factory pattern provides a single point of entry for host creation
4. **Reusable**: The factory can be used across multiple test projects in the packaging
5. **Service Isolation**: Each host creation can have different service configurations

## Available Methods

- `CreateHost(string[] args)` - Creates a host with standard configuration
- `CreateHost(string[] args, Action<IServiceCollection> configureServices)` - Creates a host with inline service configuration
- `ConfigureServices(Action<IServiceCollection> configureServices)` - Sets up a callback for service configuration
- `ResetServiceConfiguration()` - Clears any configured service callbacks
