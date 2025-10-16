using SnowKingdomBackendAPI.ApiService.Models;

namespace SnowKingdomBackendAPI.ApiService.Game;

public static class GameConfiguration
{
    public static readonly Dictionary<SymbolId, SymbolConfig> Symbols = new()
    {
        [SymbolId.Wild] = new SymbolConfig
        {
            Id = SymbolId.Wild,
            Name = "Wild",
            Payout = new Dictionary<int, int> { { 2, 5 }, { 3, 20 }, { 4, 100 }, { 5, 500 }, { 6, 2000 } },
            Image = "/images/symbols/Wild.png"
        },
        [SymbolId.Scatter] = new SymbolConfig
        {
            Id = SymbolId.Scatter,
            Name = "Scatter",
            Payout = new Dictionary<int, int> { { 3, 5 }, { 4, 30 }, { 5, 150 }, { 6, 1000 } },
            Image = "/images/symbols/Scatter.png"
        },
        [SymbolId.Crown] = new SymbolConfig
        {
            Id = SymbolId.Crown,
            Name = "Crown",
            Payout = new Dictionary<int, int> { { 3, 10 }, { 4, 60 }, { 5, 100 }, { 6, 150 } },
            Image = "/images/symbols/Crown.png"
        },
        [SymbolId.Dragon] = new SymbolConfig
        {
            Id = SymbolId.Dragon,
            Name = "Dragon",
            Payout = new Dictionary<int, int> { { 3, 10 }, { 4, 30 }, { 5, 85 }, { 6, 120 } },
            Image = "/images/symbols/Dragon.png"
        },
        [SymbolId.Leopard] = new SymbolConfig
        {
            Id = SymbolId.Leopard,
            Name = "Leopard",
            Payout = new Dictionary<int, int> { { 3, 5 }, { 4, 30 }, { 5, 85 }, { 6, 120 } },
            Image = "/images/symbols/Leopard.png"
        },
        [SymbolId.Queen] = new SymbolConfig
        {
            Id = SymbolId.Queen,
            Name = "Queen",
            Payout = new Dictionary<int, int> { { 3, 5 }, { 4, 20 }, { 5, 60 }, { 6, 100 } },
            Image = "/images/symbols/Queen.png"
        },
        [SymbolId.Stone] = new SymbolConfig
        {
            Id = SymbolId.Stone,
            Name = "Stone",
            Payout = new Dictionary<int, int> { { 3, 5 }, { 4, 20 }, { 5, 60 }, { 6, 100 } },
            Image = "/images/symbols/Stone.png"
        },
        [SymbolId.Wolf] = new SymbolConfig
        {
            Id = SymbolId.Wolf,
            Name = "Wolf",
            Payout = new Dictionary<int, int> { { 3, 5 }, { 4, 20 }, { 5, 60 }, { 6, 100 } },
            Image = "/images/symbols/Wolf.png"
        },
        [SymbolId.Ace] = new SymbolConfig
        {
            Id = SymbolId.Ace,
            Name = "Ace",
            Payout = new Dictionary<int, int> { { 3, 2 }, { 4, 5 }, { 5, 20 }, { 6, 40 } },
            Image = "/images/symbols/A.png"
        },
        [SymbolId.Jack] = new SymbolConfig
        {
            Id = SymbolId.Jack,
            Name = "Jack",
            Payout = new Dictionary<int, int> { { 3, 2 }, { 4, 5 }, { 5, 20 }, { 6, 40 } },
            Image = "/images/symbols/J.png"
        },
        [SymbolId.QueenCard] = new SymbolConfig
        {
            Id = SymbolId.QueenCard,
            Name = "Queen Card",
            Payout = new Dictionary<int, int> { { 3, 2 }, { 4, 5 }, { 5, 20 }, { 6, 40 } },
            Image = "/images/symbols/Q.png"
        },
        [SymbolId.King] = new SymbolConfig
        {
            Id = SymbolId.King,
            Name = "King",
            Payout = new Dictionary<int, int> { { 3, 2 }, { 4, 5 }, { 5, 20 }, { 6, 40 } },
            Image = "/images/symbols/K.png"
        },
        [SymbolId.Ten] = new SymbolConfig
        {
            Id = SymbolId.Ten,
            Name = "Ten",
            Payout = new Dictionary<int, int> { { 3, 2 }, { 4, 5 }, { 5, 20 }, { 6, 40 } },
            Image = "/images/symbols/10.png"
        }
    };

