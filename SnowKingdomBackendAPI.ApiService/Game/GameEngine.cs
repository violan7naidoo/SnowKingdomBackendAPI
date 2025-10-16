using SnowKingdomBackendAPI.ApiService.Models;

namespace SnowKingdomBackendAPI.ApiService.Game;

public class GameEngine
{
    private readonly Random _random = new();

    public SpinResult EvaluateSpin(List<List<SymbolId>> grid, int betAmount)
    {
        var result = new SpinResult();
        result.Grid = grid;

        // 1. Evaluate Paylines
        for (int paylineIndex = 0; paylineIndex < GameConstants.Paylines.Count; paylineIndex++)
        {
            var line = GameConstants.Paylines[paylineIndex];
            var lineSymbols = line.Select((row, reel) => grid[reel][row]).ToList();

            var firstSymbol = lineSymbols[0];
            var count = 1;

            // Count consecutive matching symbols
            while (count < lineSymbols.Count && lineSymbols[count] == firstSymbol)
            {
                count++;
            }

            // Check for wild symbols
            if (firstSymbol != SymbolId.Wild)
            {
                // If first symbol is not wild, check for wilds at the start
                var wildCount = 0;
                while (wildCount < lineSymbols.Count && lineSymbols[wildCount] == SymbolId.Wild)
                {
                    wildCount++;
                }

                if (wildCount > 0)
                {
                    // If we have leading wilds, they can substitute for the first symbol
                    count = Math.Max(count, wildCount);
                }
            }

            if (GameConfiguration.Symbols.TryGetValue(firstSymbol, out var symbolInfo) &&
                symbolInfo.Payout.TryGetValue(count, out var payout))
            {
                var totalPayout = payout * betAmount;
                if (totalPayout > 0)
                {
                    result.TotalWin += totalPayout;
                    result.WinningLines.Add(new WinningLine
                    {
                        PaylineIndex = paylineIndex,
                        Symbol = firstSymbol,
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
                if (grid[reelIndex][rowIndex] == SymbolId.Scatter)
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
            // Add scatter payout (example: 3x, 4x, or 5x bet amount)
            var scatterPayout = betAmount * (scatterCount switch
            {
                3 => 5,
                4 => 20,
                _ => 50 // 5 or 6 scatters
            });

            result.TotalWin += scatterPayout;

            // Add scatter symbols to winning lines for visual feedback
            result.WinningLines.Add(new WinningLine
            {
                PaylineIndex = -1, // Special index for scatters
                Symbol = SymbolId.Scatter,
                Count = scatterCount,
                Payout = scatterPayout,
                Line = scatterPositions.Select(p => p.row).ToList()
            });
        }

        return result;
    }

    public List<List<SymbolId>> GenerateGrid()
    {
        var grid = new List<List<SymbolId>>();

        for (int reelIndex = 0; reelIndex < GameConstants.NumReels; reelIndex++)
        {
            var reel = new List<SymbolId>();
            var strip = GameConfiguration.ReelStrips[reelIndex];
            var finalStopIndex = _random.Next(strip.Count);

            for (int rowIndex = 0; rowIndex < GameConstants.NumRows; rowIndex++)
            {
                var symbolIndex = (finalStopIndex + rowIndex) % strip.Count;
                reel.Add(strip[symbolIndex]);
            }

            grid.Add(reel);
        }

        return grid;
    }
}
