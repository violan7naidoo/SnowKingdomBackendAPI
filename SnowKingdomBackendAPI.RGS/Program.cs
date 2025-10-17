var builder = WebApplication.CreateBuilder(args);

// Adds services for making HTTP calls and using Swagger
builder.Services.AddHttpClient();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configures JSON to use camelCase to match the frontend
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
});

// THIS IS THE FIX: Allow requests FROM the frontend's stable address.
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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// CRUCIAL: app.UseCors() must be called here.
app.UseCors();

app.MapPost("/play", async (PlayRequest request, IHttpClientFactory clientFactory) =>
{
    var httpClient = clientFactory.CreateClient("apiservice");
    var gameEngineResponse = await httpClient.PostAsJsonAsync("/play", request);

    if (!gameEngineResponse.IsSuccessStatusCode)
    {
        return Results.Problem("Error calling the game engine.");
    }

    var gameResult = await gameEngineResponse.Content.ReadFromJsonAsync<PlayResponse>();
    return Results.Ok(gameResult);
});

app.Run();

// Defines the data shapes for the API
public record PlayRequest(string SessionId, decimal BetAmount);
public record PlayResponse(List<List<string>> Grid, decimal WinAmount, decimal NewBalance, List<WinningLine> WinningLines, int FreeSpinsLeft, int FreeSpinsWon);
public record WinningLine(int PaylineIndex, string Symbol, int Count, decimal Payout);