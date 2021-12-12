using System;
using System.Collections.Generic;
using System.Text;

namespace AntSim.Sim
{
    public class Grid
    {
        private const int WallOffset = 1;

        private readonly Cell[,] _cells;

        public int Width { get; }
        public int Height { get; }

        public Grid(int width, int height)
        {
            if ((Width = width) < 1) throw new ArgumentOutOfRangeException(nameof(width));
            if ((Height = height) < 1) throw new ArgumentOutOfRangeException(nameof(height));

            _cells = new Cell[width + 2 * WallOffset, height + 2 * WallOffset];

            /*
             * Surround the world with walls to simply the logic.
             * For a 2x3 playable space, we use a 4x5 grid like:
             *
             *     W W W W
             *     W o o W        W = wall
             *     W o o W        o = space
             *     W o o W
             *     W W W W
             */
            for (var x = 0; x < width + 2 * WallOffset; x++)
            {
                for (var y = 0; y < height + 2 * WallOffset; y++)
                {
                    var c = _cells[x, y] = new Cell(this, x, y)
                    {
                        IsWall = x < WallOffset
                                 || y < WallOffset
                                 || x > width + WallOffset - 1
                                 || y > height + WallOffset - 1
                    };
                }
            }
        }


        public Cell this[int x, int y] => _cells[x, y];


        public IEnumerable<Cell> GetCells()
        {
            for (var x = 0; x < _cells.GetLength(0); x++)
            {
                for (var y = 0; y < _cells.GetLength(1); y++)
                {
                    yield return _cells[x, y];
                }
            }
        }

        public override string ToString() => $"({Width} x {Height})";


        public string Print()
        {
            var sb = new StringBuilder();

            for (var y = 0; y < _cells.GetLength(1); y++)
            {
                for (var x = 0; x < _cells.GetLength(0); x++)
                {
                    var cell = _cells[x, y];
                    var c = " ";

                    if (cell.IsWall)
                    {
                        c = "=";
                    }
                    else if (cell.Creature != null)
                    {
                        c = ".";
                    }

                    sb.Append(c);
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}