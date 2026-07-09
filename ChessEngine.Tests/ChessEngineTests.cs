using ChessEngine;
using FluentAssertions;
using Xunit;

namespace ChessEngine.Tests
{
    public class ChessEngineTests
    {
        [Fact]
        public void Chess_can_be_instantiated_from_the_default_starting_position()
        {
            var chess = new Chess();

            chess.Should().NotBeNull();
            chess.Fen.Should().NotBeNullOrEmpty();
        }
    }
}