    public static readonly List<List<SymbolId>> ReelStrips =
    [
        // Reel 1 (34 symbols)
        [
            SymbolId.King, SymbolId.Crown, SymbolId.QueenCard, SymbolId.Ten, SymbolId.Ace,
            SymbolId.Wolf, SymbolId.Stone, SymbolId.Queen, SymbolId.Ten, SymbolId.Jack,
            SymbolId.QueenCard, SymbolId.King, SymbolId.Ace, SymbolId.Leopard, SymbolId.Dragon,
            SymbolId.Jack, SymbolId.Ten, SymbolId.Jack, SymbolId.QueenCard, SymbolId.King,
            SymbolId.Ace, SymbolId.Wolf, SymbolId.Stone, SymbolId.Ten, SymbolId.Jack,
            SymbolId.QueenCard, SymbolId.King, SymbolId.Ace, SymbolId.Wild, SymbolId.Ten,
            SymbolId.Jack, SymbolId.QueenCard, SymbolId.King, SymbolId.Scatter
        ],
        // Reel 2 (34 symbols)
        [
            SymbolId.Ten, SymbolId.Stone, SymbolId.Queen, SymbolId.King, SymbolId.Ace,
            SymbolId.Wolf, SymbolId.Stone, SymbolId.QueenCard, SymbolId.Ten, SymbolId.Jack,
            SymbolId.QueenCard, SymbolId.King, SymbolId.Ace, SymbolId.Leopard, SymbolId.Dragon,
            SymbolId.Crown, SymbolId.Ten, SymbolId.Jack, SymbolId.QueenCard, SymbolId.King,
            SymbolId.Ace, SymbolId.Wolf, SymbolId.Jack, SymbolId.Ten, SymbolId.Jack,
            SymbolId.QueenCard, SymbolId.King, SymbolId.Ace, SymbolId.Wild, SymbolId.Ten,
            SymbolId.Jack, SymbolId.QueenCard, SymbolId.King, SymbolId.Scatter
        ],
        // Reel 3 (34 symbols)
        [
            SymbolId.Wild, SymbolId.Ace, SymbolId.Wolf, SymbolId.Stone, SymbolId.Ace,
            SymbolId.QueenCard, SymbolId.Stone, SymbolId.Queen, SymbolId.Ten, SymbolId.Jack,
            SymbolId.QueenCard, SymbolId.King, SymbolId.Ace, SymbolId.Leopard, SymbolId.Dragon,
            SymbolId.Crown, SymbolId.Ten, SymbolId.Jack, SymbolId.QueenCard, SymbolId.King,
            SymbolId.Ace, SymbolId.Wolf, SymbolId.King, SymbolId.Ten, SymbolId.Jack,
            SymbolId.QueenCard, SymbolId.King, SymbolId.Jack, SymbolId.Wild, SymbolId.Ten,
            SymbolId.Jack, SymbolId.QueenCard, SymbolId.King, SymbolId.Scatter
        ],
        // Reel 4 (34 symbols)
        [
            SymbolId.Ten, SymbolId.Jack, SymbolId.QueenCard, SymbolId.King, SymbolId.Ace,
            SymbolId.Wolf, SymbolId.Stone, SymbolId.Queen, SymbolId.Ten, SymbolId.Jack,
            SymbolId.QueenCard, SymbolId.King, SymbolId.Ace, SymbolId.Leopard, SymbolId.Dragon,
            SymbolId.Crown, SymbolId.Ten, SymbolId.Jack, SymbolId.QueenCard, SymbolId.King,
            SymbolId.Ace, SymbolId.Wolf, SymbolId.Stone, SymbolId.Ten, SymbolId.Jack,
            SymbolId.QueenCard, SymbolId.King, SymbolId.Ace, SymbolId.Wild, SymbolId.Ten,
            SymbolId.Jack, SymbolId.QueenCard, SymbolId.King, SymbolId.Scatter
        ],
        // Reel 5 (34 symbols)
        [
            SymbolId.Dragon, SymbolId.Wild, SymbolId.Leopard, SymbolId.Jack, SymbolId.Ace,
            SymbolId.Wolf, SymbolId.Stone, SymbolId.Queen, SymbolId.Ten, SymbolId.Jack,
            SymbolId.QueenCard, SymbolId.King, SymbolId.Ace, SymbolId.QueenCard, SymbolId.Ten,
            SymbolId.Crown, SymbolId.Ten, SymbolId.Jack, SymbolId.QueenCard, SymbolId.King,
            SymbolId.Ace, SymbolId.Wolf, SymbolId.Stone, SymbolId.Ten, SymbolId.Jack,
            SymbolId.QueenCard, SymbolId.King, SymbolId.Ace, SymbolId.King, SymbolId.Ten,
            SymbolId.Jack, SymbolId.QueenCard, SymbolId.King, SymbolId.Scatter
        ],
        // Reel 6 (34 symbols)
        [
            SymbolId.Jack, SymbolId.Scatter, SymbolId.King, SymbolId.QueenCard, SymbolId.Ace,
            SymbolId.Wolf, SymbolId.Stone, SymbolId.Queen, SymbolId.Ten, SymbolId.Jack,
            SymbolId.QueenCard, SymbolId.King, SymbolId.Ace, SymbolId.Leopard, SymbolId.Dragon,
            SymbolId.Crown, SymbolId.Ten, SymbolId.Jack, SymbolId.QueenCard, SymbolId.King,
            SymbolId.Ace, SymbolId.Wolf, SymbolId.Stone, SymbolId.Ten, SymbolId.Jack,
            SymbolId.QueenCard, SymbolId.King, SymbolId.Ace, SymbolId.Wild, SymbolId.Ten,
            SymbolId.Jack, SymbolId.QueenCard, SymbolId.King, SymbolId.Ten
        ]
    ];
}
