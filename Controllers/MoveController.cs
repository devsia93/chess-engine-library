using System;
namespace ChessEngine
{
     class MoveController
    {
        public static MoveController none = new MoveController();

        public Figure CurrentFigure { get; private set; }
        public Cell CurrentCell { get; private set; }
        public Cell NewCell { get; private set; }
        public Figure Transformation { get; private set; }
        public Figure TransformatedFigure { get { return Transformation == Figure.none ? CurrentFigure : Transformation; } }

        public int DifferenceX {  get { return NewCell.x - CurrentCell.x; } }
        public int DifferenceY { get { return NewCell.y - CurrentCell.y; } }
        public int AbsDifferenceX { get { return Math.Abs(DifferenceX); } }
        public int AbsDifferenceY { get { return Math.Abs(DifferenceY); } }
        public int SignX { get { return Math.Sign(DifferenceX); } }
        public int SignY { get { return Math.Sign(DifferenceY); } }

        private MoveController()
        {
            CurrentFigure = Figure.none;
            CurrentCell = Cell.none;
            NewCell = Cell.none;
            Transformation = Figure.none;
        }

        public MoveController(FigureOnCell figureOnCell, Cell newCell, Figure transformation = Figure.none)
        {
            this.CurrentFigure = figureOnCell.figure;
            this.CurrentCell = figureOnCell.CurrentCell;
            this.NewCell = newCell;
            this.Transformation = transformation;
        }

        public MoveController(string move)//example for move values: Pe2e4, Ph7h8Q
        {
            this.CurrentFigure = (Figure)move[0];
            this.CurrentCell = new Cell(move.Substring(1, 2));
            this.NewCell = new Cell(move.Substring(3, 2));
            if (move.Length == 6)
                this.Transformation = (Figure)move[5];
            else this.Transformation = Figure.none;
        }

        public override string ToString()
        {
            return ((char)CurrentFigure).ToString() + CurrentCell.Name +
            NewCell.Name + (Transformation == Figure.none ? "" : ((char)Transformation).ToString());
        }
    }
}
