var builder = WebApplication.CreateBuilder(args);

// Adds the ability for the RGS to make HTTP calls to other services
builder.Services.AddHttpClient();

// Configures JSON to use camelCase to match the frontend's expectations
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
});

// Adds CORS policy to allow the frontend to make requests to this RGS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        // IMPORTANT: Replace with your frontend's URL from the Aspire Dashboard
        policy.WithOrigins("http://localhost:56656")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();
app.UseCors();

// This is the new endpoint that your FRONTEND will call
app.MapPost("/play", async (PlayRequest request, IHttpClientFactory clientFactory) =>
{
    // 1. Create a client to talk to the Game Engine ("apiservice")
    var httpClient = clientFactory.CreateClient("apiservice");

    // 2. Forward the request to the Game Engine's /play endpoint
    var gameEngineResponse = await httpClient.PostAsJsonAsync("/play", request);

    if (!gameEngineResponse.IsSuccessStatusCode)
    {
        return Results.Problem("Error calling the game engine.");
    }

    // 3. Read the result from the Game Engine
    var gameResult = await gameEngineResponse.Content.ReadFromJsonAsync<PlayResponse>();

    // 4. In a real RGS, you would add logic here:
    //    - Validate the player's session token
    //    - Perform database transactions for the bet and win
    //    - Add operator-specific data to the response

    // For our dummy RGS, we just return the game result directly
    return Results.Ok(gameResult);
});

app.Run();

// These record types define the data structure for the /play endpoint
public record PlayRequest(string SessionId, decimal BetAmount);
public record PlayResponse(
    List<List<string>> Grid,
    decimal WinAmount,
    decimal NewBalance,
    List<WinningLine> WinningLines,
    int FreeSpinsLeft,
    int FreeSpinsWon
);
public record WinningLine(int PaylineIndex, string Symbol, int Count, decimal Payout);