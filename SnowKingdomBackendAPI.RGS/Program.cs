using SnowKingdomBackendAPI.ApiService.Services;
using SnowKingdomBackendAPI.RGS.Services;

namespace SnowKingdomBackendAPI.RGS;

public class BackendOptions
{
    public string Url { get; set; } = "http://localhost:5001/play";
}

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add service defaults & Aspire client integrations.
        builder.AddServiceDefaults();

        // Add controllers
        builder.Services.AddControllers();

        // Add SessionService
        builder.Services.AddSingleton<SessionService>();

        // Add HttpClient for backend communication
        builder.Services.AddHttpClient();

        // Add RNG service
        builder.Services.AddHttpClient<RngService>();

        // Add configuration for backend URL
        builder.Services.Configure<BackendOptions>(builder.Configuration.GetSection("Backend"));

        // Add CORS policy to allow the frontend to call the RGS
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.WithOrigins("http://localhost:3000") // Your frontend URL
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
        });

        var app = builder.Build();

        app.UseCors();
        app.MapDefaultEndpoints();

        // Map controllers
        app.MapControllers();

        // Add health check endpoint
        app.MapGet("/health", () => Results.Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow }))
           .WithName("HealthCheck");

        app.Run();
    }
}