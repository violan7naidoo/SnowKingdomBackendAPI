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

            // Determine the winning symbol for the line (the first non-WILD symbol)
            var winningSymbol = lineSymbols.FirstOrDefault(s => s != SymbolId.WILD);

            // If the line is all wilds, the winning symbol is WILD itself
            if (winningSymbol == default(SymbolId))
            {
                winningSymbol = SymbolId.WILD;
            }

            // Count consecutive symbols from the left, where WILD substitutes for the winningSymbol
            var count = 0;
            while (count < lineSymbols.Count && (lineSymbols[count] == winningSymbol || lineSymbols[count] == SymbolId.WILD))
            {
                count++;
            }

            // Check for a win based on the count and the determined winning symbol
            if (GameConfiguration.Symbols.TryGetValue(winningSymbol, out var symbolInfo) &&
                symbolInfo.Payout.TryGetValue(count, out var payout))
            {
                var totalPayout = payout * betAmount;
                if (totalPayout > 0)
                {
                    result.TotalWin += totalPayout;
                    result.WinningLines.Add(new WinningLine
                    {
                        PaylineIndex = paylineIndex,
                        Symbol = winningSymbol, // Report the actual symbol that formed the win
                        Count = count,
                        Payout = totalPayout,
                        Line = line.Take(count).ToList()
                    });
                }
            }
        }

        // 2. Evaluate Scatters (No changes needed here)
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
            var scatterPayout = betAmount * (scatterCount switch
            {
                3 => 5,
                4 => 20,
                _ => 50 // 5 or 6 scatters
            });

            result.TotalWin += scatterPayout;

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