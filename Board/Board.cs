using System;
using System.Collections.Generic;

namespace ChessEngine
{
    internal class Board
    {
        internal Figure[,] figures;

        internal Board(FenParser.FenParseResult parsed)
        {
            figures = parsed.figures;
        }

        internal Figure GetFigureAtCell(Cell cell)
        {
            if (cell.CheckOnBoard())
                return figures[cell.x, cell.y];
            return Figure.none;
        }

        internal void SetFigureAtCell(Cell cell, Figure figure)
        {
            if (cell.CheckOnBoard())
                figures[cell.x, cell.y] = figure;
        }

        internal IEnumerable<FigureOnCell> YieldFiguresOnCell(Color color)
        {
            foreach (Cell cell in Cell.YieldBoardCells())
            {
                if (GetFigureAtCell(cell).GetColor() == color)
                    yield return new FigureOnCell(GetFigureAtCell(cell), cell);
            }
        }
    }
}
