using System;
using System.Collections.Generic;

namespace ChessEngine
{
    class BoardController
    {
        protected Board board;

        public string Fen { get; protected set; }
        public Color CurrentMoveColor { get; protected set; }
        public bool CanCastlingA1 { get; protected set; }
        public bool CanCastlingH1 { get; protected set; }
        public bool CanCastlingA8 { get; protected set; }
        public bool CanCastlingH8 { get; protected set; }
        public Cell Enpassant { get; protected set; }
        public int DrawNumber { get; protected set; }
        public int MoveNumber { get; protected set; }

        public BoardController(string fen)
        {
            this.Fen = fen;

            FenParser.FenParseResult parsed = FenParser.Parse(fen);
            board = new Board(parsed);
            CurrentMoveColor = parsed.currentMoveColor;
            CanCastlingA1 = parsed.canCastlingA1;
            CanCastlingH1 = parsed.canCastlingH1;
            CanCastlingA8 = parsed.canCastlingA8;
            CanCastlingH8 = parsed.canCastlingH8;
            Enpassant = parsed.enpassant;
            DrawNumber = parsed.drawNumber;
            MoveNumber = parsed.moveNumber;
        }

        public BoardController Move(MoveController moveController)
        {
            return new MovedBoard(Fen, moveController);
        }

        public IEnumerable<FigureOnCell> YieldFiguresOnCell()
        {
            return board.YieldFiguresOnCell(CurrentMoveColor);
        }

        public Figure GetFigureAtCell(Cell cell)
        {
            return board.GetFigureAtCell(cell);
        }

        protected void SetFigureAtCell(Cell cell, Figure figure)
        {
            board.SetFigureAtCell(cell, figure);
        }

        public bool isCheckAfter(MoveController mc)
        {
            return CheckDetector.IsCheckAfter(this, mc);
        }

        public bool isCheck()
        {
            return CheckDetector.IsCheck(this);
        }
    }

    class MovedBoard : BoardController
    {
        MoveController mc;

        public MovedBoard(string fen, MoveController mc) : base(fen)
        {
            this.mc = mc;
            bool isCapture = IsCapture();
            DropEnpassant();
            SetEnpassant();
            MoveFigures();
            ChangeCurrentMoveColor();
            MoveCastlingRook();
            UpdateCastlingFlags();
            ChangeDrawNumber(isCapture);
            ChangeMoveNumber();
            CreateNewFen();
        }

        private bool IsCapture()
        {
            if (GetFigureAtCell(mc.NewCell) != Figure.none)
                return true;
            if (mc.NewCell == Enpassant &&
                (mc.CurrentFigure == Figure.whitePawn || mc.CurrentFigure == Figure.blackPawn))
                return true;
            return false;
        }

        private void ChangeDrawNumber(bool isCapture)
        {
            bool isPawnMove = mc.CurrentFigure == Figure.whitePawn ||
                              mc.CurrentFigure == Figure.blackPawn;
            if (isPawnMove || isCapture)
                DrawNumber = 0;
            else
                DrawNumber++;
        }

