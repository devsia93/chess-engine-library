using System;
namespace ChessEngine
{
    class FigureOnCell
    {
        public Figure figure { get; private set; }
        public Cell CurrentCell { get; private set; }

        public FigureOnCell(Figure figure, Cell cell)
        {
            this.figure = figure;
            this.CurrentCell = cell;
        }
    }
}
