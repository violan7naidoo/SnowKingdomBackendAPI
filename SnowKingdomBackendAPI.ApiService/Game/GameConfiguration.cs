using SnowKingdomBackendAPI.ApiService.Models;

namespace SnowKingdomBackendAPI.ApiService.Game;

public static class GameConfiguration
{
    public static readonly Dictionary<SymbolId, SymbolConfig> Symbols = new()
    {
        [SymbolId.WILD] = new SymbolConfig
        {
            Id = SymbolId.WILD,
            Name = "WILD",
            Payout = new Dictionary<int, int> { { 2, 5 }, { 3, 20 }, { 4, 100 }, { 5, 500 }, { 6, 2000 } },
            Image = "/images/symbols/Wild.png"
        },
        [SymbolId.SCATTER] = new SymbolConfig
        {
            Id = SymbolId.SCATTER,
            Name = "SCATTER",
            Payout = new Dictionary<int, int> { { 3, 5 }, { 4, 30 }, { 5, 150 }, { 6, 1000 } },
            Image = "/images/symbols/Scatter.png"
        },
        [SymbolId.CROWN] = new SymbolConfig
        {
            Id = SymbolId.CROWN,
            Name = "CROWN",
            Payout = new Dictionary<int, int> { { 3, 10 }, { 4, 60 }, { 5, 100 }, { 6, 150 } },
            Image = "/images/symbols/Crown.png"
        },
        [SymbolId.DRAGON] = new SymbolConfig
        {
            Id = SymbolId.DRAGON,
            Name = "DRAGON",
            Payout = new Dictionary<int, int> { { 3, 10 }, { 4, 30 }, { 5, 85 }, { 6, 120 } },
            Image = "/images/symbols/Dragon.png"
        },
        [SymbolId.LEOPARD] = new SymbolConfig
        {
            Id = SymbolId.LEOPARD,
            Name = "LEOPARD",
            Payout = new Dictionary<int, int> { { 3, 5 }, { 4, 30 }, { 5, 85 }, { 6, 120 } },
            Image = "/images/symbols/Leopard.png"
        },
        [SymbolId.QUEEN] = new SymbolConfig
        {
            Id = SymbolId.QUEEN,
            Name = "QUEEN",
            Payout = new Dictionary<int, int> { { 3, 5 }, { 4, 20 }, { 5, 60 }, { 6, 100 } },
            Image = "/images/symbols/Queen.png"
        },
        [SymbolId.STONE] = new SymbolConfig
        {
            Id = SymbolId.STONE,
            Name = "STONE",
            Payout = new Dictionary<int, int> { { 3, 5 }, { 4, 20 }, { 5, 60 }, { 6, 100 } },
            Image = "/images/symbols/Stone.png"
        },
        [SymbolId.WOLF] = new SymbolConfig
        {
            Id = SymbolId.WOLF,
            Name = "WOLF",
            Payout = new Dictionary<int, int> { { 3, 5 }, { 4, 20 }, { 5, 60 }, { 6, 100 } },
            Image = "/images/symbols/Wolf.png"
        },
        [SymbolId.ACE] = new SymbolConfig
        {
            Id = SymbolId.ACE,
            Name = "ACE",
            Payout = new Dictionary<int, int> { { 3, 2 }, { 4, 5 }, { 5, 20 }, { 6, 40 } },
            Image = "/images/symbols/A.png"
        },
        [SymbolId.JACK] = new SymbolConfig
        {
            Id = SymbolId.JACK,
            Name = "JACK",
            Payout = new Dictionary<int, int> { { 3, 2 }, { 4, 5 }, { 5, 20 }, { 6, 40 } },
            Image = "/images/symbols/J.png"
        },
        [SymbolId.QUEEN_CARD] = new SymbolConfig
        {
            Id = SymbolId.QUEEN_CARD,
            Name = "QUEEN_CARD",
            Payout = new Dictionary<int, int> { { 3, 2 }, { 4, 5 }, { 5, 20 }, { 6, 40 } },
            Image = "/images/symbols/Q.png"
        },
        [SymbolId.KING] = new SymbolConfig
        {
            Id = SymbolId.KING,
            Name = "KING",
            Payout = new Dictionary<int, int> { { 3, 2 }, { 4, 5 }, { 5, 20 }, { 6, 40 } },
            Image = "/images/symbols/K.png"
        },
        [SymbolId.TEN] = new SymbolConfig
        {
            Id = SymbolId.TEN,
            Name = "TEN",
            Payout = new Dictionary<int, int> { { 3, 2 }, { 4, 5 }, { 5, 20 }, { 6, 40 } },
            Image = "/images/symbols/10.png"
        }
    };

