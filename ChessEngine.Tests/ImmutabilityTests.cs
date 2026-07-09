using ChessEngine;
using FluentAssertions;
using Xunit;

namespace ChessEngine.Tests
{
    public class ImmutabilityTests
    {
        private const string StartFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

        [Fact]
        public void Move_returns_new_instance_and_leaves_source_position_intact()
        {
            var a = new Chess();
            var b = a.Move("Pe2e4");

            b.Should().NotBeSameAs(a);
            a.Fen.Should().Be(StartFen);
            a.GetFigureFromPosition("e2").Should().Be('P');
        }

        [Fact]
        public void Valid_move_does_not_mutate_source_state_flags()
        {
            var a = new Chess();

            a.Move("Pe2e4");

            a.IsCheck.Should().BeFalse();
            a.IsCheckMate.Should().BeFalse();
            a.IsStalemate.Should().BeFalse();
        }

        [Fact]
        public void Invalid_move_returns_same_instance_and_leaves_source_unchanged()
        {
            var a = new Chess();

            // FIXME(behavior): invalid move silently returns the same instance instead of throwing — pinned as current behavior
            var c = a.Move("Pe2e5");

            ReferenceEquals(c, a).Should().BeTrue();
            a.Fen.Should().Be(StartFen);
        }

        [Fact]
        public void IsValidMove_does_not_mutate_source_position()
        {
            var a = new Chess();

            a.isValidMove("Pe2e4").Should().BeTrue();
            a.Fen.Should().Be(StartFen);
        }

        [Fact]
        public void Chained_moves_produce_expected_placement_and_side_to_move()
        {
            var after = new Chess().Move("Pe2e4").Move("pe7e5").Move("Ng1f3");

            after.Fen.Should().StartWith("rnbqkbnr/pppp1ppp/8/4p3/4P3/5N2/PPPP1PPP/RNBQKB1R b");
        }
    }
}
