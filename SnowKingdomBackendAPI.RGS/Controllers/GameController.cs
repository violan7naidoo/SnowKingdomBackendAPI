using Microsoft.AspNetCore.Mvc;
using SnowKingdomBackendAPI.RGS.Models;
using SnowKingdomBackendAPI.ApiService.Services;
using SnowKingdomBackendAPI.ApiService.Models;
using System.Text.Json;
using System.Text;

namespace SnowKingdomBackendAPI.RGS.Controllers;

[ApiController]
[Route("{operatorId}/{gameId}")]
public class GameController : ControllerBase
{
    private readonly SessionService _sessionService;
    private readonly ILogger<GameController> _logger;
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public GameController(SessionService sessionService, ILogger<GameController> logger, HttpClient httpClient, IConfiguration configuration)
    {
        _sessionService = sessionService;
        _logger = logger;
        _httpClient = httpClient;
        _configuration = configuration;
    }

    [HttpPost("start")]
    public async Task<ActionResult<GameStartResponse>> StartGame(
        [FromRoute] string operatorId,
        [FromRoute] string gameId,
        [FromBody] GameStartRequest request)
    {
        try
        {
            _logger.LogInformation($"Game start requested for operator: {operatorId}, game: {gameId}");

            // Generate a new session ID
            var sessionId = Guid.NewGuid().ToString("N");
            var playerId = $"Player-{DateTime.Now.Ticks}";

            // Create session
            var session = new GameSession
            {
                SessionId = sessionId,
                PlayerId = playerId,
                OperatorId = operatorId,
                GameId = gameId,
                Balance = 1000.00m, // Default demo balance
                FreeSpinsRemaining = 0,
                LastWin = 0,
                CreatedAt = DateTime.UtcNow
            };

            await _sessionService.CreateSessionAsync(session);

            var response = new GameStartResponse
            {
                Player = new PlayerInfo
                {
                    SessionId = sessionId,
                    Id = playerId,
                    Balance = session.Balance
                },
                Client = new ClientInfo
                {
                    Type = request.Client,
                    Ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1",
                    Country = new CountryInfo
                    {
                        Code = "ZA", // South Africa for local development
                        Name = "South Africa"
                    }
                },
                Currency = new CurrencyInfo
                {
                    Symbol = "R",
                    IsoCode = "ZAR",
                    Name = "South African Rand",
                    Decimals = 2,
                    Separator = new SeparatorInfo
                    {
                        Decimal = ".",
                        Thousand = ","
                    }
                },
                Game = new GameInfo
                {
                    Rtp = 96.00m,
                    Mode = 0,
                    Bet = new BetConfigInfo
                    {
                        Default = 1,
                        Levels = new List<decimal> { 1, 2, 3, 5 }
                    },
                    FunMode = request.FunMode == 1,
                    MaxWinCap = 0,
                    Config = new GameConfigInfo
                    {
                        StartScreen = null,
                        Settings = new GameSettingsInfo
                        {
                            IsAutoplay = "1",
                            IsSlamStop = "1",
                            IsBuyFeatures = "0",
                            IsTurboSpin = "1",
                            IsRealityCheck = "1",
                            MinSpin = "0",
                            MaxSpin = "0"
                        }
                    },
                    FreeSpins = new FreeSpinsInfo(),
                    PromoFreeSpins = new PromoFreeSpinsInfo(),
                    LastPlay = new LastPlayInfo
                    {
                        BetLevel = new BetLevelInfo { Index = 0, Value = 1 },
                        Results = new List<object>()
                    }
                }
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting game");
            return StatusCode(500, new GameStartResponse
            {
                StatusCode = 6001,
                Message = "Internal server error"
            });
        }
    }

    [HttpPost("play")]
    public async Task<ActionResult<GamePlayResponse>> PlayGame(
        [FromRoute] string operatorId,
        [FromRoute] string gameId,
        [FromBody] GamePlayRequest request)
    {
        try
        {
            _logger.LogInformation($"Game play requested for session: {request.SessionId}");

            // Validate request
            if (string.IsNullOrEmpty(request.SessionId))
            {
                return BadRequest(new GamePlayResponse
                {
                    StatusCode = 6002,
                    Message = "Session ID is required"
                });
            }

            if (request.Bets == null || !request.Bets.Any())
            {
                return BadRequest(new GamePlayResponse
                {
                    StatusCode = 6002,
                    Message = "At least one bet is required"
                });
            }

            // Validate bet amounts
            if (request.Bets.Any(b => b.Amount <= 0))
            {
                return BadRequest(new GamePlayResponse
                {
                    StatusCode = 6002,
                    Message = "Bet amounts must be greater than zero"
                });
            }

            // Validate mode
            if (request.Mode < 0 || request.Mode > 3)
            {
                return BadRequest(new GamePlayResponse
                {
                    StatusCode = 6002,
                    Message = "Invalid game mode. Must be 0-3"
                });
            }

            // Get session
            var session = await _sessionService.GetSessionAsync(request.SessionId);
            if (session == null)
            {
                return BadRequest(new GamePlayResponse
                {
                    StatusCode = 6001,
                    Message = "Invalid session"
                });
            }

            // Calculate total bet (use provided bet or calculate from bets array)
            var totalBet = request.Bet > 0 ? request.Bet : request.Bets.Sum(b => b.Amount);
            var roundId = Guid.NewGuid().ToString("N");
            var withdrawTransactionId = Guid.NewGuid().ToString("N");
            var depositTransactionId = Guid.NewGuid().ToString("N");

            // Store previous balance
            var prevBalance = session.Balance;

            // Deduct bet from balance
            session.Balance -= totalBet;

            // Call backend game engine
            var backendRequest = new SnowKingdomBackendAPI.ApiService.Models.PlayRequest
            {
                SessionId = request.SessionId,
                BetAmount = (int)totalBet,
                LastResponse = session.LastResponse
            };

            // Call the actual backend API
            var gameResult = await CallBackendEngine(backendRequest);

            // Update session with results
            session.Balance += gameResult.TotalWin; // int implicitly converts to decimal
            session.LastWin = gameResult.TotalWin; // int implicitly converts to decimal
            session.LastResponse = new GameState
            {
                Balance = (int)session.Balance, // decimal to int conversion
                FreeSpinsRemaining = session.FreeSpinsRemaining,
                LastWin = (int)session.LastWin, // decimal to int conversion
                Results = gameResult
            };

            // Handle free spins
            if (gameResult.ScatterWin.TriggeredFreeSpins)
            {
                session.FreeSpinsRemaining += GameConstants.FreeSpinsAwarded;
            }

            // Decrement free spins if in free spins mode
            if (session.FreeSpinsRemaining > 0)
            {
                session.FreeSpinsRemaining--;
            }

            await _sessionService.UpdateSessionAsync(session);

            var response = new GamePlayResponse
            {
                Player = new PlayerPlayInfo
                {
                    SessionId = request.SessionId,
                    RoundId = roundId,
                    Transaction = new TransactionInfo
                    {
                        Withdraw = withdrawTransactionId,
                        Deposit = depositTransactionId
                    },
                    PrevBalance = prevBalance,
                    Balance = session.Balance,
                    Bet = totalBet,
                    Win = gameResult.TotalWin,
                    CurrencyId = "ZAR"
                },
                Game = new GamePlayInfo
                {
                    Results = gameResult,
                    Mode = session.FreeSpinsRemaining > 0 ? 1 : 0, // 1 = free spins mode
                    MaxWinCap = new MaxWinCapInfo
                    {
                        Achieved = false,
                        Value = 0,
                        RealWin = gameResult.TotalWin
                    }
                },
                FreeSpins = new FreeSpinsInfo
                {
                    Amount = gameResult.ScatterWin.TriggeredFreeSpins ? GameConstants.FreeSpinsAwarded : 0,
                    Left = session.FreeSpinsRemaining,
                    BetValue = totalBet,
                    RoundWin = gameResult.TotalWin,
                    TotalWin = 0, // Would need to track total free spins wins
                    TotalBet = totalBet,
                    Won = gameResult.ScatterWin.TriggeredFreeSpins ? GameConstants.FreeSpinsAwarded : 0,
                    IsPromotion = false
                },
                PromoFreeSpins = new PromoFreeSpinsInfo(),
                Jackpots = new List<object>(),
                Feature = new FeatureInfo
                {
                    Name = "",
                    Type = "",
                    IsClosure = 0
                }
            };

            return Ok(response);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Backend communication error");
            return StatusCode(503, new GamePlayResponse
            {
                StatusCode = 6003,
                Message = "Backend service unavailable"
            });
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON serialization/deserialization error");
            return StatusCode(400, new GamePlayResponse
            {
                StatusCode = 6004,
                Message = "Invalid data format"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error playing game");
            return StatusCode(500, new GamePlayResponse
            {
                StatusCode = 6001,
                Message = "Internal server error"
            });
        }
    }

    private async Task<SpinResult> CallBackendEngine(SnowKingdomBackendAPI.ApiService.Models.PlayRequest request)
    {
        try
        {
            // Get backend URL from configuration
            var backendUrl = _configuration["apiservice:url"] ?? "http://localhost:5001/play";
            
            _logger.LogInformation($"Calling backend at: {backendUrl}");
            
            // Serialize request to JSON
            var jsonContent = JsonSerializer.Serialize(request, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            
            // Make HTTP call to backend
            var response = await _httpClient.PostAsync(backendUrl, content);
            
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"Backend response: {responseContent}");
                
                // Deserialize response
                var playResponse = JsonSerializer.Deserialize<PlayResponse>(responseContent, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
                
                if (playResponse?.Game?.Results != null)
                {
                    return playResponse.Game.Results;
                }
            }
            
            _logger.LogError($"Backend call failed: {response.StatusCode} - {response.ReasonPhrase}");
            throw new HttpRequestException($"Backend call failed: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling backend engine");
            throw;
        }
    }
}
