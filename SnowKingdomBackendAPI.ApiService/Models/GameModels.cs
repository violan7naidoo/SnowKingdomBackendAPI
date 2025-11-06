using System.Text.Json.Serialization;

namespace SnowKingdomBackendAPI.ApiService.Models;

public class SymbolConfig
{
    public string Name { get; set; } = string.Empty;
    public Dictionary<int, int> Payout { get; set; } = new();
    public string Image { get; set; } = string.Empty;
}

public class WinningLine
{
    public int PaylineIndex { get; set; }
    public string Symbol { get; set; } = string.Empty;
    public int Count { get; set; }
    public int Payout { get; set; }
    public List<int> Line { get; set; } = new();
}

public class ScatterWin
{
    public int Count { get; set; }
    public bool TriggeredFreeSpins { get; set; }
}

public class SpinResult
{
    public int TotalWin { get; set; }
    public List<WinningLine> WinningLines { get; set; } = new();
    public ScatterWin ScatterWin { get; set; } = new();
    public List<List<string>> Grid { get; set; } = new();
}

public class PlayRequest
{
    public string SessionId { get; set; } = string.Empty;
    public int BetAmount { get; set; }
    public GameState? LastResponse { get; set; }
    public string? GameId { get; set; } // Optional: allows RGS to specify which game config to use
}

public class GameState
{
    public int Balance { get; set; }
    public int FreeSpinsRemaining { get; set; }
    public int LastWin { get; set; }
    public SpinResult Results { get; set; } = new();
}

public class PlayResponse
{
    public string SessionId { get; set; } = string.Empty;
    public GameState Player { get; set; } = new();
    public GameState Game { get; set; } = new();
    public int FreeSpins { get; set; }
}

public static class GameConstants
{
    public const int NumReels = 6;
    public const int NumRows = 4;
    public static readonly int[] BetAmounts = [1, 2, 3, 5];
    public const int FreeSpinsAwarded = 10;

    public static readonly List<List<int>> Paylines =
[
    [0, 0, 0, 0, 0, 0], // Line 1: Top row (was Line 3)
    [1, 1, 1, 1, 1, 1], // Line 2: Second row (was Line 1)
    [2, 2, 2, 2, 2, 2], // Line 3: Third row (was Line 2)
    [3, 3, 3, 3, 3, 3], // Line 4: Bottom row (unchanged)
    [1, 0, 0, 0, 0, 1], // Line 5 (unchanged)
    [2, 3, 3, 3, 3, 2], // Line 6 (unchanged)
    [2, 1, 2, 1, 2, 1], // Line 7 (unchanged)
    [1, 2, 1, 2, 1, 2], // Line 8 (unchanged)
    [0, 1, 0, 1, 0, 1], // Line 9 (unchanged)
    [3, 2, 3, 2, 3, 2]  // Line 10 (unchanged)
];
}
