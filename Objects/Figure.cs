using System;
using System.Collections.Generic;
namespace ChessEngine
{
    enum Figure
    {
        none, 
        whiteKing = 'K',
        whiteQueen = 'Q',
        whiteRook = 'R',
        whiteBishop = 'B',
        whiteKnight = 'N',
        whitePawn = 'P',
        blackKing = 'k',
        blackQueen = 'q',
        blackRook = 'r',
        blackBishop = 'b',
        blackKnight = 'n',
        blackPawn = 'p'
    }

    static class FigureMethods
    {
        public static Color GetColor(this Figure figure)
        {
            if (figure == Figure.none)
                return Color.None;

            return figure.ToString().ToLower().Contains("white") ?
            Color.White : Color.Black;
        }

        public static IEnumerable<Figure> YieldTransformations(this Figure figure, Cell cell)
        {
            if (figure == Figure.whitePawn && cell.y == Constants.COUNT_SQUARES - 1)
            {
                yield return Figure.whiteQueen;
                yield return Figure.whiteRook;
                yield return Figure.whiteBishop;
                yield return Figure.whiteKnight;
            }
            else if (figure == Figure.blackPawn && cell.y == 0)
            {
                yield return Figure.blackQueen;
                yield return Figure.blackRook;
                yield return Figure.blackBishop;
                yield return Figure.blackKnight;
            }
            else yield return Figure.none;
        }
    }
}
