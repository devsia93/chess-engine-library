using System;

namespace ChessEngine
{
     class Moves
    {
        MoveController moveController;
        BoardController boardController;

        public Moves(BoardController boardController)
        {
            this.boardController = boardController;
        }

        public bool CanMove(MoveController mc)
        {
            this.moveController = mc;
            return CanMoveCurrent() && CanMoveNew() && CanFigureMove();
        }

        private bool CanFigureMove()
        {
            switch (moveController.CurrentFigure)
            {
                case Figure.whiteKing:
                case Figure.blackKing:
                    return CheckMovesKing() || CheckCanCastling();
                case Figure.whiteQueen:
                case Figure.blackQueen:
                    return CheckStraightMoves();
                case Figure.whiteRook:
                case Figure.blackRook:
                    return (moveController.SignX == 0 || 
                    moveController.SignY == 0) && CheckStraightMoves();
                case Figure.whiteBishop:
                case Figure.blackBishop:
                    return (moveController.SignX != 0 &&
                   moveController.SignY != 0) && CheckStraightMoves();
                case Figure.whiteKnight:
                case Figure.blackKnight:
                    return CheckMovesKnight();
                case Figure.whitePawn:
                case Figure.blackPawn:
                    return CheckMovesPawn();
                default:
                    return false;
            }
        }

        private bool CheckCanCastling()
        {
            if(moveController.CurrentFigure == Figure.whiteKing)
            {
                Cell whiteKingStartPosition = new Cell(Constants.WHITE_KING_START_POSITION);
                Cell whiteCastlingPositionToRight = new Cell(whiteKingStartPosition.x +
                    Constants.DIF_POSITION_KING_X_AFTER_CASTLING, whiteKingStartPosition.y);
                Cell whiteCastlingPositionToLeft = new Cell(whiteKingStartPosition.x -
                    Constants.DIF_POSITION_KING_X_AFTER_CASTLING, whiteKingStartPosition.y);

                if (moveController.CurrentCell == whiteKingStartPosition //check can castling to right (white)
                && moveController.NewCell == whiteCastlingPositionToRight)
                    if (boardController.CanCastlingH1 && boardController.GetFigureAtCell(
                    new Cell(Constants.COUNT_SQUARES-1, 0)) == Figure.whiteRook)
                        if (checkCellsForEmpty(whiteKingStartPosition, whiteCastlingPositionToRight))
                            if (!boardController.isCheck())
                            if (!boardController.isCheckAfter(new MoveController(new FigureOnCell(
                            Figure.whiteKing, whiteKingStartPosition), new Cell(whiteKingStartPosition.x+1, 0))))
                                return true;

                if (moveController.CurrentCell == whiteKingStartPosition //check can castling to left (white)
               && moveController.NewCell == whiteCastlingPositionToLeft)
                    if (boardController.CanCastlingA1 && boardController.GetFigureAtCell(
                    new Cell(0, 0)) == Figure.whiteRook)
                        if (checkCellsForEmpty(whiteKingStartPosition, whiteCastlingPositionToLeft))
                        if (!boardController.isCheck())
                            if (!boardController.isCheckAfter(new MoveController(new FigureOnCell(
                            Figure.whiteKing, whiteKingStartPosition), new Cell(whiteKingStartPosition.x - 1, 0))))
                            return true;
            }
            else if (moveController.CurrentFigure == Figure.blackKing)
            {
                Cell blackKingStartPosition = new Cell(Constants.BLACK_KING_START_POSITION);
                Cell blackCastlingPositionToRight = new Cell(blackKingStartPosition.x +
                    Constants.DIF_POSITION_KING_X_AFTER_CASTLING, blackKingStartPosition.y);
                Cell blackCastlingPositionToLeft = new Cell(blackKingStartPosition.x -
                    Constants.DIF_POSITION_KING_X_AFTER_CASTLING, blackKingStartPosition.y);

                if (moveController.CurrentCell == blackKingStartPosition //check can castling to right (black)
                && moveController.NewCell == blackCastlingPositionToRight)
                    if (boardController.CanCastlingH8 && boardController.GetFigureAtCell(
                    new Cell(Constants.COUNT_SQUARES - 1, Constants.COUNT_SQUARES - 1)) == Figure.blackRook)
                        if (checkCellsForEmpty(blackKingStartPosition, blackCastlingPositionToRight))
                            if (!boardController.isCheck())
                            if (!boardController.isCheckAfter(new MoveController(new FigureOnCell(
                            Figure.blackKing, blackKingStartPosition), new Cell(blackKingStartPosition.x + 1, Constants.COUNT_SQUARES-1))))
                            return true;

                if (moveController.CurrentCell == blackKingStartPosition //check can castling to left (black)
               && moveController.NewCell == blackCastlingPositionToLeft)
                    if (boardController.CanCastlingA8 && boardController.GetFigureAtCell(
                    new Cell(0, Constants.COUNT_SQUARES - 1)) == Figure.blackRook)
                        if (checkCellsForEmpty(blackKingStartPosition, blackCastlingPositionToLeft))
                            if (!boardController.isCheck())
                            if (!boardController.isCheckAfter(new MoveController(new FigureOnCell(
                            Figure.blackKing, blackKingStartPosition), new Cell(blackKingStartPosition.x - 1, Constants.COUNT_SQUARES - 1))))
                            return true;
            }
            return false;
        }

