using SnowKingdomBackendAPI.ApiService.Models;

namespace SnowKingdomBackendAPI.ApiService.Services;

public class SessionService
{
    private readonly Dictionary<string, GameState> _sessions = new();
    private const int DefaultBalance = 1000;

    public GameState GetOrCreateSession(string sessionId)
    {
        if (string.IsNullOrEmpty(sessionId))
        {
            sessionId = Guid.NewGuid().ToString();
        }

        if (!_sessions.TryGetValue(sessionId, out var session))
        {
            session = new GameState
            {
                Balance = DefaultBalance,
                FreeSpinsRemaining = 0,
                LastWin = 0
            };
            _sessions[sessionId] = session;
        }

        return session;
    }

    public GameState UpdateSession(string sessionId, GameState newState)
    {
        _sessions[sessionId] = newState;
        return newState;
    }

    public void ResetSession(string sessionId)
    {
        if (_sessions.ContainsKey(sessionId))
        {
            _sessions[sessionId] = new GameState
            {
                Balance = DefaultBalance,
                FreeSpinsRemaining = 0,
                LastWin = 0
            };
        }
    }
}
