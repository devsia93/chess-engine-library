using System;
using System.Collections.Generic;
using System.Text;

namespace ChessEngine
{
    class BoardController
    {
        protected Figure[,] figures;

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
            figures = new Figure[Constants.COUNT_SQUARES, Constants.COUNT_SQUARES];

            Initialise();
        }

        public BoardController Move(MoveController moveController)
        {
            return new Board(Fen, moveController);
        }

        public IEnumerable<FigureOnCell> YieldFiguresOnCell()
        {
            foreach (Cell cell in Cell.YieldBoardCells())
            {
                if (GetFigureAtCell(cell).GetColor() == CurrentMoveColor)
                    yield return new FigureOnCell(GetFigureAtCell(cell), cell);
            }
        }

        public Figure GetFigureAtCell(Cell cell)
        {
            if (cell.CheckOnBoard())
                return figures[cell.x, cell.y];
            return Figure.none;
        }

        void Initialise()
        {
            //rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1
            //0                                           1 2    3 4 5 indexes
            string[] parts = Fen.Split();
            InitFigures(parts[0]);
            InitMoveColor(parts[1]);
            InitCastlingFlags(parts[2]);
            InitEnpassant(parts[3]);
            InitDrawNumber(parts[4]);
            InitMoveDraw(parts[5]);
        }

        public bool isCheckAfter(MoveController mc)
        {
            BoardController after = Move(mc);
            return after.CanEatKing();
        }

        private bool CanEatKing()
        {
            Cell enemyKing = FindEnemyKing();
            Moves moves = new Moves(this);
            foreach (FigureOnCell fc in YieldFiguresOnCell())
                if (moves.CanMove(new MoveController(fc, enemyKing)))
                    return true;
            return false;
        }

        private Cell FindEnemyKing()
        {
            Figure enemyKing = CurrentMoveColor == Color.White ? 
            Figure.blackKing : Figure.whiteKing;
            foreach (Cell cell in Cell.YieldBoardCells())
            {
                if (GetFigureAtCell(cell) == enemyKing)
                    return cell;
            }
            return Cell.none;
        }

        private void InitMoveDraw(string v)
        {
            MoveNumber = int.Parse(v);
        }

        private void InitDrawNumber(string v)
        {
            DrawNumber = int.Parse(v);
        }

        private void InitEnpassant(string v)
        {
            Enpassant = new Cell(v);
        }

        private void InitCastlingFlags(string v)
        {
            CanCastlingA1 = v.Contains("Q");
            CanCastlingH1 = v.Contains("K");
            CanCastlingA8 = v.Contains("q");
            CanCastlingH8 = v.Contains("k");
        }

        private void InitMoveColor(string v)
        {
            CurrentMoveColor = v == "b" ? Color.Black : Color.White;
        }

        private void InitFigures(string v)
        {
            for (int j = Constants.COUNT_SQUARES; j >= 2; j--)
                v = v.Replace(j.ToString(), (j - 1).ToString() + "1");
            v = v.Replace('1', (char)Figure.none);
            string[] lines = v.Split('/');
            for (int y = Constants.COUNT_SQUARES - 1; y >= 0; y--)
                for (int x = 0; x < Constants.COUNT_SQUARES; x++)
                    figures[x, y] = (Figure)lines[(Constants.COUNT_SQUARES - 1) - y][x];
        }

        public bool isCheck()
        {
            return isCheckAfter(MoveController.none);
        }
    }

    class Board : BoardController
    {
        MoveController mc;

        public Board(string fen, MoveController mc) : base(fen)
        {
            this.mc = mc;
            DropEnpassant();
            SetEnpassant();
            MoveFigures();
            ChangeCurrentMoveColor();
            MoveCastlingRook();
            UpdateCastlingFlags();
            ChangeMoveNumber();
            CreateNewFen();
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
            if (CurrentMoveColor == Color.Black)
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
                    CanCastlingA1 &= mc.CurrentCell != new Cell(0, Constants.COUNT_SQUARES - 1);
                    CanCastlingH1 &= mc.CurrentCell != new Cell(Constants.COUNT_SQUARES - 1, Constants.COUNT_SQUARES - 1);
                    return;
                default: return;
            }
        }

        private void MoveFigures()
        {
            SetFigureAtCell(mc.CurrentCell, Figure.none);
            SetFigureAtCell(mc.NewCell, mc.TransformatedFigure);
        }

        private void SetFigureAtCell(Cell cell, Figure figure)
        {
            if (cell.CheckOnBoard())
                figures[cell.x, cell.y] = figure;
        }

        private void CreateNewFen()
        {
            this.Fen = GetFenFigures() + " " +
            GetFenMoveColor() + " " +
            GetFenCastlingFlags() + " " +
            GetFenEnpassat() + " " +
            GetFenDrawNumber() + " " +
            GetFenMoveNumber();
        }

        private string GetFenEnpassat()
        {
            return this.Enpassant.Name;
        }

        private string GetFenMoveNumber()
        {
            return this.MoveNumber.ToString();
        }

        private string GetFenDrawNumber()
        {
            return this.DrawNumber.ToString();
        }

        private string GetFenCastlingFlags()
        {
            string flags = (CanCastlingH1 ? "K" : "") +
                (CanCastlingA1 ? "Q" : "") +
                (CanCastlingH8 ? "k" : "") +
                (CanCastlingA8 ? "q" : "");
            return flags == "" ? "-" : flags;
        }

        private string GetFenMoveColor()
        {
            return this.CurrentMoveColor == Color.White ? "w" : "b";
        }

        private string GetFenFigures()
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

        private string GenerateEmptyLine()
        {
            string result = string.Empty;
            for (int i = 0; i < Constants.COUNT_SQUARES; i++)
                result += "1";
            return result;
        }
    }
}