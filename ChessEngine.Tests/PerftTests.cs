using ChessEngine;
using FluentAssertions;
using Xunit;

namespace ChessEngine.Tests
{
    public class PerftTests
    {
        private const string StartposFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

        private const string KiwipeteFen =
            "r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - 0 1";

        // Standard perft traversal: count every leaf position reached at the given depth.
        // Every move yielded by YieldValidMoves() is valid by construction, so Move(move)
        // always advances to a real child position here.
        private static long Perft(Chess position, int depth)
        {
            if (depth == 0) return 1;

            long nodes = 0;
            foreach (var move in position.YieldValidMoves())
                nodes += Perft(position.Move(move), depth - 1);

            return nodes;
        }

        [Theory(DisplayName = "Perft matches standard published node counts")]
        [Trait("Category", "Slow")]
        [InlineData(StartposFen, 1, 20L)]
        [InlineData(StartposFen, 2, 400L)]
        [InlineData(StartposFen, 3, 8902L)]
        [InlineData(StartposFen, 4, 197281L)]
        [InlineData(KiwipeteFen, 1, 48L)]
        [InlineData(KiwipeteFen, 2, 2039L)]
        [InlineData(KiwipeteFen, 3, 97862L)]
        public void Perft_matches_standard_node_counts(string fen, int depth, long expected)
        {
            var chess = new Chess(fen);

            Perft(chess, depth).Should().Be(expected);
        }
    }
}
