using SnowKingdomBackendAPI.ApiService.Game;
using SnowKingdomBackendAPI.ApiService.Models;
using SnowKingdomBackendAPI.ApiService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();
builder.Services.AddSingleton<GameEngine>();
builder.Services.AddSingleton<SessionService>();

// Configure JSON options to use camelCase for API responses
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
});

// Add CORS policy to allow frontend communication
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(
                "http://localhost:3000",
                "https://localhost:3000",
                "http://localhost:9003",
                "https://localhost:9003",
                "http://localhost:5073",
                "https://localhost:5073",
                "http://localhost:59775",
                "https://localhost:59775",
                "http://localhost:52436",
                "https://localhost:52436",
                "http://127.0.0.1:3000",
                "http://127.0.0.1:9003",
                "http://127.0.0.1:5073",
                "http://127.0.0.1:59775",
                "http://127.0.0.1:52436"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

// Enable CORS
app.UseCors();

app.MapPost("/play", async (PlayRequest request, GameEngine gameEngine, SessionService sessionService) =>
{
    try
    {
        // Get or create session
        var currentState = sessionService.GetOrCreateSession(request.SessionId);

        // Check if player has enough balance or free spins
        if (currentState.Balance < request.BetAmount && currentState.FreeSpinsRemaining == 0)
        {
            return Results.BadRequest(new
            {
                Error = "Insufficient balance",
                CurrentBalance = currentState.Balance,
                FreeSpinsRemaining = currentState.FreeSpinsRemaining
            });
        }

        // Generate new grid
        var grid = gameEngine.GenerateGrid();

        // Evaluate the spin
        var spinResult = gameEngine.EvaluateSpin(grid, request.BetAmount);

        // Update player state
        var newBalance = currentState.Balance;
        var newFreeSpins = currentState.FreeSpinsRemaining;

        if (currentState.FreeSpinsRemaining > 0)
        {
            // Using free spin
            newFreeSpins = currentState.FreeSpinsRemaining - 1;
        }
        else
        {
            // Deduct bet amount
            newBalance = currentState.Balance - request.BetAmount;
        }

        // Add winnings
        newBalance += spinResult.TotalWin;

        // Add free spins if triggered
        if (spinResult.ScatterWin.TriggeredFreeSpins)
        {
            newFreeSpins += GameConstants.FreeSpinsAwarded;
        }

        // Update session state
        var newState = new GameState
        {
            Balance = newBalance,
            FreeSpinsRemaining = newFreeSpins,
            LastWin = spinResult.TotalWin,
            Results = spinResult
        };

        sessionService.UpdateSession(request.SessionId, newState);

        // Return response
        return Results.Ok(new PlayResponse
        {
            SessionId = request.SessionId,
            Player = newState,
            Game = newState,
            FreeSpins = newFreeSpins
        });
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error processing spin: {ex.Message}");
    }
})
.WithName("PlayGame");

app.MapGet("/session/{sessionId}", (string sessionId, SessionService sessionService) =>
{
    var session = sessionService.GetOrCreateSession(sessionId);
    return Results.Ok(new PlayResponse
    {
        SessionId = sessionId,
        Player = session,
        Game = session,
        FreeSpins = session.FreeSpinsRemaining
    });
})
.WithName("GetSession");

app.MapPost("/session/{sessionId}/reset", (string sessionId, SessionService sessionService) =>
{
    sessionService.ResetSession(sessionId);
    return Results.Ok(new { Message = "Session reset successfully" });
})
.WithName("ResetSession");

app.MapDefaultEndpoints();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
