using System;
using System.Collections.Generic;

namespace ChessEngine
{
    public class Chess
    {
        BoardController boardController;
        Moves moves;

        public bool IsCheck { get; private set; }
        public bool IsCheckMate { get; private set; }
        public bool IsStalemate { get; private set; }

        public string Fen 
        {
            get
            {
                return boardController.Fen;
            }
        }

        public Chess(string fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1")
        {
            boardController = new BoardController(fen);
            moves = new Moves(boardController);
            UpdateCheckFlags();
        }

        Chess(BoardController boardController)
        {
            this.boardController = boardController;
            this.moves = new Moves(boardController);
            UpdateCheckFlags();
        }

        public bool isValidMove(string move)
        {
            MoveController moveController = new MoveController(move);
            if (!moves.CanMove(moveController))
                return false;
            if (boardController.isCheckAfter(moveController))
                return false;
            return true;
        
        }

        public Chess Move(string move)
        {
            if (!isValidMove(move))
                return this;
            MoveController moveController = new MoveController(move);
            BoardController newBC = boardController.Move(moveController);
            Chess newChessEngine = new Chess(newBC);
            return newChessEngine;
        }

        public char GetFigureFromPosition(int x, int y)
        {
            Cell cell = new Cell(x, y);
            Figure figure = boardController.GetFigureAtCell(cell);
            return figure == Figure.none ? '.' : (char)figure;
        }

        public char GetFigureFromPosition(string coordinate)
        {
            Cell cell = new Cell(coordinate);
            Figure figure = boardController.GetFigureAtCell(cell);
            return figure == Figure.none ? '.' : (char)figure;
        }

        void UpdateCheckFlags()
        {
            IsCheck = boardController.isCheck();
            IsCheckMate = false;
            IsStalemate = false;
            foreach (string moves in YieldValidMoves())
                return;
            if (IsCheck)
                IsCheckMate = true;
            else IsStalemate = true;
        }

        public IEnumerable<string> YieldValidMoves()
        {
            foreach (FigureOnCell fc in boardController.YieldFiguresOnCell())
                foreach (Cell cell in Cell.YieldBoardCells())
                    foreach(Figure transformation in fc.figure.YieldTransformations(cell))
                    {
                        MoveController mc = new MoveController(fc, cell, transformation);
                        if (moves.CanMove(mc))
                            if (!boardController.isCheckAfter(mc))
                            yield return mc.ToString();
                    }
        }
    }
}

