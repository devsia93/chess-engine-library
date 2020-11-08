using System;
namespace ChessEngine
{
    public static class Constants
    { 
       public static readonly int COUNT_SQUARES = 8;

        public static readonly int DIF_KING_X = 1;
        public static readonly int DIF_KING_Y = 1;
        public static readonly string WHITE_KING_START_POSITION = "e1";
        public static readonly string BLACK_KING_START_POSITION = "e"+COUNT_SQUARES.ToString();
        public static readonly int DIF_POSITION_KING_X_AFTER_CASTLING = 2;


        public static readonly int HORIZONTAL_FOR_WHITE_PAWN = 1;
        public static readonly int HORIZONTAL_FOR_BLACK_PAWN = 
        COUNT_SQUARES - 2;
        public static readonly int MAX_DIF_PAWN_Y = 2;

        public static readonly int DIF_KNIGHT_X_1 = 1;
        public static readonly int DIF_KNIGHT_X_2 = 2;
        public static readonly int DIF_KNIGHT_Y_1 = 1;
        public static readonly int DIF_KNIGHT_Y_2 = 2;
    }
}
