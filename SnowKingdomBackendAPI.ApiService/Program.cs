using Microsoft.EntityFrameworkCore;
using SnowKingdomBackendAPI.ApiService.Data;
using SnowKingdomBackendAPI.ApiService.Game;
using SnowKingdomBackendAPI.ApiService.Models;
using SnowKingdomBackendAPI.ApiService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();
builder.Services.AddSingleton<GameConfigService>();
builder.Services.AddSingleton<SessionService>(sp => new SessionService(sp));

// Database configuration for slot game data persistence
// Database is stored at C:\OnlineGameData\onlinegame.db
var dbPath = @"C:\OnlineGameData\onlinegame.db";
var dbDirectory = Path.GetDirectoryName(dbPath);
if (!string.IsNullOrEmpty(dbDirectory) && !Directory.Exists(dbDirectory))
{
    Directory.CreateDirectory(dbDirectory);
}

builder.Services.AddDbContext<GameDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

// Register GameDataService for persistence
builder.Services.AddScoped<GameDataService>();

// Configure JSON options to use camelCase for API responses
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
});

// Add HTTP logging middleware
builder.Services.AddHttpLogging(options =>
{
    options.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.All;
    options.RequestBodyLogLimit = 64 * 1024;
    options.ResponseBodyLogLimit = 64 * 1024;
});

// Add CORS policy to allow frontend communication
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(
                "http://localhost:3000", // Lobby
                "http://localhost:3001", // FrontEnd (Game 1)
                "http://localhost:3002", // FrontEnd5x3 (Game 2)
                "http://localhost:3003", // frontEndDamian (Inferno-Empress)
                "http://localhost:3004", // FrontEndRicky (Reign Of Thunder)
                "https://localhost:3000",
                "https://localhost:3001",
                "https://localhost:3002",
                "https://localhost:3003",
                "https://localhost:3004",
                "http://localhost:9003",
                "https://localhost:9003",
                "http://localhost:5073",
                "https://localhost:5073",
                "http://localhost:59775",
                "https://localhost:59775",
                "http://localhost:52436",
                "https://localhost:52436",
                "http://127.0.0.1:3000",
                "http://127.0.0.1:3001",
                "http://127.0.0.1:3002",
                "http://127.0.0.1:3003",
                "http://127.0.0.1:3004",
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

// Enable HTTP logging
app.UseHttpLogging();

// Enable CORS
app.UseCors();

app.MapPost("/play", async (PlayRequest request, GameConfigService configService, SessionService sessionService, GameDataService dataService) =>
{
    try
    {
        // Get or create session
        var currentState = sessionService.GetOrCreateSession(request.SessionId);

        // Get gameId: prefer from request (RGS route), then from session, then default
        var session = await sessionService.GetSessionAsync(request.SessionId);
        var gameId = request.GameId ?? session?.GameId ?? "SnowKingdom";
        
        // Update session's GameId if it was provided in the request and differs
        if (!string.IsNullOrEmpty(request.GameId) && session != null && session.GameId != request.GameId)
        {
            session.GameId = request.GameId;
            await sessionService.UpdateSessionAsync(session);
        }

        // Load game configuration
        var gameConfig = configService.LoadGameConfig(gameId);

        // Create game engine instance with loaded config
        var gameEngine = new GameEngine(gameConfig);

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
        var freeSpinsAwarded = 0;
        if (spinResult.ScatterWin.TriggeredFreeSpins)
        {
            freeSpinsAwarded = gameConfig.FreeSpinsAwarded;
            newFreeSpins += freeSpinsAwarded;
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

        // Log spin transaction to database
        try
        {
            var isFreeSpin = currentState.FreeSpinsRemaining > 0;
            await dataService.SaveSpinTransactionAsync(
                request.SessionId,
                gameId,
                request.BetAmount,
                spinResult,
                isFreeSpin,
                freeSpinsAwarded);
        }
        catch (Exception ex)
        {
            // Log error but don't fail the request
            // In production, you might want to use a proper logger here
            Console.WriteLine($"[ERROR] Failed to save spin transaction: {ex.Message}");
        }

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

// API endpoints for game data
app.MapGet("/game/sessions/{sessionId}/history", async (string sessionId, GameDataService dataService, int? limit = 50) =>
{
    try
    {
        var history = await dataService.GetSpinHistoryAsync(sessionId, limit);
        return Results.Ok(history);
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error retrieving spin history: {ex.Message}");
    }
})
.WithName("GetSpinHistory");

app.MapGet("/game/sessions/{sessionId}", async (string sessionId, GameDataService dataService) =>
{
    try
    {
        var session = await dataService.GetSessionAsync(sessionId);
        if (session == null)
        {
            return Results.NotFound(new { Error = "Session not found" });
        }
        return Results.Ok(session);
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error retrieving session: {ex.Message}");
    }
})
.WithName("GetSessionDetails");

app.MapGet("/game/stats", async (GameDataService dataService) =>
{
    try
    {
        var stats = await dataService.GetGameStatsAsync();
        return Results.Ok(stats);
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error retrieving game statistics: {ex.Message}");
    }
})
.WithName("GetGameStats");

// Ensure database is created on startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<GameDbContext>();
    await dbContext.Database.EnsureCreatedAsync();
}

app.MapDefaultEndpoints();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
