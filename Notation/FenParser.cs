using System;

namespace ChessEngine
{
    internal static class FenParser
    {
        internal class FenParseResult
        {
            internal Figure[,] figures;
            internal Color currentMoveColor;
            internal bool canCastlingA1;
            internal bool canCastlingH1;
            internal bool canCastlingA8;
            internal bool canCastlingH8;
            internal Cell enpassant;
            internal int drawNumber;
            internal int moveNumber;
        }

        internal static FenParseResult Parse(string fen)
        {
            FenParseResult r = new FenParseResult();
            r.figures = new Figure[Constants.COUNT_SQUARES, Constants.COUNT_SQUARES];

            //rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1
            //0                                           1 2    3 4 5 indexes
            string[] parts = fen.Split();
            InitFigures(r, parts[0]);
            InitMoveColor(r, parts[1]);
            InitCastlingFlags(r, parts[2]);
            InitEnpassant(r, parts[3]);
            InitDrawNumber(r, parts[4]);
            InitMoveDraw(r, parts[5]);
            return r;
        }

        static void InitMoveDraw(FenParseResult r, string v)
        {
            r.moveNumber = int.Parse(v);
        }

        static void InitDrawNumber(FenParseResult r, string v)
        {
            r.drawNumber = int.Parse(v);
        }

        static void InitEnpassant(FenParseResult r, string v)
        {
            r.enpassant = new Cell(v);
        }

        static void InitCastlingFlags(FenParseResult r, string v)
        {
            r.canCastlingA1 = v.Contains("Q");
            r.canCastlingH1 = v.Contains("K");
            r.canCastlingA8 = v.Contains("q");
            r.canCastlingH8 = v.Contains("k");
        }

        static void InitMoveColor(FenParseResult r, string v)
        {
            r.currentMoveColor = v == "b" ? Color.Black : Color.White;
        }

        static void InitFigures(FenParseResult r, string v)
        {
            for (int j = Constants.COUNT_SQUARES; j >= 2; j--)
                v = v.Replace(j.ToString(), (j - 1).ToString() + "1");
            v = v.Replace('1', (char)Figure.none);
            string[] lines = v.Split('/');
            for (int y = Constants.COUNT_SQUARES - 1; y >= 0; y--)
                for (int x = 0; x < Constants.COUNT_SQUARES; x++)
                    r.figures[x, y] = (Figure)lines[(Constants.COUNT_SQUARES - 1) - y][x];
        }
    }
}