        private void MoveCastlingRook()
        {
            if (mc.CurrentFigure == Figure.whiteKing)
            {
                Cell whiteKingStartPosition = new Cell(Constants.WHITE_KING_START_POSITION);
                Cell whiteCastlingPositionToRight = new Cell(whiteKingStartPosition.x +
                    Constants.DIF_POSITION_KING_X_AFTER_CASTLING, whiteKingStartPosition.y);
                Cell whiteCastlingPositionToLeft = new Cell(whiteKingStartPosition.x -
                    Constants.DIF_POSITION_KING_X_AFTER_CASTLING, whiteKingStartPosition.y);

                if (mc.CurrentCell == whiteKingStartPosition //castling to right (white)
                && mc.NewCell == whiteCastlingPositionToRight)
                {
                    SetFigureAtCell(new Cell(Constants.COUNT_SQUARES - 1, 0), Figure.none);
                    SetFigureAtCell(new Cell(whiteCastlingPositionToRight.x - 1, 0), Figure.whiteRook);
                }
                else if (mc.CurrentCell == whiteKingStartPosition //castling to left (white)
              && mc.NewCell == whiteCastlingPositionToLeft)
                {
                    SetFigureAtCell(new Cell(0, 0), Figure.none);
                    SetFigureAtCell(new Cell(whiteCastlingPositionToLeft.x + 1, 0), Figure.whiteRook);
                }
            }
            else if (mc.CurrentFigure == Figure.blackKing)
            {
                Cell blackKingStartPosition = new Cell(Constants.BLACK_KING_START_POSITION);
                Cell blackCastlingPositionToRight = new Cell(blackKingStartPosition.x +
                    Constants.DIF_POSITION_KING_X_AFTER_CASTLING, blackKingStartPosition.y);
                Cell blackCastlingPositionToLeft = new Cell(blackKingStartPosition.x -
                    Constants.DIF_POSITION_KING_X_AFTER_CASTLING, blackKingStartPosition.y);

                if (mc.CurrentCell == blackKingStartPosition //castling to right (black)
                && mc.NewCell == blackCastlingPositionToRight)
                {
                    SetFigureAtCell(new Cell(Constants.COUNT_SQUARES - 1, Constants.COUNT_SQUARES - 1), Figure.none);
                    SetFigureAtCell(new Cell(blackCastlingPositionToRight.x - 1, Constants.COUNT_SQUARES - 1), Figure.blackRook);
                }
                else if (mc.CurrentCell == blackKingStartPosition //castling to left (black)
              && mc.NewCell == blackCastlingPositionToLeft)
                {
                    SetFigureAtCell(new Cell(0, Constants.COUNT_SQUARES - 1), Figure.none);
                    SetFigureAtCell(new Cell(blackCastlingPositionToLeft.x + 1, Constants.COUNT_SQUARES - 1), Figure.blackRook);
                }

            }

        }

        private void DropEnpassant()
        {
            if (mc.NewCell == Enpassant)
                if (mc.CurrentFigure == Figure.whitePawn ||
                mc.CurrentFigure == Figure.blackPawn)
                    SetFigureAtCell(new Cell(mc.NewCell.x, mc.CurrentCell.y), Figure.none);
        }

        private void SetEnpassant()
        {
            Enpassant = Cell.none;
            if (mc.CurrentFigure == Figure.whitePawn)
                if (mc.CurrentCell.y == Constants.HORIZONTAL_FOR_WHITE_PAWN &&
                mc.NewCell.y == Constants.HORIZONTAL_FOR_WHITE_PAWN + Constants.MAX_DIF_PAWN_Y)
                    Enpassant = new Cell(mc.CurrentCell.x, mc.CurrentCell.y + 1);
            if (mc.CurrentFigure == Figure.blackPawn)
                if (mc.CurrentCell.y == Constants.HORIZONTAL_FOR_BLACK_PAWN &&
                mc.NewCell.y == Constants.HORIZONTAL_FOR_BLACK_PAWN - Constants.MAX_DIF_PAWN_Y)
                    Enpassant = new Cell(mc.CurrentCell.x, mc.CurrentCell.y - 1);
        }

        private void ChangeMoveNumber()
        {
            if (CurrentMoveColor == Color.White)
                MoveNumber++;
        }

        private void ChangeCurrentMoveColor()
        {
            CurrentMoveColor = CurrentMoveColor.FlipColor();
        }

        private void UpdateCastlingFlags()
        {
            switch (mc.CurrentFigure)
            {
                case Figure.whiteKing:
                    CanCastlingA1 = false;
                    CanCastlingH1 = false;
                    return;
                case Figure.whiteRook:
                    CanCastlingA1 &= mc.CurrentCell != new Cell(0, 0);
                    CanCastlingH1 &= mc.CurrentCell != new Cell(Constants.COUNT_SQUARES - 1, 0);
                    return;
                case Figure.blackKing:
                    CanCastlingA8 = false;
                    CanCastlingH8 = false;
                    return;
                case Figure.blackRook:
                    CanCastlingA8 &= mc.CurrentCell != new Cell(0, Constants.COUNT_SQUARES - 1);
                    CanCastlingH8 &= mc.CurrentCell != new Cell(Constants.COUNT_SQUARES - 1, Constants.COUNT_SQUARES - 1);
                    return;
                default: return;
            }
        }

        private void MoveFigures()
        {
            SetFigureAtCell(mc.CurrentCell, Figure.none);
            SetFigureAtCell(mc.NewCell, mc.TransformatedFigure);
        }

        private void CreateNewFen()
        {
            this.Fen = FenSerializer.Serialize(
                board.figures,
                CurrentMoveColor,
                CanCastlingA1,
                CanCastlingH1,
                CanCastlingA8,
                CanCastlingH8,
                Enpassant,
                DrawNumber,
                MoveNumber);
        }
    }
}