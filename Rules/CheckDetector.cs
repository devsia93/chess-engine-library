using System;
using System.Collections.Generic;

namespace ChessEngine
{
    internal static class CheckDetector
    {
        public static bool IsCheckAfter(BoardController state, MoveController move)
        {
            BoardController after = state.Move(move);
            return CanEatKing(after);
        }

        public static bool IsCheck(BoardController state)
        {
            return IsCheckAfter(state, MoveController.none);
        }

        private static bool CanEatKing(BoardController state)
        {
            Cell enemyKing = FindEnemyKing(state);
            MoveRules moves = new MoveRules(state);
            foreach (FigureOnCell fc in state.YieldFiguresOnCell())
                if (moves.CanMove(new MoveController(fc, enemyKing)))
                    return true;
            return false;
        }

        private static Cell FindEnemyKing(BoardController state)
        {
            Figure enemyKing = state.CurrentMoveColor == Color.White ? 
            Figure.blackKing : Figure.whiteKing;
            foreach (Cell cell in Cell.YieldBoardCells())
            {
                if (state.GetFigureAtCell(cell) == enemyKing)
                    return cell;
            }
            return Cell.none;
        }
    }
}
