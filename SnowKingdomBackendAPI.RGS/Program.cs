var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add a typed HttpClient to communicate with the backend ApiService.
builder.Services.AddHttpClient<GameApiClient>(client =>
{
    client.BaseAddress = new("http://snowkingdombackendapi.apiservice");
});

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

// This endpoint will be called by the frontend and will forward the request to the backend.
app.MapPost("/play", async (PlayRequest request, GameApiClient gameApiClient) =>
{
    try
    {
        var response = await gameApiClient.PlayAsync(request);
        return Results.Ok(response);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error forwarding request to ApiService: {ex.Message}");
        return Results.Problem("Failed to connect to the game service.", statusCode: 503);
    }
});

app.Run();

public record PlayRequest(string SessionId, int BetAmount);

public class GameApiClient
{
    private readonly HttpClient _httpClient;

    public GameApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<object?> PlayAsync(PlayRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("/play", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<object>();
    }
}