using SnowKingdomBackendAPI.ApiService.Models;

namespace SnowKingdomBackendAPI.ApiService.Game;

public class GameEngine
{
    private readonly Random _random = new();
    private readonly GameConfig _config;

    public GameEngine(GameConfig config)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
    }

    public SpinResult EvaluateSpin(List<List<string>> grid, int betAmount)
    {
        var result = new SpinResult();
        result.Grid = grid;

        // 1. Evaluate Paylines
        for (int paylineIndex = 0; paylineIndex < _config.Paylines.Count; paylineIndex++)
        {
            var line = _config.Paylines[paylineIndex];
            var lineSymbols = line.Select((row, reel) => grid[reel][row]).ToList();

            // Determine the winning symbol for the line (the first non-WILD symbol)
            var winningSymbol = lineSymbols.FirstOrDefault(s => s != _config.WildSymbol);

            // If the line is all wilds, the winning symbol is WILD itself
            if (winningSymbol == null)
            {
                winningSymbol = _config.WildSymbol;
            }

            // Count consecutive symbols from the left, where WILD substitutes for the winningSymbol
            var count = 0;
            while (count < lineSymbols.Count && 
                   (lineSymbols[count] == winningSymbol || lineSymbols[count] == _config.WildSymbol))
            {
                count++;
            }

            // Check for a win based on the count and the determined winning symbol
            if (_config.Symbols.TryGetValue(winningSymbol, out var symbolInfo) &&
                symbolInfo.Payout.TryGetValue(count, out var payout))
            {
                var totalPayout = payout * betAmount;
                if (totalPayout > 0)
                {
                    result.TotalWin += totalPayout;
                    result.WinningLines.Add(new WinningLine
                    {
                        PaylineIndex = paylineIndex,
                        Symbol = winningSymbol,
                        Count = count,
                        Payout = totalPayout,
                        Line = line.Take(count).ToList()
                    });
                }
            }
        }

        // 2. Evaluate Scatters
        var scatterCount = 0;
        var scatterPositions = new List<(int reel, int row)>();

        for (int reelIndex = 0; reelIndex < grid.Count; reelIndex++)
        {
            for (int rowIndex = 0; rowIndex < grid[reelIndex].Count; rowIndex++)
            {
                if (grid[reelIndex][rowIndex] == _config.ScatterSymbol)
                {
                    scatterCount++;
                    scatterPositions.Add((reelIndex, rowIndex));
                }
            }
        }

        result.ScatterWin.Count = scatterCount;
        result.ScatterWin.TriggeredFreeSpins = scatterCount >= 3;

        if (scatterCount >= 3)
        {
            // Use scatter payout from config, with fallback for 5+ scatters
            var scatterMultiplier = _config.ScatterPayout.TryGetValue(scatterCount, out var multiplier) 
                ? multiplier 
                : _config.ScatterPayout.Values.Max();

            var scatterPayout = betAmount * scatterMultiplier;
            result.TotalWin += scatterPayout;

            result.WinningLines.Add(new WinningLine
            {
                PaylineIndex = -1, // Special index for scatters
                Symbol = _config.ScatterSymbol,
                Count = scatterCount,
                Payout = scatterPayout,
                Line = scatterPositions.Select(p => p.row).ToList()
            });
        }

        return result;
    }

    public List<List<string>> GenerateGrid()
    {
        var grid = new List<List<string>>();

        for (int reelIndex = 0; reelIndex < _config.NumReels; reelIndex++)
        {
            var reel = new List<string>();
            var strip = _config.ReelStrips[reelIndex];
            var finalStopIndex = _random.Next(strip.Count);

            for (int rowIndex = 0; rowIndex < _config.NumRows; rowIndex++)
            {
                var symbolIndex = (finalStopIndex + rowIndex) % strip.Count;
                var symbolName = strip[symbolIndex];
                reel.Add(symbolName);
            }

            grid.Add(reel);
        }

        return grid;
    }
}