    public static readonly List<List<SymbolId>> ReelStrips =
    [
        // Reel 1 (34 symbols)
        [
            SymbolId.KING, SymbolId.CROWN, SymbolId.QUEEN_CARD, SymbolId.TEN, SymbolId.ACE,
            SymbolId.WOLF, SymbolId.STONE, SymbolId.QUEEN_CARD, SymbolId.TEN, SymbolId.JACK,
            SymbolId.QUEEN_CARD, SymbolId.KING, SymbolId.ACE, SymbolId.LEOPARD, SymbolId.DRAGON,
            SymbolId.JACK, SymbolId.TEN, SymbolId.JACK, SymbolId.QUEEN_CARD, SymbolId.KING,
            SymbolId.ACE, SymbolId.WOLF, SymbolId.STONE, SymbolId.TEN, SymbolId.JACK,
            SymbolId.QUEEN_CARD, SymbolId.KING, SymbolId.ACE, SymbolId.WILD, SymbolId.TEN,
            SymbolId.JACK, SymbolId.QUEEN_CARD, SymbolId.KING, SymbolId.SCATTER
        ],
        // Reel 2 (34 symbols)
        [
            SymbolId.TEN, SymbolId.STONE, SymbolId.QUEEN, SymbolId.KING, SymbolId.ACE,
            SymbolId.WOLF, SymbolId.STONE, SymbolId.QUEEN_CARD, SymbolId.TEN, SymbolId.JACK,
            SymbolId.QUEEN_CARD, SymbolId.KING, SymbolId.ACE, SymbolId.LEOPARD, SymbolId.DRAGON,
            SymbolId.CROWN, SymbolId.TEN, SymbolId.JACK, SymbolId.QUEEN_CARD, SymbolId.KING,
            SymbolId.ACE, SymbolId.WOLF, SymbolId.JACK, SymbolId.TEN, SymbolId.JACK,
            SymbolId.QUEEN_CARD, SymbolId.KING, SymbolId.ACE, SymbolId.WILD, SymbolId.TEN,
            SymbolId.JACK, SymbolId.QUEEN_CARD, SymbolId.KING, SymbolId.SCATTER
        ],
        // Reel 3 (34 symbols)
        [
            SymbolId.WILD, SymbolId.ACE, SymbolId.WOLF, SymbolId.STONE, SymbolId.ACE,
            SymbolId.QUEEN_CARD, SymbolId.STONE, SymbolId.QUEEN, SymbolId.TEN, SymbolId.JACK,
            SymbolId.QUEEN_CARD, SymbolId.KING, SymbolId.ACE, SymbolId.LEOPARD, SymbolId.DRAGON,
            SymbolId.CROWN, SymbolId.TEN, SymbolId.JACK, SymbolId.QUEEN_CARD, SymbolId.KING,
            SymbolId.ACE, SymbolId.WOLF, SymbolId.KING, SymbolId.TEN, SymbolId.JACK,
            SymbolId.QUEEN_CARD, SymbolId.KING, SymbolId.JACK, SymbolId.WILD, SymbolId.TEN,
            SymbolId.JACK, SymbolId.QUEEN_CARD, SymbolId.KING, SymbolId.SCATTER
        ],
        // Reel 4 (34 symbols)
        [
            SymbolId.TEN, SymbolId.JACK, SymbolId.QUEEN_CARD, SymbolId.KING, SymbolId.ACE,
            SymbolId.WOLF, SymbolId.STONE, SymbolId.QUEEN, SymbolId.TEN, SymbolId.JACK,
            SymbolId.QUEEN_CARD, SymbolId.KING, SymbolId.ACE, SymbolId.LEOPARD, SymbolId.DRAGON,
            SymbolId.CROWN, SymbolId.TEN, SymbolId.JACK, SymbolId.QUEEN_CARD, SymbolId.KING,
            SymbolId.ACE, SymbolId.WOLF, SymbolId.STONE, SymbolId.TEN, SymbolId.JACK,
            SymbolId.QUEEN_CARD, SymbolId.KING, SymbolId.ACE, SymbolId.WILD, SymbolId.TEN,
            SymbolId.JACK, SymbolId.QUEEN_CARD, SymbolId.KING, SymbolId.SCATTER
        ],
        // Reel 5 (34 symbols)
        [
            SymbolId.DRAGON, SymbolId.WILD, SymbolId.LEOPARD, SymbolId.JACK, SymbolId.ACE,
            SymbolId.WOLF, SymbolId.STONE, SymbolId.QUEEN, SymbolId.TEN, SymbolId.JACK,
            SymbolId.QUEEN_CARD, SymbolId.KING, SymbolId.ACE, SymbolId.LEOPARD, SymbolId.TEN,
            SymbolId.CROWN, SymbolId.TEN, SymbolId.JACK, SymbolId.QUEEN_CARD, SymbolId.KING,
            SymbolId.ACE, SymbolId.WOLF, SymbolId.STONE, SymbolId.TEN, SymbolId.JACK,
            SymbolId.QUEEN_CARD, SymbolId.KING, SymbolId.ACE, SymbolId.KING, SymbolId.TEN,
            SymbolId.JACK, SymbolId.QUEEN_CARD, SymbolId.KING, SymbolId.SCATTER
        ],
        // Reel 6 (34 symbols)
        [
            SymbolId.JACK, SymbolId.SCATTER, SymbolId.KING, SymbolId.QUEEN_CARD, SymbolId.ACE,
            SymbolId.WOLF, SymbolId.STONE, SymbolId.QUEEN, SymbolId.TEN, SymbolId.JACK,
            SymbolId.QUEEN_CARD, SymbolId.KING, SymbolId.ACE, SymbolId.LEOPARD, SymbolId.DRAGON,
            SymbolId.CROWN, SymbolId.TEN, SymbolId.JACK, SymbolId.QUEEN_CARD, SymbolId.KING,
            SymbolId.ACE, SymbolId.WOLF, SymbolId.STONE, SymbolId.TEN, SymbolId.JACK,
            SymbolId.QUEEN_CARD, SymbolId.KING, SymbolId.ACE, SymbolId.WILD, SymbolId.TEN,
            SymbolId.JACK, SymbolId.QUEEN_CARD, SymbolId.KING, SymbolId.TEN
        ]
    ];
}
