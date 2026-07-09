using System;
namespace ChessEngine
{
    enum Color
    {
        None,
        White,
        Black
    }

    static class ColorMethods
    {
        public static Color FlipColor(this Color color)
        {
            if (color == Color.Black)
                return Color.White;
            if (color == Color.White)
                return Color.Black;
            return color;
        }
    }
}
