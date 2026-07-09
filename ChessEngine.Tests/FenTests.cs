using ChessEngine;
using FluentAssertions;
using Xunit;

namespace ChessEngine.Tests
{
    public class FenTests
    {
        private const string StartFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

        [Theory]
        [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1")]
        [InlineData("rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq e3 0 1")]
        [InlineData("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - 0 1")]
        [InlineData("8/2k5/8/8/8/8/5K2/8 w - - 0 1")]
        [InlineData("8/P6k/8/8/8/8/8/K7 w - - 0 1")]
        public void Fen_round_trips_through_constructor(string fen)
        {
            var chess = new Chess(fen);

            chess.Fen.Should().Be(fen);
        }

        [Fact]
        public void Default_constructor_yields_starting_position_fen()
        {
            var chess = new Chess();

            chess.Fen.Should().Be(StartFen);
        }

        [Theory]
        [InlineData("e1", 'K')]
        [InlineData("d8", 'q')]
        [InlineData("e4", '.')]
        public void GetFigureFromPosition_reads_piece_placement_from_startpos(string coordinate, char expected)
        {
            var chess = new Chess(StartFen);

            chess.GetFigureFromPosition(coordinate).Should().Be(expected);
        }

        [Fact]
        public void Move_e2e4_from_startpos_advances_pawn_and_side_to_move()
        {
            var chess = new Chess(StartFen);

            var after = chess.Move("Pe2e4");

            after.Fen.Should().Be("rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq e3 0 1");
        }

        [Fact]
        public void Move_e2e4_e7e5_from_startpos_yields_correct_counters()
        {
            var chess = new Chess(StartFen);

            var after = chess.Move("Pe2e4").Move("pe7e5");

            after.Fen.Should().Be("rnbqkbnr/pppp1ppp/8/4p3/4P3/8/PPPP1PPP/RNBQKBNR w KQkq e6 0 2");
        }

        [Fact]
        public void Move_knight_from_kiwipete_increments_halfmove_and_keeps_fullmove()
        {
            const string kiwipete = "r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - 0 1";
            var chess = new Chess(kiwipete);

            var after = chess.Move("Nc3b1");

            after.Fen.Should().Be("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/5Q1p/PPPBBPPP/RN2K2R b KQkq - 1 1");
        }
    }
}
