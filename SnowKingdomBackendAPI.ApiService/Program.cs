using SnowKingdomBackendAPI.ApiService.Game;
using SnowKingdomBackendAPI.ApiService.Models;
using SnowKingdomBackendAPI.ApiService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add Aspire service defaults
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();
builder.Services.AddSingleton<GameEngine>();
builder.Services.AddSingleton<SessionService>();

// Configure JSON to use camelCase
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
});

var app = builder.Build();

// Map Aspire's default endpoints
app.MapDefaultEndpoints();

// The endpoint now just calls the GameEngine
app.MapPost("/play", (PlayRequest request, GameEngine gameEngine) =>
{
    try
    {
        var result = gameEngine.Play(request);
        return Results.Ok(result);
    }
    catch (InvalidOperationException ex) // Catches specific errors like "Insufficient balance"
    {
        return Results.BadRequest(new { error = ex.Message });
    }
    catch (Exception ex) // Catches any other unexpected errors
    {
        return Results.Problem($"An error occurred: {ex.Message}");
    }
});

app.Run();