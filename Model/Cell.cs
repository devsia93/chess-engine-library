using System;
using System.Collections.Generic;

namespace ChessEngine
{
    struct Cell
    {
        public static Cell none = new Cell(-1, -1);

        public int x { get; private set; }
        public int y { get; private set; }

        public Cell(string nameCell)
        {
            //check coordinate for validity
            if (nameCell.Length == 2 &&
            nameCell[0] >= 'a' && nameCell[0] <= 'h' &&
                nameCell[1] >= '1' && nameCell[1] <= '8')
            {
                x = nameCell[0] - 'a';
                y = nameCell[1] - '1';
            }
            else this = none;
        }

        public Cell(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public string Name
        {
            get
            {
                if (CheckOnBoard())
                    return ((char)('a' + x)).ToString() + (y + 1).ToString();
                else return "-";
            }
        }

        public bool CheckOnBoard()
        {
            return (x >= 0 && x < Constants.COUNT_SQUARES) && 
            (y >= 0 && y < Constants.COUNT_SQUARES);
        }

        public static IEnumerable<Cell> YieldBoardCells()
        {
            for (int i = 0; i < Constants.COUNT_SQUARES; i++)
                for (int j = 0; j < Constants.COUNT_SQUARES; j++)
                {
                    yield return new Cell(j, i);
                }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Cell))
            {
                return false;
            }

            var cell = (Cell)obj;
            return x == cell.x &&
                   y == cell.y;
        }

        public override int GetHashCode()
        {
            var hashCode = 1502939027;
            hashCode = hashCode * -1521134295 + x.GetHashCode();
            hashCode = hashCode * -1521134295 + y.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(Cell a, Cell b) => a.x == b.x && a.y == b.y;

        public static bool operator !=(Cell a, Cell b) => !(a == b);

    }
}
