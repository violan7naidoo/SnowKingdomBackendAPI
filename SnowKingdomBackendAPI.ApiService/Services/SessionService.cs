using SnowKingdomBackendAPI.ApiService.Models;

namespace SnowKingdomBackendAPI.ApiService.Services;

public class SessionService
{
    private readonly Dictionary<string, GameSession> _sessions = new();
    private readonly Dictionary<string, List<GameSession>> _playerSessions = new();
    private const int DefaultBalance = 1000;

    // Legacy methods for backward compatibility
    public GameState GetOrCreateSession(string sessionId)
    {
        if (string.IsNullOrEmpty(sessionId))
        {
            sessionId = Guid.NewGuid().ToString();
        }

        if (!_sessions.TryGetValue(sessionId, out var session))
        {
            session = new GameSession
            {
                SessionId = sessionId,
                PlayerId = $"Player-{DateTime.Now.Ticks}",
                OperatorId = "LOCAL",
                GameId = "FROSTY_FORTUNES",
                Balance = DefaultBalance,
                FreeSpinsRemaining = 0,
                LastWin = 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _sessions[sessionId] = session;
        }

        return new GameState
        {
            Balance = (int)session.Balance,
            FreeSpinsRemaining = session.FreeSpinsRemaining,
            LastWin = (int)session.LastWin,
            Results = session.LastResponse?.Results ?? new SpinResult()
        };
    }

    public GameState UpdateSession(string sessionId, GameState newState)
    {
        if (_sessions.TryGetValue(sessionId, out var session))
        {
            session.Balance = newState.Balance; // Implicit conversion from int to decimal
            session.FreeSpinsRemaining = newState.FreeSpinsRemaining;
            session.LastWin = newState.LastWin; // Implicit conversion from int to decimal
            session.LastResponse = newState;
            session.UpdatedAt = DateTime.UtcNow;
        }
        return newState;
    }

    public void ResetSession(string sessionId)
    {
        if (_sessions.ContainsKey(sessionId))
        {
            _sessions[sessionId] = new GameSession
            {
                SessionId = sessionId,
                PlayerId = $"Player-{DateTime.Now.Ticks}",
                OperatorId = "LOCAL",
                GameId = "FROSTY_FORTUNES",
                Balance = DefaultBalance,
                FreeSpinsRemaining = 0,
                LastWin = 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }
    }

    // New RGS-compatible methods
    public async Task<GameSession?> GetSessionAsync(string sessionId)
    {
        await Task.CompletedTask; // For async compatibility
        return _sessions.TryGetValue(sessionId, out var session) ? session : null;
    }

    public async Task CreateSessionAsync(GameSession session)
    {
        await Task.CompletedTask; // For async compatibility
        _sessions[session.SessionId] = session;
        
        if (!_playerSessions.ContainsKey(session.PlayerId))
        {
            _playerSessions[session.PlayerId] = new List<GameSession>();
        }
        _playerSessions[session.PlayerId].Add(session);
    }

    public async Task UpdateSessionAsync(GameSession session)
    {
        await Task.CompletedTask; // For async compatibility
        session.UpdatedAt = DateTime.UtcNow;
        _sessions[session.SessionId] = session;
    }

    public async Task<List<GameSession>> GetSessionsByPlayerIdAsync(string playerId)
    {
        await Task.CompletedTask; // For async compatibility
        return _playerSessions.TryGetValue(playerId, out var sessions) ? sessions : new List<GameSession>();
    }
}
