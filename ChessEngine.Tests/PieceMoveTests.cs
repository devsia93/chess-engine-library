using System.Collections.Generic;
using System.Linq;
using ChessEngine;
using FluentAssertions;
using Xunit;

namespace ChessEngine.Tests
{
    public class PieceMoveTests
    {
        // Move string = <pieceChar><from><to>[<promoChar>]; piece chars are FEN chars
        // (white upper, black lower); castling is a two-square KING move (e.g. Ke1g1).
        // "moves for a square" = yielded moves whose from-field matches that square.
        private static IEnumerable<string> MovesForSquare(Chess chess, string from)
            => chess.YieldValidMoves().Where(m => m.Substring(1, 2) == from);

        public static IEnumerable<object[]> ExactMoveSetCases()
        {
            // 1. Knight b1 from the starting position: a3 and c3 (d2 is blocked by own pawn).
            yield return new object[]
            {
                "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1",
                "b1",
                new[] { "Nb1a3", "Nb1c3" }
            };

            // 2. Blocked rook: own pawn a2 blocks the a-file, own king e1 blocks the rank.
            //    The Q-side castling Ke1c1 belongs to the KING's square (e1), not a1's set.
            yield return new object[]
            {
                "8/8/8/8/8/8/P7/R3K3 w Q - 0 1",
                "a1",
                new[] { "Ra1b1", "Ra1c1", "Ra1d1" }
            };

            // 3. Sliding capture, BUT the white rook is absolutely pinned (own king d1 directly
            //    behind it on the d-file, black rook d5 directly in front). Standard chess allows
            //    only d-file moves (incl. capturing d5); lateral rank moves would expose the king
            //    and are illegal. This is isomorphic to case 4. The task text says "plus lateral
            //    moves"; standard chess drops them -- see report. Flags a re-pin follow-up risk.
            yield return new object[]
            {
                "4k3/8/8/3r4/8/8/3R4/3K4 w - - 0 1",
                "d2",
                new[] { "Rd2d3", "Rd2d4", "Rd2d5" }
            };

            // 4. Vertical pin: rook e2 is pinned by the adjacent black rook e3 to the king on e1.
            //    The only legal move is capturing the pinning rook on e3.
            yield return new object[]
            {
                "4k3/8/8/8/8/4r3/4R3/4K3 w - - 0 1",
                "e2",
                new[] { "Re2e3" }
            };

            // 5a. Pawn on its starting square: one- and two-step pushes.
            yield return new object[]
            {
                "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1",
                "e2",
                new[] { "Pe2e3", "Pe2e4" }
            };

            // 5b. Same pawn with the two-step square (e4) blocked by a black pawn.
            yield return new object[]
            {
                "4k3/8/8/8/4p3/8/4P3/4K3 w - - 0 1",
                "e2",
                new[] { "Pe2e3" }
            };

            // 6. Pawn captures: diagonal captures of black pawns on d3/f3 plus the two pushes.
            yield return new object[]
            {
                "4k3/8/8/8/8/3p1p2/4P3/4K3 w - - 0 1",
                "e2",
                new[] { "Pe2e3", "Pe2e4", "Pe2d3", "Pe2f3" }
            };

            // 7. Promotion: a white pawn reaching the 8th rank yields four move strings,
            //    one per promotion piece (Q,R,B,N).
            yield return new object[]
            {
                "8/P6k/8/8/8/8/8/K7 w - - 0 1",
                "a7",
                new[] { "Pa7a8Q", "Pa7a8R", "Pa7a8B", "Pa7a8N" }
            };

            // 10. King in check from an adjacent black rook on e2. The king may capture the
            //      undefended rook (e2) or step off the e-file to d1/f1. d2 and f2 stay on the
            //      rook's rank (attacked), so they are illegal.
            yield return new object[]
            {
                "4k3/8/8/8/8/8/4r3/4K3 w - - 0 1",
                "e1",
                new[] { "Ke1e2", "Ke1d1", "Ke1f1" }
            };
        }

        [Theory]
        [MemberData(nameof(ExactMoveSetCases))]
        public void Square_yields_exact_expected_legal_move_set(string fen, string from, string[] expected)
        {
            var chess = new Chess(fen);

            var moves = MovesForSquare(chess, from);

            moves.Should().BeEquivalentTo(expected);
        }

        // 8. En passant: the e.p. target square is f6, so the e5 pawn may capture e.p. to f6 and
        //    push to e6; it must NOT capture e.p. to d6 (e.p. target is f6 only). The exact
        //    standard-legal set is { "Pe5e6", "Pe5f6" }; asserted via inclusion/exclusion per task.
        [Fact]
        public void Pawn_e5_can_capture_en_passant_to_f6_but_not_d6()
        {
            var chess = new Chess("rnbqkbnr/ppp1p1pp/8/3pPp2/8/8/PPPP1PPP/RNBQKBNR w KQkq f6 0 3");

            var moves = MovesForSquare(chess, "e5");

            moves.Should().Contain("Pe5e6");
            moves.Should().Contain("Pe5f6");
            moves.Should().NotContain("Pe5d6");
        }

        // 9. Castling rights (Kiwipete): with KQkq and b1/c1/d1/f1/g1 empty and unattacked, the
        //    white king may castle both sides. The task asks only for inclusion of Ke1g1/Ke1c1.
        //    (Full exact standard-legal set for the e1 king here is { Ke1d1, Ke1f1, Ke1g1, Ke1c1 } --
        //    no black piece attacks rank 1 -- but only the two castling moves are asserted per task.)
        [Fact]
        public void King_e1_can_castle_both_sides_in_kiwipete()
        {
            var chess = new Chess("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - 0 1");

            var moves = MovesForSquare(chess, "e1");

            moves.Should().Contain("Ke1g1");
            moves.Should().Contain("Ke1c1");
        }
    }
}
