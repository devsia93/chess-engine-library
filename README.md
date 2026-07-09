# ChessEngine

An immutable, FEN-based chess engine library for .NET, targeting both **netstandard2.0** and **net8.0**. Every legal move returns a brand-new position, so a game is a chain of values you can branch, undo, or replay without ever mutating the board you came from.

## Features

- **Legal move generation** — `YieldValidMoves()` enumerates only fully legal moves; pseudo-legal moves that leave your own king in check are filtered out, including pinned pieces.
- **Check / checkmate / stalemate detection** — `IsCheck`, `IsCheckMate`, and `IsStalemate` flags, recomputed for every position.
- **Castling, en passant, and promotion** — all special moves are supported. Castling is encoded as a two-square king move (e.g. `Ke1g1`).
- **FEN parse & serialize** — positions are constructed from a FEN string and round-trip back to FEN exactly.
- **Immutable API** — `Move(...)` returns a new `Chess` instance; the source position is never touched.

## Quick start

```csharp
using ChessEngine;

var game = new Chess();                 // standard starting position
game = game.Move("Pe2e4");              // returns a NEW position; `game` is re-bound

foreach (var move in game.YieldValidMoves())   // legal moves for the side to move
    Console.WriteLine(move);

Console.WriteLine(game.Fen);
// rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq e3 0 1
```

**Move string format:** `<FEN piece char><from><to>[<promotion>]`.

| Example   | Meaning                                          |
| --------- | ------------------------------------------------ |
| `Pe2e4`   | White pawn e2 -> e4                              |
| `pe7e5`   | Black pawn e7 -> e5 (lowercase = Black)          |
| `Ph7h8Q`  | White pawn promotes to a queen on h8             |
| `Ke1g1`   | White kingside castling (king two squares to g1) |
| `Ke1c1`   | White queenside castling                         |

Piece characters are the FEN letters: uppercase `PNBRQK` for White, lowercase `pnbrqk` for Black.

## Architecture

The engine is split into single-responsibility layers, each in its own folder:

```
Chess.cs        Chess            public facade: FEN, move API, game-state flags
Controllers/    BoardController  board state + move application (MovedBoard subclass)
Board/          Board            the 8x8 piece grid (Figure[,])
Rules/          MoveRules        per-piece legality, castling, en passant
                CheckDetector    is-check / is-check-after simulation
Notation/       FenParser        FEN string  -> structured position
                FenSerializer    structured position -> FEN string
Model/          Move, Cell, Figure, FigureOnCell, Color   domain primitives
Constants.cs    board geometry constants
```

**Design.** Each layer owns one concern: `Notation/` turns strings into state and back, `Board/` stores the piece grid, `Rules/` decides what is legal, and `Model/` holds the value types. The `Chess` facade composes them and keeps the public API tiny. Immutability is structural: applying a move builds a fresh `BoardController` from the current FEN and replays only that one move, then re-serializes — so the previous `Chess` is left completely untouched and can be reused as a branch point.

## Verified by perft

[Perft](https://www.chessprogramming.org/Perft) counts every leaf position reached at a given depth, so it is a brutal end-to-end check of move generation, special moves, and check filtering. This engine matches the published reference node counts exactly:

| Position  | Depth | Nodes   | Matches reference |
| --------- | ----- | ------- | ----------------- |
| Startpos  | 1     | 20      | yes               |
| Startpos  | 2     | 400     | yes               |
| Startpos  | 3     | 8902    | yes               |
| Startpos  | 4     | 197281  | yes               |
| Kiwipete  | 1     | 48      | yes               |
| Kiwipete  | 2     | 2039    | yes               |
| Kiwipete  | 3     | 97862   | yes               |

The 43-test suite did more than guard the code — it found and drove the fix of four real bugs: a double-push allowed through an occupied square, a castling-rights color mixup, queenside castling permitted over an occupied b-file, and incorrect FEN half-move/full-move counters.

## Testing

The suite is xUnit + FluentAssertions and runs in Docker, so no local .NET SDK is required:

```bash
./test.sh                 # full suite (43 tests)
./test.sh PerftTests      # one test class: --filter "FullyQualifiedName~PerftTests"
```

`./test.sh` runs `dotnet test ChessEngine.sln` inside `mcr.microsoft.com/dotnet/sdk:8.0`; pass a class-name fragment as the first argument to filter to a single test class.

## In use

This engine is the rules backend for the **unity-chess-2d** rendering frontend in the sibling repository.

## License

AGPL-3.0-only — see [LICENSE](LICENSE).
