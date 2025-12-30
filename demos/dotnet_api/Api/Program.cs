using AcceptanceTesting.Api.Converters;
using Microsoft.OpenApi.Models;

namespace AcceptanceTesting.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var app = CreateApiHostBuilder(args);
            if (args.Length > 0 && args[0] == "--console")
            {
                Console.WriteLine("Starting TestPlay inside another application...");
                await app.StartAsync();
                Console.WriteLine("Web server is running. Press Enter to stop...");
                Console.ReadLine();
                await app.StopAsync();
            }
            else
            {
                await app.RunAsync();
            }
        }

        public static IHost CreateApiHostBuilder(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Ensure console logging is active so we can see errors
            builder.Logging.AddConsole();

            // Add services to the container.
            builder.Services.AddControllers()
                .AddJsonOptions(opts =>
                {
                    opts.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
                });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TestPlay.Api", Version = "v1" });

                // Map DateOnly and nullable DateOnly to a date string in the OpenAPI schema
                c.MapType<DateOnly>(() => new OpenApiSchema { Type = "string", Format = "date" });
                c.MapType<DateOnly?>(() => new OpenApiSchema { Type = "string", Format = "date" });
            });

            var app = builder.Build();

            // Global exception logging middleware so we capture exceptions that happen
            // while swagger.json is being generated (or any request).
            app.Use(async (context, next) =>
            {
                try
                {
                    await next();
                }
                catch (Exception ex)
                {
                    var logger = app.Services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "Unhandled exception while processing request {Path}", context.Request.Path);
                    throw;
                }
            });

            // Enable Swagger for troubleshooting even if not in Development.
            // Remove or gate this in production.
            app.UseSwagger();
            app.UseSwaggerUI(c => c.RoutePrefix = "swagger");

            // Keep your custom filter middleware (make sure it doesn't block swagger requests)
            app.Use(async (context, next) =>
            {
                if (context.Request.Path.StartsWithSegments("/swagger") || context.Request.Path.StartsWithSegments("/swagger/v1/swagger.json"))
                {
                    // allow swagger requests
                    await next();
                    return;
                }

                if (context.Request.Method == "GET")
                {
                    await next.Invoke();
                }
                else
                {
                    context.Response.StatusCode = 403;
                    await context.Response.WriteAsync("Forbidden request method.");
                }
            });

            app.UseAuthorization();
            app.MapControllers();

            return app;
        }
    }
}
