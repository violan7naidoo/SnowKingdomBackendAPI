using SnowKingdomBackendAPI.ApiService.Models;

namespace SnowKingdomBackendAPI.ApiService.Game;

public class GameConfig
{
    public string GameName { get; set; } = string.Empty;
    public int NumReels { get; set; }
    public int NumRows { get; set; }
    public Dictionary<string, SymbolConfig> Symbols { get; set; } = new();
    public string WildSymbol { get; set; } = "WILD";
    public string ScatterSymbol { get; set; } = "SCATTER";
    public List<List<string>> ReelStrips { get; set; } = new();
    public List<List<int>> Paylines { get; set; } = new();
    public Dictionary<int, int> ScatterPayout { get; set; } = new();
    public int FreeSpinsAwarded { get; set; }
    public int[] BetAmounts { get; set; } = Array.Empty<int>();
}

