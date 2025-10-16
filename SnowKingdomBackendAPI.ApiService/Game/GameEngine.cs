using SnowKingdomBackendAPI.ApiService.Models;

namespace SnowKingdomBackendAPI.ApiService.Game;

public class GameEngine
{
    private readonly Random _random = new();

    public SpinResult EvaluateSpin(List<List<string>> grid, int betAmount)
    {
        var result = new SpinResult();
        result.Grid = grid;

        // Convert string symbol names to SymbolId for evaluation
        var symbolGrid = grid.Select(reel =>
            reel.Select(symbolName => GameConfiguration.Symbols.First(kvp => kvp.Value.Name == symbolName).Key).ToList()
        ).ToList();

        // 1. Evaluate Paylines
        for (int paylineIndex = 0; paylineIndex < GameConstants.Paylines.Count; paylineIndex++)
        {
            var line = GameConstants.Paylines[paylineIndex];
            var lineSymbols = line.Select((row, reel) => symbolGrid[reel][row]).ToList();

            var firstSymbol = lineSymbols[0];
            var count = 1;

            // Count consecutive matching symbols
            while (count < lineSymbols.Count && lineSymbols[count] == firstSymbol)
            {
                count++;
            }

            // Check for wild symbols
            if (firstSymbol != SymbolId.WILD)
            {
                // If first symbol is not wild, check for wilds at the start
                var wildCount = 0;
                while (wildCount < lineSymbols.Count && lineSymbols[wildCount] == SymbolId.WILD)
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

        for (int reelIndex = 0; reelIndex < symbolGrid.Count; reelIndex++)
        {
            for (int rowIndex = 0; rowIndex < symbolGrid[reelIndex].Count; rowIndex++)
            {
                if (symbolGrid[reelIndex][rowIndex] == SymbolId.SCATTER)
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
                Symbol = SymbolId.SCATTER,
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

        for (int reelIndex = 0; reelIndex < GameConstants.NumReels; reelIndex++)
        {
            var reel = new List<string>();
            var strip = GameConfiguration.ReelStrips[reelIndex];
            var finalStopIndex = _random.Next(strip.Count);

            for (int rowIndex = 0; rowIndex < GameConstants.NumRows; rowIndex++)
            {
                var symbolIndex = (finalStopIndex + rowIndex) % strip.Count;
                var symbolId = strip[symbolIndex];
                var symbolName = GameConfiguration.Symbols[symbolId].Name;
                reel.Add(symbolName);
            }

            grid.Add(reel);
        }

        return grid;
    }
}