        private bool checkCellsForEmpty(Cell from, Cell rookPosition)
        {
            bool result = false;
            int minX = from.x < rookPosition.x ? from.x : rookPosition.x;
            int maxX = from.x > rookPosition.x ? from.x : rookPosition.x;
            if (from.y == rookPosition.y) {
                for (int i = minX+1; i < maxX; i++)
                    if (boardController.GetFigureAtCell(new Cell(i, from.y)) != Figure.none) 
                    {
                        result = false;
                        return result;
                    }
                    else
                    {
                        result = true;
                    }
            }
            return result;
        }

        private bool CheckMovesPawn()
        {
            if (moveController.CurrentCell.y < Constants.HORIZONTAL_FOR_WHITE_PAWN
            || moveController.CurrentCell.y >
            Constants.HORIZONTAL_FOR_BLACK_PAWN)
                return false;
            int stepY = moveController.CurrentFigure.GetColor() == Color.White ?
                +1 : -1;
            return CheckPawnGo(stepY) || CheckPawnGo2Step(stepY) || 
            CheckPawnEat(stepY) || CheckEatPawnEnppasant(stepY);
        }

        private bool CheckEatPawnEnppasant(int stepY)
        {
            if (moveController.NewCell == boardController.Enpassant)
                if (boardController.GetFigureAtCell(moveController.NewCell) == Figure.none)
                    if (moveController.DifferenceY == stepY && moveController.AbsDifferenceX == 1)
                        if (stepY == 1 && moveController.CurrentCell.y == Constants.HORIZONTAL_FOR_BLACK_PAWN - Constants.MAX_DIF_PAWN_Y ||
                        stepY == -1 && moveController.CurrentCell.y == Constants.HORIZONTAL_FOR_WHITE_PAWN + Constants.MAX_DIF_PAWN_Y)
                            return true;
            return false;
        }

        private bool CheckPawnGo2Step(int stepY)
        {
            if (boardController.GetFigureAtCell(moveController.NewCell) ==
            Figure.none)//diff.x == 0 & (abs(diff.y) == 1 || abs(diff.y) == 2)
            {
                if (moveController.DifferenceX == 0)
                    if (moveController.DifferenceY ==
                Constants.MAX_DIF_PAWN_Y * stepY)
                        if (moveController.CurrentCell.y == Constants.HORIZONTAL_FOR_WHITE_PAWN &&
                        stepY == 1 ||
                        moveController.CurrentCell.y == Constants.HORIZONTAL_FOR_BLACK_PAWN &&
                        stepY == -1)
                            if (boardController.GetFigureAtCell(new Cell(moveController.NewCell.x,
                                moveController.NewCell.y + stepY)) == Figure.none)//none figure at cells for abs(diff.y) == 2 
                                return true;
            }
            return false;
        }

        private bool CheckPawnEat(int stepY)
        {
            if (boardController.GetFigureAtCell(moveController.NewCell) != Figure.none)
                if (moveController.AbsDifferenceX == 1 && moveController.DifferenceY == stepY)
                    return true;
            return false;
        }

        private bool CheckPawnGo(int stepY)
        {
            if (boardController.GetFigureAtCell(moveController.NewCell) ==
            Figure.none)//diff.x == 0 & (abs(diff.y) == 1 || abs(diff.y) == 2)
            {
                if (moveController.DifferenceX == 0)
                    if (moveController.DifferenceY == stepY)
                        return true;
            }
            return false;
        }

        private bool CheckStraightMoves()
        {
            Cell currentCell = moveController.CurrentCell;
            do
            {
                currentCell = new Cell(currentCell.x + moveController.SignX,
                    currentCell.y + moveController.SignY);
                if (currentCell == moveController.NewCell)
                    return true;
            } while (currentCell.CheckOnBoard() && boardController.
            GetFigureAtCell(currentCell) == Figure.none);
            return false;
        }

        private bool CheckMovesKnight()
        {
            return moveController.AbsDifferenceX == Constants.DIF_KNIGHT_X_1 &&
                moveController.AbsDifferenceY == Constants.DIF_KNIGHT_Y_2 ||
                moveController.AbsDifferenceX == Constants.DIF_KNIGHT_X_2 &&
                moveController.AbsDifferenceY == Constants.DIF_KNIGHT_Y_1;
        }

        private bool CheckMovesKing()
        {
            return (moveController.AbsDifferenceX <= Constants.DIF_KING_X) &&
            (moveController.AbsDifferenceY <= Constants.DIF_KING_Y);

        } 

        bool CanMoveCurrent()
        {
            return moveController.CurrentCell.CheckOnBoard() &&
                moveController.CurrentFigure.GetColor() == 
                boardController.CurrentMoveColor && boardController
                .GetFigureAtCell(moveController.CurrentCell) == 
                moveController.CurrentFigure;
        }

         bool CanMoveNew()
        {
            return moveController.NewCell.CheckOnBoard() &&
                boardController.GetFigureAtCell(moveController.NewCell)
                .GetColor() != boardController.CurrentMoveColor;
        }
    }
}
