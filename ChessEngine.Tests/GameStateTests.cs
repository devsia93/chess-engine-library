using ChessEngine;
using FluentAssertions;
using Xunit;

namespace ChessEngine.Tests
{
    public class GameStateTests
    {
        [Theory]
        [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", false, false, false)]
        [InlineData("4k3/8/8/8/8/8/4R3/4K3 b - - 0 1", true, false, false)]
        [InlineData("rnb1kbnr/pppp1ppp/8/4p3/6Pq/5P2/PPPPP2P/RNBQKBNR w KQkq - 1 3", true, true, false)]
        [InlineData("R5k1/5ppp/8/8/8/8/8/6K1 b - - 0 1", true, true, false)]
        [InlineData("7k/5Q2/6K1/8/8/8/8/8 b - - 0 1", false, false, true)]
        [InlineData("r1bqkb1r/pppp1Qpp/2n2n2/4p3/2B1P3/8/PPPP1PPP/RNBQK1NR b KQkq - 0 4", true, true, false)]
        public void Position_flags_match_expected_state(
            string fen, bool expectedIsCheck, bool expectedIsCheckMate, bool expectedIsStalemate)
        {
            var chess = new Chess(fen);

            chess.IsCheck.Should().Be(expectedIsCheck);
            chess.IsCheckMate.Should().Be(expectedIsCheckMate);
            chess.IsStalemate.Should().Be(expectedIsStalemate);
        }

        [Fact]
        public void Move_that_delivers_check_sets_IsCheck_on_resulting_side_to_move()
        {
            var chess = new Chess("4k3/8/8/8/8/8/3R4/4K3 w - - 0 1");

            var after = chess.Move("Rd2e2");

            after.IsCheck.Should().BeTrue();
            after.IsCheckMate.Should().BeFalse();
            after.IsStalemate.Should().BeFalse();
        }
    }
}
