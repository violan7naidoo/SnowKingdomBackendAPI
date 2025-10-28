using Microsoft.AspNetCore.Mvc;
using SnowKingdomBackendAPI.RGS.Models;
using SnowKingdomBackendAPI.ApiService.Services;
using SnowKingdomBackendAPI.ApiService.Models;
using System.Text.Json;

namespace SnowKingdomBackendAPI.RGS.Controllers;

[ApiController]
[Route("{operatorId}/{gameId}")]
public class GameController : ControllerBase
{
    private readonly SessionService _sessionService;
    private readonly ILogger<GameController> _logger;

    public GameController(SessionService sessionService, ILogger<GameController> logger)
    {
        _sessionService = sessionService;
        _logger = logger;
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

            // Calculate total bet
            var totalBet = request.Bets.Sum(b => b.Amount);
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

            // For now, we'll simulate the backend call
            // In a real implementation, you'd call the backend API here
            var gameResult = await SimulateBackendCall(backendRequest);

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

    private async Task<SpinResult> SimulateBackendCall(SnowKingdomBackendAPI.ApiService.Models.PlayRequest request)
    {
        // This is a temporary simulation for local development
        // In production, this would call the actual backend game engine
        
        await Task.Delay(100); // Simulate API call delay
        
        // For now, return a simple result
        // You would replace this with an actual HTTP call to your backend
        return new SpinResult
        {
            TotalWin = 0, // Would be calculated by backend
            WinningLines = new List<WinningLine>(),
            ScatterWin = new ScatterWin
            {
                Count = 0,
                TriggeredFreeSpins = false
            },
            Grid = new List<List<string>>()
        };
    }
}
