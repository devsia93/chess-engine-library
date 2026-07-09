using System;
using System.Text;

namespace ChessEngine
{
    internal static class FenSerializer
    {
        internal static string Serialize(
            Figure[,] figures,
            Color currentMoveColor,
            bool canCastlingA1,
            bool canCastlingH1,
            bool canCastlingA8,
            bool canCastlingH8,
            Cell enpassant,
            int drawNumber,
            int moveNumber)
        {
            return GetFenFigures(figures) + " " +
            GetFenMoveColor(currentMoveColor) + " " +
            GetFenCastlingFlags(canCastlingH1, canCastlingA1, canCastlingH8, canCastlingA8) + " " +
            GetFenEnpassat(enpassant) + " " +
            GetFenDrawNumber(drawNumber) + " " +
            GetFenMoveNumber(moveNumber);
        }

        static string GetFenEnpassat(Cell enpassant)
        {
            return enpassant.Name;
        }

        static string GetFenMoveNumber(int moveNumber)
        {
            return moveNumber.ToString();
        }

        static string GetFenDrawNumber(int drawNumber)
        {
            return drawNumber.ToString();
        }

        static string GetFenCastlingFlags(bool canCastlingH1, bool canCastlingA1, bool canCastlingH8, bool canCastlingA8)
        {
            string flags = (canCastlingH1 ? "K" : "") +
                (canCastlingA1 ? "Q" : "") +
                (canCastlingH8 ? "k" : "") +
                (canCastlingA8 ? "q" : "");
            return flags == "" ? "-" : flags;
        }

        static string GetFenMoveColor(Color currentMoveColor)
        {
            return currentMoveColor == Color.White ? "w" : "b";
        }

        static string GetFenFigures(Figure[,] figures)
        {
            StringBuilder sb = new StringBuilder();
            for (int y = Constants.COUNT_SQUARES - 1; y >= 0; y--)
            {
                for (int x = 0; x < Constants.COUNT_SQUARES; x++)
                    sb.Append(figures[x, y] == Figure.none ?
                    '1' : (char)figures[x, y]);
                if (y > 0)
                    sb.Append("/");
            }
            string emptyLine = GenerateEmptyLine();
            for (int i = Constants.COUNT_SQUARES; i >= 2; i--)
                sb.Replace(emptyLine.Substring(0, i), i.ToString());
            return sb.ToString();

        }

        static string GenerateEmptyLine()
        {
            string result = string.Empty;
            for (int i = 0; i < Constants.COUNT_SQUARES; i++)
                result += "1";
            return result;
        }
    }
}
