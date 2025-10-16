using System.Text.Json.Serialization;

namespace SnowKingdomBackendAPI.ApiService.Models;

public enum SymbolId
{
    Wild,
    Scatter,
    Crown,
    Dragon,
    Leopard,
    Queen,
    Stone,
    Wolf,
    Ace,
    Jack,
    QueenCard,
    King,
    Ten
}

public class SymbolConfig
{
    public SymbolId Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Dictionary<int, int> Payout { get; set; } = new();
    public string Image { get; set; } = string.Empty;
}

public class WinningLine
{
    public int PaylineIndex { get; set; }
    public SymbolId Symbol { get; set; }
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
        [1, 1, 1, 1, 1, 1], // Middle-top row
        [2, 2, 2, 2, 2, 2], // Middle-bottom row
        [0, 0, 0, 0, 0, 0], // Top row
        [3, 3, 3, 3, 3, 3], // Bottom row
        [0, 1, 2, 2, 1, 0], // V-shape
        [3, 2, 1, 1, 2, 3], // Inverted V-shape
        [0, 0, 1, 2, 3, 3], // Diagonal up
        [3, 3, 2, 1, 0, 0], // Diagonal down
        [1, 0, 0, 0, 0, 1], // U-shape
    ];
}
