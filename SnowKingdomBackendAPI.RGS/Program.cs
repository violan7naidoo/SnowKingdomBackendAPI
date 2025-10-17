var builder = WebApplication.CreateBuilder(args);

// THIS IS THE FIX (Part 1): Add Aspire service defaults
builder.AddServiceDefaults();

// Adds services for making HTTP calls and using Swagger
builder.Services.AddHttpClient();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configures JSON to use camelCase
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
});

// Allows requests FROM your frontend's stable address
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

// THIS IS THE FIX (Part 2): Map Aspire's default endpoints
app.MapDefaultEndpoints();

app.MapPost("/play", async (PlayRequest request, IHttpClientFactory clientFactory) =>
{
    // This line will now work correctly because of the service defaults
    var httpClient = clientFactory.CreateClient("apiservice");

    var gameEngineResponse = await httpClient.PostAsJsonAsync("/play", request);

    if (!gameEngineResponse.IsSuccessStatusCode)
    {
        // This will now give you a more detailed error if the ApiService fails
        var errorContent = await gameEngineResponse.Content.ReadAsStringAsync();
        Console.WriteLine($"Error from ApiService: {errorContent}");
        return Results.Problem("Error calling the game engine.", statusCode: 500);
    }

    var gameResult = await gameEngineResponse.Content.ReadFromJsonAsync<PlayResponse>();
    return Results.Ok(gameResult);
});

app.Run();

// Defines the data shapes for the API
public record PlayRequest(string SessionId, decimal BetAmount);
public record PlayResponse(List<List<string>> Grid, decimal WinAmount, decimal NewBalance, List<WinningLine> WinningLines, int FreeSpinsLeft, int FreeSpinsWon);
public record WinningLine(int PaylineIndex, string Symbol, int Count, decimal Payout);